

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
            var oldStudentClass = await _unitOfWork.StudentClassRepository.FindSingleAsync(sc => sc.StudentFirebaseId == changeClassModel.StudentFirebaseId && sc.ClassId == changeClassModel.OldClassId);
            if (oldStudentClass is null)
            {
                throw new NotFoundException("Student class not found");
            }
            var oldClassInfo = (await _unitOfWork.ClassRepository.Entities
                .Include(c => c.Slots)
                .Include(oc => oc.StudentClasses)
                .SingleOrDefaultAsync(oc => oc.Id == oldStudentClass.ClassId))!;

            var student = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == changeClassModel.StudentFirebaseId);
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
            var classSlotIds = classInfo.Slots.Select(s => s.Id).ToList();
            var existedStudentSlots = await _unitOfWork.SlotStudentRepository.FindAsync(ss => ss.StudentFirebaseId == changeClassModel.StudentFirebaseId
                && classSlotIds.Contains(ss.SlotId), false, true);
            var studentSlots = classInfo.Slots.Select(s => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = s.Id,
                StudentFirebaseId = changeClassModel.StudentFirebaseId
            });

            studentSlots = studentSlots.Where(ss => !existedStudentSlots.Any(es => es.SlotId == ss.SlotId && ss.StudentFirebaseId == es.StudentFirebaseId)).ToList();


            foreach (var slot in existedStudentSlots)
            {
                slot.RecordStatus = RecordStatus.IsActive;
            }

            student.CurrentClassId = classInfo.Id;

            //Update old studentClass
            oldStudentClass.ClassId = changeClassModel.NewClassId;
            oldStudentClass.UpdateById = accountFirebaseId;
            oldStudentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);

            //Delete old studentSlots
            var oldSlotIds = oldClassInfo.Slots.Select(s => s.Id).ToList();
            var oldStudentSlots = await _unitOfWork.SlotStudentRepository.FindAsync(ss => oldSlotIds.Contains(ss.SlotId));
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
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(existedStudentSlots);

                //Delete
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(oldStudentSlots);
                await _unitOfWork.SaveChangesAsync();
            });
            //Notification
            
            await _serviceFactory.NotificationService.SendNotificationAsync(changeClassModel.StudentFirebaseId, "Thông tin lớp mới",
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
                $"Học sinh {student.FullName ?? student.UserName} đã chuyển ra khỏi lớp {oldClassInfo.Name}. Nếu có thắc mắc hoặc báo cáo nhầm lẫn, vui lòng nộp đơn khiếu nại hoặc liên hệ bộ phận hỗ trợ!", student.AvatarUrl ?? "");
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(newClassReceiverIds,
                $"Học sinh mới {student.FullName ?? student.UserName} được thêm vào lớp {classInfo.Name}. Hãy giúp đỡ bạn ấy hết mình!", student.AvatarUrl ?? "");
            
        }

        public async Task<List<StudentClassModel>> CreateStudentClass(CreateStudentClassModel createStudentClassesModel, string accountFirebaseId)
        {
            var students = await _unitOfWork.AccountRepository.FindAsync(a => createStudentClassesModel.StudentFirebaseIds.Contains(a.AccountFirebaseId));
            if (!students.Any() && !createStudentClassesModel.IsAutoFill)
            {
                throw new NotFoundException("No valid students found");
            }

            foreach (var student in students)
            {
                if (student.StudentStatus != StudentStatus.WaitingForClass)
                {
                    throw new BadRequestException($"Student {student.FullName ?? student.UserName} is already in a class");
                }
            }

            
            var classInfo = await _unitOfWork.ClassRepository.Entities
                .Include(c => c.StudentClasses)
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == createStudentClassesModel.ClassId);
            if (classInfo is null)
            {
                throw new NotFoundException("Class not found");
            }

            var maxStudents = int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

            if (classInfo.StudentClasses.Count + students.Count > maxStudents)
            {
                throw new BadRequestException("Class is full!");
            }
            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Class is finished");
            }
            if (createStudentClassesModel.IsAutoFill && maxStudents - classInfo.StudentClasses.Count - students.Count > 0)
            {
                var otherStudents = await _unitOfWork.AccountRepository.FindAsQueryable(s => s.StudentStatus == StudentStatus.WaitingForClass
                    && !createStudentClassesModel.StudentFirebaseIds.Contains(s.AccountFirebaseId))
                    .Take(maxStudents - classInfo.StudentClasses.Count - students.Count)
                    .ToListAsync();

                students.AddRange(otherStudents);
            }

            var receiverIds = new List<string>();
            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var addedStudentClasses = new List<StudentClass>();
                var studentSlots = new List<SlotStudent>();
                var existedSlotStudents = new List<SlotStudent>();
                var studentClassScores = new List<StudentClassScore>();

                foreach (var student in students)
                {
                    var studentClass = new StudentClass
                    {
                        Id = Guid.NewGuid(),
                        ClassId = classInfo.Id,
                        StudentFirebaseId = student.AccountFirebaseId,
                        CreatedById = accountFirebaseId,
                        IsPassed = false
                    };

                    student.StudentStatus = StudentStatus.InClass;
                    student.CurrentClassId = classInfo.Id;
                    addedStudentClasses.Add(studentClass);

                    var classSlotIds = classInfo.Slots.Select(s => s.Id).ToList();
                    var existedSlots = await _unitOfWork.SlotStudentRepository.FindAsync(s => s.StudentFirebaseId == student.AccountFirebaseId
                        && classSlotIds.Contains(s.SlotId) && s.RecordStatus == RecordStatus.IsDeleted, false, true);

                    studentSlots.AddRange(classInfo.Slots.Select(s => new SlotStudent
                    {
                        CreatedById = accountFirebaseId,
                        SlotId = s.Id,
                        StudentFirebaseId = student.AccountFirebaseId
                    }));

                    studentSlots = studentSlots.Where(ss => !existedSlots.Any(es => es.SlotId == ss.SlotId && ss.StudentFirebaseId == es.StudentFirebaseId)).ToList();

                    foreach (var slot in existedSlotStudents)
                    {
                        slot.RecordStatus = RecordStatus.IsActive;
                    }

                    if (classInfo.IsPublic)
                    {
                        var criteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.Class);
                        studentClassScores.AddRange(criteria.Select(c => new StudentClassScore
                        {
                            Id = Guid.NewGuid(),
                            StudentClassId = studentClass.Id,
                            CriteriaId = c.Id
                        }));
                    }
                    existedSlotStudents.AddRange(existedSlots);
                    receiverIds.Add(student.AccountFirebaseId);
                }

                await _unitOfWork.StudentClassRepository.AddRangeAsync(addedStudentClasses);
                await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
                await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(studentClassScores);
                await _unitOfWork.AccountRepository.UpdateRangeAsync(students);
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(existedSlotStudents);
                await _unitOfWork.SaveChangesAsync();

                return addedStudentClasses;
            });

            if (classInfo.IsPublic)
            {
                var classReceiverIds = new List<string>();
                if (classInfo.InstructorId != null)
                {
                    classReceiverIds.Add(classInfo.InstructorId);
                }
                classReceiverIds = classInfo.StudentClasses.Select(sc => sc.StudentFirebaseId).ToList();

                await _serviceFactory.NotificationService.SendNotificationToManyAsync(students.Select(s => s.AccountFirebaseId).ToList(), "Thông tin lớp mới",
                        $"Chúc mừng bạn đã được thêm vào lớp mới {classInfo.Name}. Vui lòng kiểm tra lại lịch học. Chúc các bạn gặt hái được nhiều thành công!");
                await _serviceFactory.NotificationService.SendNotificationToManyAsync(classReceiverIds,
                    $"{students.Count} học sinh mới đã được thêm vào lớp {classInfo.Name}. Hãy giúp đỡ các bạn ấy hết mình!","");
            }

            return result.Adapt<List<StudentClassModel>>();
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

            var studentClass = await _unitOfWork.StudentClassRepository.FindSingleAsync(sc => sc.StudentFirebaseId == studentId && sc.ClassId == classId);
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
            var studentClassScores = await _unitOfWork.StudentClassScoreRepository.FindAsync(scs => scs.StudentClassId == studentClass.Id);
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
            var classDetails = await _serviceFactory.ClassService.GetClassDetailById(classId);
            var classCriteria = await _serviceFactory.CriteriaService.GetAllCriteriaDetails(classId);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Grades");
            var metadataSheet = package.Workbook.Worksheets.Add("Metadata");

            // Configure metadata sheet (hidden from users)
            metadataSheet.Hidden = eWorkSheetHidden.Hidden;
            metadataSheet.Cells[1, 1].Value = "ClassId";
            metadataSheet.Cells[1, 2].Value = classId.ToString();

            // Headers
            worksheet.Cells[1, 1].Value = "Grade book";
            worksheet.Cells[1, 1, 1, 5].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.FromArgb(139, 69, 19));
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));

            // Course and instructor info
            worksheet.Cells[2, 1].Value = $"Course: {classDetails.Name}";
            worksheet.Cells[2, 1, 2, 5].Merge = true;
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1].Style.Font.Size = 12;

            worksheet.Cells[3, 1].Value = $"Instructor: {classDetails.Instructor!.UserName}";
            worksheet.Cells[3, 1, 3, 5].Merge = true;
            worksheet.Cells[3, 1].Style.Font.Bold = true;
            worksheet.Cells[3, 1].Style.Font.Size = 12;

            worksheet.Cells[4, 1].Value = "Assignments";
            worksheet.Cells[4, 1, 4, 5].Merge = true;
            worksheet.Cells[4, 1].Style.Font.Bold = true;
            worksheet.Cells[4, 1].Style.Font.Size = 12;
            worksheet.Cells[4, 1].Style.Font.Color.SetColor(Color.FromArgb(0, 112, 192));

            // Add border to header section
            var headerRange = worksheet.Cells[1, 1, 4, 5];
            headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            // Define column headers
            worksheet.Cells[7, 1].Value = "Student Name";
            worksheet.Cells[7, 1].Style.Font.Bold = true;
            worksheet.Cells[7, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[7, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
            worksheet.Cells[7, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            int startCol = 2;
            var assessments = classCriteria.Select(c => c.Name).ToArray();
            var weights = classCriteria.Select(c => (double)c.Weight).ToArray();
            Dictionary<string, int> headerCount = new Dictionary<string, int>();
            // Assessment headers
            for (int i = 0; i < assessments.Length; i++)
            {
                string assessmentName = assessments[i];

                // Handle duplicates by appending a number (e.g., "Exam (1)", "Exam (2)")
                if (headerCount.ContainsKey(assessmentName))
                {
                    headerCount[assessmentName]++;
                    assessmentName = $"{assessmentName} ({headerCount[assessmentName]})";
                }
                else
                {
                    headerCount[assessmentName] = 1;
                }
                
                var cell = worksheet.Cells[6, startCol + i];
                var nameCell = worksheet.Cells[7, startCol + i];
                nameCell.Value = assessmentName;
                nameCell.Style.Font.Bold = true;
                nameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                nameCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
                nameCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                nameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Assign weight to row 8
                var weightCell = worksheet.Cells[8, startCol + i];
                weightCell.Value = weights[i];
                weightCell.Style.Numberformat.Format = "0.0\\%";
                weightCell.Style.Font.Bold = true;
                weightCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                weightCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 239, 218));
                weightCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                weightCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int finalCol = startCol + assessments.Length;
            worksheet.Cells[7, finalCol].Value = "Total";
            worksheet.Cells[7, finalCol + 1].Value = "%";
            worksheet.Cells[7, finalCol + 2].Value = "Grade";

            int studentStartRow = 9;
            foreach (var studentClass in classDetails.StudentClasses)
            {
                worksheet.Cells[studentStartRow, 1].Value = studentClass.Student.FullName;

                // Total Score Formula
                var totalCell = worksheet.Cells[studentStartRow, finalCol];
                totalCell.Formula =
                    $"SUM({worksheet.Cells[studentStartRow, 2].Address}:{worksheet.Cells[studentStartRow, finalCol - 1].Address})";
                
                // Weighted Percentage Formula
                var percentCell = worksheet.Cells[studentStartRow, finalCol + 1];
                percentCell.Formula =
                    $"SUMPRODUCT({worksheet.Cells[studentStartRow, 2].Address}:{worksheet.Cells[studentStartRow, finalCol - 1].Address}, {worksheet.Cells[8, 2].Address}:{worksheet.Cells[8, finalCol - 1].Address})/100";
                percentCell.Style.Numberformat.Format = "0.0\\%";
                studentStartRow++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(9, 2);

            return await package.GetAsByteArrayAsync();
        }

        public async Task<bool> ImportScores(Guid classId, Stream excelFileStream, AccountModel account)
        {
            var classDetails = await _serviceFactory.ClassService.GetClassDetailById(classId);

            var classCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.Class);

            if (!classCriteria.Any())
            {
                throw new BadRequestException("Assessment criteria not found for this class");
            }

            using var package = new ExcelPackage(excelFileStream);
            var worksheet = package.Workbook.Worksheets[0];

            //Check valid template
            ValidateExcelTemplate(worksheet);

            //Get metadata sheet 
            var metaDataSheet = package.Workbook.Worksheets["Metadata"];
            if (metaDataSheet == null || metaDataSheet.Cells[1, 2].Text != classId.ToString())
            {
                throw new BadRequestException("Invalid template: This template is not for the selected class");
            }

            // Map criteria names to columns and IDs
            var criteriaMapping = MapCriteriaToColumns(worksheet, classCriteria);

            // Starting row for student data
            int studentStartRow = 9;
            int rows = worksheet.Dimension.Rows;

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Process each student row
                for (int row = studentStartRow; row <= rows; row++)
                {
                    string studentName = worksheet.Cells[row, 1].Text;
                    if (string.IsNullOrEmpty(studentName))
                        continue; // Skip empty rows

                    // Find the student in class details
                    var studentClass = classDetails.StudentClasses.FirstOrDefault(sc =>
                        string.Equals(sc.Student.FullName, studentName, StringComparison.OrdinalIgnoreCase));

                    if (studentClass == null)
                        continue; // Skip if student not found

                    // Calculate total percentage from cell
                    decimal totalPercentage = ExtractTotalPercentage(worksheet, row);

                    // Update student scores
                    await UpdateStudentClassScores(studentClass.Id, worksheet, row, criteriaMapping,
                        account.AccountFirebaseId);

                    // Update StudentClass GPA and pass status
                    await UpdateStudentClassGPA(studentClass.Id, totalPercentage, account.AccountFirebaseId,
                        classDetails.Name);
                }

                return true;
            });
        }

        private void ValidateExcelTemplate(ExcelWorksheet worksheet)
        {
            if (worksheet.Dimension.Columns < 13)
            {
                throw new BadRequestException("Invalid Excel template: Missing required columns");
            }

            string courseName = worksheet.Cells[2, 1].Text;
            string instructorName = worksheet.Cells[3, 1].Text;
            string assignmentsHeader = worksheet.Cells[4, 1].Text;

            // if (string.IsNullOrEmpty(courseName) || !courseName.StartsWith("Course:") ||
            //     string.IsNullOrEmpty(instructorName) || !instructorName.StartsWith("Instructor:") ||
            //     string.IsNullOrEmpty(assignmentsHeader) ||
            //     !assignmentsHeader.Equals("Assignments", StringComparison.OrdinalIgnoreCase))
            // {
            //     throw new BadRequestException(
            //         "Invalid Excel template: Header structure does not match required format");
            // }

            // Check for student column
            string studentHeader = worksheet.Cells[7, 1].Text;
            if (string.IsNullOrEmpty(studentHeader) ||
                !studentHeader.Equals("Student Name", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Invalid Excel template: Student column not found");
            }
        }

        private Dictionary<string, (int Column, Guid Id)> MapCriteriaToColumns(ExcelWorksheet worksheet,
            IEnumerable<Criteria> classCriteria)
        {
            int startCol = 2;
            var mapping = new Dictionary<string, (int Column, Guid Id)>(StringComparer.OrdinalIgnoreCase);

            // Map criteria names to column indices (starting from column 2)
            for (int col = startCol; col < worksheet.Dimension.Columns - 2; col++) {
                string criteriaName = worksheet.Cells[7, col].Text;
                if (!string.IsNullOrEmpty(criteriaName))
                {
                    // Find matching criteria ID
                    var matchingCriteria = classCriteria.FirstOrDefault(c =>
                        string.Equals(c.Name, criteriaName, StringComparison.OrdinalIgnoreCase));

                    if (matchingCriteria != null)
                    {
                        mapping[criteriaName] = (col, matchingCriteria.Id);
                    }
                }
            }

            return mapping;
        }

        private decimal ExtractTotalPercentage(ExcelWorksheet worksheet, int row)
        {
            decimal totalPercentage = 0;
            var percentCell = worksheet.Cells[row, worksheet.Dimension.Columns - 1];
            if (decimal.TryParse(percentCell.Text.Replace("%", ""), out decimal percentage))
            {
                totalPercentage = percentage;
            }
            else if (percentCell.Formula != null)
            {
                // Try to get calculated value if it's a formula
                totalPercentage = (decimal)(percentCell.Value != null ? Convert.ToDouble(percentCell.Value) * 100 : 0);
            }

            return totalPercentage;
        }

        private async Task UpdateStudentClassScores(Guid studentClassId, ExcelWorksheet worksheet, int row,
            Dictionary<string, (int Column, Guid Id)> criteriaMapping, string accountFirebaseId)
        {
            // Get existing scores for this student class
            var existingScores = await _unitOfWork.StudentClassScoreRepository.FindAsync(
                scs => scs.StudentClassId == studentClassId
            );

            List<StudentClassScore> scoresToUpdate = new List<StudentClassScore>();
            List<StudentClassScore> scoresToAdd = new List<StudentClassScore>();

            // Update individual criteria scores
            foreach (var criteriaEntry in criteriaMapping)
            {
                string criteriaName = criteriaEntry.Key;
                int col = criteriaEntry.Value.Column;
                Guid criteriaId = criteriaEntry.Value.Id;

                decimal? score = null;
                if (decimal.TryParse(worksheet.Cells[row, col].Text, out decimal parsedScore))
                {
                    score = parsedScore;
                }

                // Find existing score record or create new one
                var scoreRecord = existingScores.FirstOrDefault(s => s.CriteriaId == criteriaId);

                if (scoreRecord != null)
                {
                    // Update existing record
                    scoreRecord.Score = score;
                    scoreRecord.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    scoresToUpdate.Add(scoreRecord);
                }
                else
                {
                    // Create new score record
                    var newScore = new StudentClassScore
                    {
                        Id = Guid.NewGuid(),
                        StudentClassId = studentClassId,
                        CriteriaId = criteriaId,
                        Score = score,
                    };
                    scoresToAdd.Add(newScore);
                }
            }

            // Update and add scores
            if (scoresToUpdate.Any())
            {
                await _unitOfWork.StudentClassScoreRepository.UpdateRangeAsync(scoresToUpdate);
            }

            if (scoresToAdd.Any())
            {
                await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(scoresToAdd);
            }
        }

        private async Task UpdateStudentClassGPA(Guid studentClassId, decimal totalPercentage, string accountFirebaseId,
            string className)
        {
            var studentClass = await _unitOfWork.StudentClassRepository.GetByIdAsync(studentClassId);
            if (studentClass != null)
            {
                studentClass.GPA = totalPercentage / 10; // Convert percentage to GPA scale (assuming 100% = 10.0)
                studentClass.UpdateById = accountFirebaseId;
                studentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);

                // Determine if passed based on GPA threshold (e.g., 5.0)
                decimal passThreshold = 5.0m; // This should ideally come from configuration
                studentClass.IsPassed = studentClass.GPA >= passThreshold;

                await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);

                // Send notification to student
                await _serviceFactory.NotificationService.SendNotificationAsync(
                    studentClass.StudentFirebaseId,
                    "Grade Update",
                    $"Your grades for class {className} have been updated. Please check your performance in the class dashboard."
                );
            }
        }
    }
}

