

using System.Drawing;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services
{
    public class StudentClassService : IStudentClassService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IUnitOfWork _unitOfWork;

        public StudentClassService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
        {
            _serviceFactory = serviceFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task ChangeClassOfStudent(ChangeClassModel changeClassModel, string accountFirebaseId)
        {
            var oldStudentClass = await _unitOfWork.StudentClassRepository.FindSingleAsync(sc =>
                sc.StudentFirebaseId == changeClassModel.StudentFirebaseId &&
                sc.ClassId == changeClassModel.OldClassId);
            if (oldStudentClass is null)
            {
                throw new NotFoundException("Student class not found");
            }

            var oldClassInfo = (await _unitOfWork.ClassRepository.Entities.Include(oc => oc.StudentClasses)
                .SingleOrDefaultAsync(oc => oc.Id == oldStudentClass.ClassId))!;

            var student =
                await _unitOfWork.AccountRepository.FindSingleAsync(a =>
                    a.AccountFirebaseId == changeClassModel.StudentFirebaseId);
            if (student is null)
            {
                throw new NotFoundException("Student not found");
            }

            if (student.StudentStatus != StudentStatus.InClass)
            {
                throw new BadRequestException("Student is currently not belong to any class");
            }

            var classInfo = await _unitOfWork.ClassRepository.Entities
                .Include(c => c.StudentClasses)
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == changeClassModel.NewClassId);
            if (classInfo is null)
            {
                throw new NotFoundException("Class not found");
            }

            if (!oldClassInfo.IsPublic || !classInfo.IsPublic)
            {
                throw new BadRequestException("Both class need to be published to use this feature");
            }

            var maxStudents =
                int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

            if (classInfo.StudentClasses.Count >= maxStudents)
            {
                throw new BadRequestException("Class is full!");
            }

            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Class is finished");
            }

            //Create student slots
            var studentSlots = classInfo.Slots.Select(s => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = s.Id,
                StudentFirebaseId = changeClassModel.StudentFirebaseId
            });

            student.CurrentClassId = classInfo.Id;

            //Update old studentClass
            oldStudentClass.ClassId = changeClassModel.NewClassId;
            oldStudentClass.UpdateById = accountFirebaseId;
            oldStudentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);

            //Delete old studentSlots
            var oldSlotIds = classInfo.Slots.Select(s => s.Id).ToList();
            var oldStudentSlots =
                await _unitOfWork.SlotStudentRepository.FindAsync(ss => oldSlotIds.Contains(ss.SlotId));
            foreach (var oldStudentSlot in oldStudentSlots)
            {
                oldStudentSlot.RecordStatus = RecordStatus.IsDeleted;
                oldStudentSlot.DeletedAt = DateTime.UtcNow.AddHours(7);
                oldStudentSlot.DeletedById = accountFirebaseId;
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
                await _unitOfWork.AccountRepository.UpdateAsync(student);
                await _unitOfWork.StudentClassRepository.UpdateAsync(oldStudentClass);

                //Delete
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(oldStudentSlots);
                await _unitOfWork.SaveChangesAsync();
            });
            //Notification

            await _serviceFactory.NotificationService.SendNotificationAsync(changeClassModel.StudentFirebaseId,
                "Thông tin lớp mới",
                $"Chúc mừng bạn đã được thêm vào lớp mới {classInfo.Name}. Vui lòng kiểm tra lại lịch học. Chúc các bạn gặt hái được nhiều thành công!");

            var newClassReceiverIds = classInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
            if (classInfo.InstructorId != null)
            {
                newClassReceiverIds.Add(classInfo.InstructorId);
            }

            var oldClassReceiverIds = oldClassInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
            if (oldClassInfo.InstructorId != null)
            {
                oldClassReceiverIds.Add(oldClassInfo.InstructorId);
            }

            await _serviceFactory.NotificationService.SendNotificationToManyAsync(oldClassReceiverIds,
                $"Học sinh {student.FullName ?? student.UserName} đã chuyển ra khỏi lớp {oldClassInfo.Name}. Nếu có thắc mắc hoặc báo cáo nhầm lẫn, vui lòng nộp đơn khiếu nại hoặc liên hệ bộ phận hỗ trợ!",
                student.AvatarUrl ?? "");
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(newClassReceiverIds,
                $"Học sinh mới {student.FullName ?? student.UserName} được thêm vào lớp {classInfo.Name}. Hãy giúp đỡ bạn ấy hết mình!",
                student.AvatarUrl ?? "");

        }

        public async Task<StudentClassModel> CreateStudentClass(CreateStudentClassModel createStudentClassModel,
            string accountFirebaseId)
        {
            var student = await _unitOfWork.AccountRepository.FindSingleAsync(a =>
                a.AccountFirebaseId == createStudentClassModel.StudentFirebaseId);
            if (student is null)
            {
                throw new NotFoundException("Student not found");
            }

            if (student.StudentStatus != StudentStatus.WaitingForClass)
            {
                throw new BadRequestException("Student is already in a class");
            }

            var classInfo = await _unitOfWork.ClassRepository.Entities
                .Include(c => c.StudentClasses)
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == createStudentClassModel.ClassId);
            if (classInfo is null)
            {
                throw new NotFoundException("Class not found");
            }

            var maxStudents =
                int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

            if (classInfo.StudentClasses.Count >= maxStudents)
            {
                throw new BadRequestException("Class is full!");
            }

            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Class is finished");
            }

            //Create student slots
            var studentSlots = classInfo.Slots.Select(s => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = s.Id,
                StudentFirebaseId = createStudentClassModel.StudentFirebaseId
            });



            var studentClass = createStudentClassModel.Adapt<StudentClass>();
            studentClass.CreatedById = accountFirebaseId;
            studentClass.IsPassed = false;
            student.StudentStatus = StudentStatus.InClass;
            student.CurrentClassId = classInfo.Id;

            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var addedStudentClass = await _unitOfWork.StudentClassRepository.AddAsync(studentClass);

                //Create student class score
                if (classInfo.IsPublic)
                {
                    var criteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.Class);
                    var studentClassScores = criteria.Select(c => new StudentClassScore
                    {
                        Id = Guid.NewGuid(),
                        StudentClassId = addedStudentClass.Id,
                        CriteriaId = c.Id
                    });

                    await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(studentClassScores);
                }

                await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
                await _unitOfWork.AccountRepository.UpdateAsync(student);
                await _unitOfWork.SaveChangesAsync();
                return addedStudentClass;
            });
            //Notification
            if (classInfo.IsPublic)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(
                    createStudentClassModel.StudentFirebaseId, "Thông tin lớp mới",
                    $"Chúc mừng bạn đã được thêm vào lớp mới {classInfo.Name}. Vui lòng kiểm tra lại lịch học. Chúc các bạn gặt hái được nhiều thành công!");

                var receiverIds = classInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
                if (classInfo.InstructorId != null)
                {
                    receiverIds.Add(classInfo.InstructorId);
                }

                await _serviceFactory.NotificationService.SendNotificationToManyAsync(receiverIds,
                    $"Học sinh mới {student.FullName ?? student.UserName} được thêm vào lớp {classInfo.Name}. Hãy giúp đỡ bạn ấy hết mình!",
                    student.AvatarUrl ?? "");
            }

            return result.Adapt<StudentClassModel>();
        }

        public async Task DeleteStudentClass(string studentId, Guid classId, bool isExpelled, string accountFirebaseId)
        {
            var student = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == studentId);
            if (student is null)
            {
                throw new NotFoundException("Student not found");
            }

            var classInfo = await _unitOfWork.ClassRepository.Entities
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classInfo is null)
            {
                throw new NotFoundException("Class not found");
            }

            var studentClass =
                await _unitOfWork.StudentClassRepository.FindSingleAsync(sc =>
                    sc.StudentFirebaseId == studentId && sc.ClassId == classId);
            if (studentClass is null)
            {
                throw new NotFoundException("Student class not found");
            }

            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Class is finished");
            }

            student.StudentStatus = isExpelled ? StudentStatus.DropOut : StudentStatus.WaitingForClass;
            student.CurrentClassId = null;
            //Delete studentClass
            studentClass.RecordStatus = RecordStatus.IsDeleted;
            studentClass.DeletedById = accountFirebaseId;
            studentClass.DeletedAt = DateTime.UtcNow.AddHours(7);

            //Delete studentClassScore
            var studentClassScores =
                await _unitOfWork.StudentClassScoreRepository.FindAsync(scs => scs.StudentClassId == studentClass.Id);
            foreach (var studentScore in studentClassScores)
            {
                studentScore.RecordStatus = RecordStatus.IsDeleted;
                studentScore.DeletedAt = DateTime.UtcNow.AddHours(7);
            }

            //Delete studentSlots
            var slotIds = classInfo.Slots.Select(s => s.Id).ToList();
            var studentSlots = await _unitOfWork.SlotStudentRepository.FindAsync(ss => slotIds.Contains(ss.SlotId));
            foreach (var studentSlot in studentSlots)
            {
                studentSlot.RecordStatus = RecordStatus.IsDeleted;
                studentSlot.DeletedAt = DateTime.UtcNow.AddHours(7);
                studentSlot.DeletedById = accountFirebaseId;
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.StudentClassScoreRepository.UpdateRangeAsync(studentClassScores);
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(studentSlots);
                await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);
                await _unitOfWork.AccountRepository.UpdateAsync(student);
                await _unitOfWork.SaveChangesAsync();
            });


            //Notification
            if (classInfo.IsPublic)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(accountFirebaseId, "Xóa khỏi lớp",
                    $"Bạn đã bị xóa khỏi lớp {classInfo.Name}. Nếu có thắc mặc hoặc cho rằng đây là sự nhầm lẫn, vui lòng gửi đơn khiếu nại hoặc liên hệ trực tiếp bộ phận hỗ trợ!");

                var receiverIds = classInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
                if (classInfo.InstructorId != null)
                {
                    receiverIds.Add(classInfo.InstructorId);
                }

                await _serviceFactory.NotificationService.SendNotificationToManyAsync(receiverIds,
                    $"Học viên {student.FullName ?? student.UserName} đã bị xóa khỏi lớp {classInfo.Name}. Nếu có thắc mắc hoặc cho rằng đây là sự nhầm lẫn, vui lòng gửi đơn khiếu nại hoặc liên hệ trực tiếp bộ phận hỗ trợ!",
                    student.AvatarUrl ?? "");
            }
        }

        //Down excel
        public async Task<byte[]> GenerateGradeTemplate(Guid classId)
        {
            //fetch class details 
            var classDetails = await _serviceFactory.ClassService.GetClassDetailById(classId);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Grades");

            //Add headers
            worksheet.Cells[1, 1].Value = "Grade book";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.FromArgb(139, 69, 19));
            worksheet.Cells[2, 1].Value = $"Course: {classDetails.Name}";
            worksheet.Cells[3, 1].Value = $"Instructor: {classDetails.Instructor.UserName}";
            worksheet.Cells[4, 1].Value = "Assignments";

            // Define column headers
            worksheet.Cells[6, 1].Value = "Student Name";

            var assignmentHeader = worksheet.Cells[6, 1, 6, 15];
            assignmentHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
            assignmentHeader.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(222, 184, 170)); // Light brown
            assignmentHeader.Style.Font.Bold = true;

            // Column Headers - HW and Exam columns
            int startCol = 2;
            string[] assignments =
            {
                "HW-1", "HW-2", "HW-3", "HW-4", "Exam-1", "HW-5", "HW-6", "HW-7", "HW-8", "Exam-2", "HW-9",
                "HW-10", "HW-11", "Final"
            };

            // Create assignment columns
            for (int i = 0; i < assignments.Length; i++)
            {
                worksheet.Cells[7, startCol + i].Value = assignments[i];
                worksheet.Cells[8, startCol + i].Value = "50"; // Points/Weighting row
            }

            // Points/Weighting row styling
            var pointsRow = worksheet.Cells[8, 2, 8, startCol + assignments.Length - 1];
            pointsRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            pointsRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            pointsRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Student column
            worksheet.Cells[7, 1].Value = "Student";
            worksheet.Cells[7, 1].Style.Font.Bold = true;

            // Final columns
            int finalCol = startCol + assignments.Length;
            worksheet.Cells[7, finalCol].Value = "Total";
            worksheet.Cells[7, finalCol + 1].Value = "%";
            worksheet.Cells[7, finalCol + 2].Value = "Grade";

            // Add grade conversion table
            int gradeStartRow = 7;
            string[,] gradeConversion =
            {
                { "Grade", "Percent", "Performance" },
                { "A++", "100%", "Perfect (or with extra credit)" },
                { "A+", "98%", "Excellent" },
                { "A", "95%", "Excellent" },
                { "A-", "92%", "Excellent" },
                { "B+", "89%", "Good" },
                { "B", "86%", "Good" },
                { "B-", "83%", "Good" },
                { "C+", "79%", "Satisfactory" },
                { "C", "75%", "Satisfactory" },
                { "C-", "72%", "Satisfactory" },
                { "D+", "69%", "Passing" },
                { "D", "65%", "Passing" },
                { "D-", "62%", "Passing" },
                { "F", "55%", "Failure" }
            };

            // Position grade conversion table to the right
            int gradeTableCol = finalCol + 4;
            worksheet.Cells[gradeStartRow, gradeTableCol].LoadFromArrays(
                Enumerable.Range(0, gradeConversion.GetLength(0))
                    .Select(i => Enumerable.Range(0, gradeConversion.GetLength(1))
                        .Select(j => (object)gradeConversion[i, j])
                        .ToArray())
            );


            // Style grade conversion table
            var gradeTable =
                worksheet.Cells[gradeStartRow, gradeTableCol, gradeStartRow + 14, gradeTableCol + 2];
            gradeTable.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            gradeTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            gradeTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            gradeTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            gradeTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            // Add student rows
            int studentStartRow = 9;
            foreach (var studentClass in classDetails.StudentClasses)
            {
                worksheet.Cells[studentStartRow, 1].Value = studentClass.Student.UserName;

                // Add formula for total
                var totalCell = worksheet.Cells[studentStartRow, finalCol];
                totalCell.Formula =
                    $"SUM({worksheet.Cells[studentStartRow, 2].Address}:{worksheet.Cells[studentStartRow, finalCol - 1].Address})";

                // Add formula for percentage
                var percentCell = worksheet.Cells[studentStartRow, finalCol + 1];
                percentCell.Formula =
                    $"{totalCell.Address}/(SUM({worksheet.Cells[8, 2].Address}:{worksheet.Cells[8, finalCol - 1].Address}))*100";
                percentCell.Style.Numberformat.Format = "0.0\\%";

                studentStartRow++;
            }

            // Add Class Average and Median rows at the bottom
            int lastRow = studentStartRow + 1;
            worksheet.Cells[lastRow, 1].Value = "Class Average:";
            worksheet.Cells[lastRow + 1, 1].Value = "Median:";

            // Style for average and median rows
            var statsRows = worksheet.Cells[lastRow, 1, lastRow + 1, finalCol + 2];
            statsRows.Style.Fill.PatternType = ExcelFillStyle.Solid;
            statsRows.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(222, 184, 170)); // Light brown

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Freeze panes
            worksheet.View.FreezePanes(9, 2);

            return package.GetAsByteArray();
        }
    }
    
        // public async Task<bool> ImportScores(Guid classId, Stream excelFileStream, AccountModel account)
        // {
        //     
        // }
}
