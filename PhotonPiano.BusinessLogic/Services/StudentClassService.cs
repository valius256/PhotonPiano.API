using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

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

        public async Task ChangeClassOfStudent(ChangeClassModel changeClassModel, AccountModel account)
        {
            if (account.Role == Role.Student)
            {
                changeClassModel.StudentFirebaseId = account.AccountFirebaseId;
            }

            var oldStudentClass = await _unitOfWork.StudentClassRepository.FindSingleAsync(sc =>
                sc.StudentFirebaseId == changeClassModel.StudentFirebaseId &&
                sc.ClassId == changeClassModel.OldClassId);
            if (oldStudentClass is null)
            {
                throw new NotFoundException("Student class not found");
            }

            var oldClassInfo = (await _unitOfWork.ClassRepository.Entities
                .Include(c => c.Slots)
                .Include(oc => oc.StudentClasses)
                .SingleOrDefaultAsync(oc => oc.Id == oldStudentClass.ClassId))!;

            if (account.Role == Role.Student)
            {
                var deadlineDays =
                    int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.ChangingClassDeadline))
                        .ConfigValue ?? "0");
                var firstSlot = oldClassInfo.Slots.OrderBy(s => s.Date).OrderBy(s => s.Shift).FirstOrDefault();
                if (firstSlot != null && DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7).AddDays(deadlineDays)) >
                    firstSlot.Date)
                {
                    throw new BadRequestException("The deadline for changing class has been overdued");
                }
            }

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

            if (classInfo.LevelId != student.LevelId)
            {
                throw new BadRequestException("Student is not in the same level as the class");
            }

            var allowSkipLevel =
                bool.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.AllowSkippingLevel))
                    .ConfigValue ?? "0");
            if (!allowSkipLevel && student.LevelId != classInfo.LevelId)
            {
                throw new BadRequestException(
                    "Skipping level is not allowed. Student need to be in the same level as the class!");
            }

            //Create student slots
            var classSlotIds = classInfo.Slots.Select(s => s.Id).ToList();
            var existedStudentSlots = await _unitOfWork.SlotStudentRepository.FindAsync(ss =>
                ss.StudentFirebaseId == changeClassModel.StudentFirebaseId
                && classSlotIds.Contains(ss.SlotId), false, true);
            var studentSlots = classInfo.Slots.Select(s => new SlotStudent
            {
                CreatedById = account.AccountFirebaseId,
                SlotId = s.Id,
                StudentFirebaseId = changeClassModel.StudentFirebaseId
            });

            studentSlots = studentSlots.Where(ss =>
                    !existedStudentSlots.Any(es =>
                        es.SlotId == ss.SlotId && ss.StudentFirebaseId == es.StudentFirebaseId))
                .ToList();


            foreach (var slot in existedStudentSlots)
            {
                slot.RecordStatus = RecordStatus.IsActive;
            }

            student.CurrentClassId = classInfo.Id;

            //Update old studentClass
            oldStudentClass.ClassId = changeClassModel.NewClassId;
            oldStudentClass.UpdateById = account.AccountFirebaseId;
            oldStudentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);

            //Delete old studentSlots
            var oldSlotIds = oldClassInfo.Slots.Select(s => s.Id).ToList();
            var oldStudentSlots =
                await _unitOfWork.SlotStudentRepository.FindAsync(ss => oldSlotIds.Contains(ss.SlotId));
            foreach (var oldStudentSlot in oldStudentSlots)
            {
                oldStudentSlot.RecordStatus = RecordStatus.IsDeleted;
                oldStudentSlot.DeletedAt = DateTime.UtcNow.AddHours(7);
                oldStudentSlot.DeletedById = account.AccountFirebaseId;
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
            // Notification

            // Notify the student about being added to the new class
            await _serviceFactory.NotificationService.SendNotificationAsync(
                changeClassModel.StudentFirebaseId,
                "New Class Information",
                $"Congratulations! You have been added to the new class {classInfo.Name}. Please check your updated schedule. Wishing you much success!"
            );

            // Get Firebase IDs of students and instructor in the new class
            var newClassReceiverIds = classInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
            if (classInfo.InstructorId != null)
            {
                newClassReceiverIds.Add(classInfo.InstructorId);
            }

            // Get Firebase IDs of students and instructor in the old class
            var oldClassReceiverIds = oldClassInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
            if (oldClassInfo.InstructorId != null)
            {
                oldClassReceiverIds.Add(oldClassInfo.InstructorId);
            }

            // Notify the old class that the student has left
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                oldClassReceiverIds,
                $"Student {student.FullName ?? student.UserName} has been transferred out of the class {oldClassInfo.Name}. If you have any questions or believe this is a mistake, please file a complaint or contact the support team!",
                student.AvatarUrl ?? ""
            );

            // Notify the new class about the newly added student
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                newClassReceiverIds,
                $"New student {student.FullName ?? student.UserName} has been added to the class {classInfo.Name}. Please give them your full support!",
                student.AvatarUrl ?? ""
            );
        }

        public async Task<List<StudentClassModel>> CreateStudentClass(CreateStudentClassModel createStudentClassesModel, AccountModel account)
        {
            var students = await _unitOfWork.AccountRepository.FindAsync(a =>
                createStudentClassesModel.StudentFirebaseIds.Contains(a.AccountFirebaseId));

            if (account.Role == Role.Student)
            {
                if (students.Any(s => s.AccountFirebaseId != account.AccountFirebaseId))
                {
                    throw new ForbiddenMethodException("You can only add your self to the class");
                }

                if (createStudentClassesModel.IsAutoFill)
                {
                    throw new ForbiddenMethodException("Student can not use auto fill feature");
                }
            }

            if (students.Count == 0 && !createStudentClassesModel.IsAutoFill)
            {
                throw new NotFoundException("No valid students found");
            }

            foreach (var student in students)
            {
                if (student.StudentStatus != StudentStatus.WaitingForClass)
                {
                    throw new BadRequestException(
                        $"Student {student.FullName ?? student.UserName} is already in a class");
                }
            }


            var classInfo = await _serviceFactory.ClassService.GetClassDetailById(createStudentClassesModel.ClassId);
            if (classInfo is null)
            {
                throw new NotFoundException("Class not found");
            }

            var maxStudents =
                int.Parse(
                    (await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents)).ConfigValue ??
                    "0");

            if (classInfo.StudentClasses.Count + students.Count > maxStudents)
            {
                throw new BadRequestException("Class is full!");
            }

            //if (students.Any(s => s.LevelId != classInfo.LevelId))
            //{
            //    throw new BadRequestException("Some of students is not in the same level as the class");
            //}

            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Class is finished");
            }

            var allowSkipLevel =
                bool.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.AllowSkippingLevel))
                    .ConfigValue ?? "0");
            if (!allowSkipLevel && students.Any(s => s.LevelId != classInfo.LevelId))
            {
                throw new BadRequestException(
                    "Skipping level is not allowed. All student need to be in the same level as the class!");
            }

            if (createStudentClassesModel.IsAutoFill &&
                maxStudents - classInfo.StudentClasses.Count - students.Count > 0)
            {
                var otherStudents = await _unitOfWork.AccountRepository.FindAsQueryable(s =>
                        s.StudentStatus == StudentStatus.WaitingForClass
                        && !createStudentClassesModel.StudentFirebaseIds.Contains(s.AccountFirebaseId)
                        && s.LevelId == classInfo.LevelId)
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
                        CreatedById = account.AccountFirebaseId,
                        IsPassed = false,
                    };

                    student.StudentStatus = StudentStatus.InClass;
                    student.CurrentClassId = classInfo.Id;
                    addedStudentClasses.Add(studentClass);

                    var classSlotIds = classInfo.Slots.Select(s => s.Id).ToList();
                    var existedSlots = await _unitOfWork.SlotStudentRepository.FindAsync(s =>
                        s.StudentFirebaseId == student.AccountFirebaseId
                        && classSlotIds.Contains(s.SlotId) && s.RecordStatus == RecordStatus.IsDeleted, false, true);

                    studentSlots.AddRange(classInfo.Slots.Select(s => new SlotStudent
                    {
                        CreatedById = account.AccountFirebaseId,
                        SlotId = s.Id,
                        StudentFirebaseId = student.AccountFirebaseId
                    }));

                    studentSlots = studentSlots.Where(ss =>
                            !existedSlots.Any(es =>
                                es.SlotId == ss.SlotId && ss.StudentFirebaseId == es.StudentFirebaseId))
                        .ToList();

                    existedSlotStudents.AddRange(existedSlots);
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

                    receiverIds.Add(student.AccountFirebaseId);

                    studentClass.Student = student;
                    classInfo.StudentClasses.Add(studentClass.Adapt<StudentClassModel>());
                }

                await _unitOfWork.StudentClassRepository.AddRangeAsync(addedStudentClasses);
                await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
                await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(studentClassScores);
                await _unitOfWork.AccountRepository.UpdateRangeAsync(students);
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(existedSlotStudents);
                await _unitOfWork.SaveChangesAsync();

                return addedStudentClasses;
            });

            //Create tuition
            if (classInfo.IsPublic)
            {
                classInfo.StudentClasses = [.. classInfo.StudentClasses.Where(sc => createStudentClassesModel.StudentFirebaseIds.Contains(sc.StudentFirebaseId))];
                await _serviceFactory.TuitionService.CreateTuitionWhenRegisterClass(classInfo);
            }
            
            if (classInfo.IsPublic)
            {
                var classReceiverIds = new List<string>();
                if (classInfo.InstructorId != null)
                {
                    classReceiverIds.Add(classInfo.InstructorId);
                }

                classReceiverIds = classInfo.StudentClasses.Select(sc => sc.StudentFirebaseId).ToList();

                await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                    students.Select(s => s.AccountFirebaseId).ToList(), "Thông tin lớp mới",
                    $"Chúc mừng bạn đã được thêm vào lớp mới {classInfo.Name}. Vui lòng kiểm tra lại lịch học. Chúc các bạn gặt hái được nhiều thành công!");
                await _serviceFactory.NotificationService.SendNotificationToManyAsync(classReceiverIds,
                    $"{students.Count} học sinh mới đã được thêm vào lớp {classInfo.Name}. Hãy giúp đỡ các bạn ấy hết mình!",
                    "");
            }

            return result.Adapt<List<StudentClassModel>>();
        }

        public async Task DeleteStudentClass(string studentId, Guid classId, bool isExpelled, AccountModel accountModel)
        {
            if (accountModel.Role == Role.Student && studentId != accountModel.AccountFirebaseId)
            {
                throw new ForbiddenMethodException("Student can not update other students");
            }

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
            studentClass.DeletedById = accountModel.AccountFirebaseId;
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
                studentSlot.DeletedById = accountModel.AccountFirebaseId;
            }

            //Delete tuition if any
            var tuitions = await _unitOfWork.TuitionRepository.FindAsync(t => t.StudentClassId == studentClass.Id);
            if (accountModel.Role == Role.Student && tuitions.Any(t => t.PaymentStatus == PaymentStatus.Succeed))
            {
                throw new BadRequestException("You has paid this class's tuition");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.StudentClassScoreRepository.UpdateRangeAsync(studentClassScores);
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(studentSlots);
                await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);
                await _unitOfWork.AccountRepository.UpdateAsync(student);
                await _unitOfWork.TuitionRepository.DeleteRangeAsync(tuitions);
                await _unitOfWork.SaveChangesAsync();
            });


            // Notification
            if (classInfo.IsPublic)
            {
                // Notify the removed student
                await _serviceFactory.NotificationService.SendNotificationAsync(
                    accountModel.AccountFirebaseId,
                    "Removed from class",
                    $"You have been removed from the class {classInfo.Name}. If you have any questions or believe this is a mistake, please file a complaint or contact the support team directly!"
                );

                // Notify the remaining students and instructor
                var receiverIds = classInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
                if (classInfo.InstructorId != null)
                {
                    receiverIds.Add(classInfo.InstructorId);
                }

                await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                    receiverIds,
                    $"Student {student.FullName ?? student.UserName} has been removed from the class {classInfo.Name}. If you have any questions or believe this is a mistake, please file a complaint or contact the support team directly!",
                    student.AvatarUrl ?? ""
                );
            }
        }

        //Down excel
        public async Task<byte[]> GenerateGradeTemplate(Guid classId)
        {
            var classDetails = await _serviceFactory.ClassService.GetClassDetailById(classId);
            var classCriteria = await _serviceFactory.CriteriaService.GetAllCriteriaDetails(classId);

            var sortedCriteria = classCriteria
                .OrderBy(c => DetermineCriteriaOrder(c.Name))
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Grades");
            var metadataSheet = package.Workbook.Worksheets.Add("Metadata");

            // Configure metadata sheet (hidden from users)
            metadataSheet.Hidden = eWorkSheetHidden.Hidden;
            metadataSheet.Cells[1, 1].Value = "ClassId";
            metadataSheet.Cells[1, 2].Value = classId.ToString();
            metadataSheet.Cells[2, 1].Value = "TemplateVersion";
            metadataSheet.Cells[2, 2].Value = "1.0";
            metadataSheet.Cells[3, 1].Value = "GeneratedOn";
            metadataSheet.Cells[3, 2].Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            // Store criteria IDs for later processing when importing
            for (int i = 0; i < sortedCriteria.Count; i++)
            {
                metadataSheet.Cells[4 + i, 1].Value = "CriteriaId";
                metadataSheet.Cells[4 + i, 2].Value = sortedCriteria[i].Id.ToString();
                metadataSheet.Cells[4 + i, 3].Value = sortedCriteria[i].Name;
                metadataSheet.Cells[4 + i, 4].Value = sortedCriteria[i].Weight;
            }

            // Add header information
            worksheet.Cells[1, 2, 1, 6].Merge = true;
            worksheet.Cells[1, 2].Value = "GRADE BOOK";
            worksheet.Cells[1, 2].Style.Font.Bold = true;
            worksheet.Cells[1, 2].Style.Font.Size = 16;
            worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));

            // Course and instructor information
            worksheet.Cells[2, 1].Value = "Course:";
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            var courseInfo = $"{classDetails.Name} {classDetails.Level?.Name}";
            worksheet.Cells[2, 2].Value = courseInfo;

            worksheet.Cells[3, 1].Value = "Instructor:";
            worksheet.Cells[3, 1].Style.Font.Bold = true;
            var instructorInfo = classDetails.Instructor?.FullName ?? classDetails.Instructor?.UserName;
            worksheet.Cells[3, 2].Value = instructorInfo;

            worksheet.Cells[5, 1].Value = "INSTRUCTIONS";
            worksheet.Cells[5, 1].Style.Font.Bold = true;
            worksheet.Cells[5, 1].Style.Font.Size = 14;
            worksheet.Cells[5, 1].Style.Font.Color.SetColor(Color.Blue);

            worksheet.Cells[6, 1].Value = "1. Do not modify student names";
            worksheet.Cells[7, 1].Value = "2. Enter scores between 0 and 10 only";
            worksheet.Cells[8, 1].Value = "3. Please don't leave empty cells when import";
            var instructionsRange = worksheet.Cells[6, 1, 8, 1];
            instructionsRange.Style.Font.Bold = true;
            instructionsRange.Style.Font.Italic = true;

            // Student header
            int headerRow = 10;
            worksheet.Cells[headerRow, 1].Value = "Student Name";
            worksheet.Cells[headerRow, 1].Style.Font.Bold = true;
            worksheet.Cells[headerRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[headerRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
            worksheet.Cells[headerRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells[headerRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[headerRow + 1, 1].Value = "Weight (%)";
            worksheet.Cells[headerRow + 1, 1].Style.Font.Bold = true;
            worksheet.Cells[headerRow + 1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[headerRow + 1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 239, 218));
            worksheet.Cells[headerRow + 1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells[headerRow + 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int startCol = 2;
            var assessments = sortedCriteria.Select(c => c.Name).ToArray();
            var weights = sortedCriteria.Select(c => (double)c.Weight).ToArray();
            Dictionary<string, int> headerCount = [];

            // Assessment headers
            for (int i = 0; i < assessments.Length; i++)
            {
                string assessmentName = assessments[i];

                // Handle duplicates by appending a number
                if (headerCount.ContainsKey(assessmentName))
                {
                    headerCount[assessmentName]++;
                    assessmentName = $"{assessmentName} ({headerCount[assessmentName]})";
                }
                else
                {
                    headerCount[assessmentName] = 1;
                }

                // Set column header
                var nameCell = worksheet.Cells[headerRow, startCol + i];
                nameCell.Value = assessmentName;
                nameCell.Style.Font.Bold = true;
                nameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                nameCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
                nameCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                nameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Set column width based on content
                worksheet.Column(startCol + i).Width = Math.Max(15, assessmentName.Length * 1.2);

                // Assign weight to weight row
                var weightCell = worksheet.Cells[headerRow + 1, startCol + i];
                weightCell.Value = weights[i];
                weightCell.Style.Numberformat.Format = "0.0\\%";
                weightCell.Style.Font.Bold = true;
                weightCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                weightCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 239, 218));
                weightCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                weightCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add data validation for score range (0-10)
                int totalStudents = classDetails.StudentClasses.Count;
                var dataValidation = worksheet.DataValidations.AddDecimalValidation(
                    ExcelCellBase.GetAddress(headerRow + 2, startCol + i, headerRow + 1 + totalStudents, startCol + i));
                dataValidation.ShowErrorMessage = true;
                dataValidation.ErrorTitle = "Invalid Score";
                dataValidation.Error = "Please enter a score between 0 and 10";
                dataValidation.Operator = ExcelDataValidationOperator.between;
                dataValidation.Formula.Value = 0;
                dataValidation.Formula2.Value = 10;
            }

            // No need for total and percentage columns
            int studentStartRow = headerRow + 2;
            foreach (var studentClass in classDetails.StudentClasses)
            {
                worksheet.Cells[studentStartRow, 1].Value = studentClass.Student.FullName;
                worksheet.Cells[studentStartRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[studentStartRow, 1].Style.Locked = true;
                studentStartRow++;
            }

            // Add conditional formatting for scores
            // Red for failing scores (< 5)
            var failingScores = worksheet.ConditionalFormatting.AddLessThan(
                new ExcelAddress(headerRow + 2, startCol, studentStartRow - 1, startCol + assessments.Length - 1));
            failingScores.Formula = "5";
            failingScores.Style.Fill.PatternType = ExcelFillStyle.Solid;
            failingScores.Style.Fill.BackgroundColor.SetColor(Color.LightPink);

            // Green for passing scores (>= 5)
            var passingScores = worksheet.ConditionalFormatting.AddGreaterThanOrEqual(
                new ExcelAddress(headerRow + 2, startCol, studentStartRow - 1, startCol + assessments.Length - 1));
            passingScores.Formula = "5";
            passingScores.Style.Fill.PatternType = ExcelFillStyle.Solid;
            passingScores.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

            // No conditional formatting for totals as the total column has been removed

            // Protect worksheet
            worksheet.Protection.IsProtected = true;
            worksheet.Protection.AllowSelectLockedCells = false;
            worksheet.Protection.AllowSelectUnlockedCells = true;
            worksheet.Protection.AllowDeleteColumns = false;
            worksheet.Protection.AllowDeleteRows = false;
            worksheet.Protection.AllowInsertColumns = false;
            worksheet.Protection.AllowInsertRows = false;
            worksheet.Protection.AllowFormatCells = false;
            worksheet.Cells[worksheet.Dimension.Address].Style.Locked = true;
            // Allow editing only the grade cells
            var gradeRange = worksheet.Cells[headerRow + 2, startCol, studentStartRow - 1,
                startCol + assessments.Length - 1];
            gradeRange.Style.Locked = false;
            // Auto-fit and freeze panes
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(headerRow + 2, 2);

            return await package.GetAsByteArrayAsync();
        }

        public async Task<bool> ImportScores(Guid classId, Stream excelFileStream, AccountModel account)
        {
            if (classId == Guid.Empty)
            {
                throw new ArgumentException("Invalid class ID", nameof(classId));
            }

            var classDetails = await _serviceFactory.ClassService.GetClassDetailById(classId);

            if (classDetails.IsPublic == false)
            {
                throw new BadRequestException("The class is not open for scores");
            }

            if (classDetails.Status == ClassStatus.NotStarted)
            {
                throw new BadRequestException("The class is not started");
            }

            if (classDetails.IsScorePublished)
            {
                throw new BadRequestException("The scores have been published");
            }

            var classCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.Class);

            if (classCriteria.Count == 0)
            {
                throw new BadRequestException("Assessment criteria not found for this class");
            }

            using var package = new ExcelPackage(excelFileStream);
            var worksheet = package.Workbook.Worksheets["Grades"];
            if (worksheet == null)
            {
                throw new BadRequestException("Invalid Excel template: 'Grades' worksheet not found");
            }

            var metadataSheet = package.Workbook.Worksheets["Metadata"];
            if (metadataSheet == null || metadataSheet.Cells[1, 2].Text != classId.ToString())
            {
                throw new BadRequestException("Invalid template: This template is not for the selected class");
            }

            int headerRow = 10;
            var criteriaMapping = MapCriteriaToColumns(worksheet, classCriteria, headerRow);

            //Check valid template
            ValidateExcelTemplate(worksheet, criteriaMapping.Count, headerRow);
            
            int studentStartRow = headerRow + 2;
            int rows = worksheet.Dimension.Rows;
            List<string> allValidationErrors = [];

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                for (int row = studentStartRow; row <= rows; row++)
                {
                    string studentName = worksheet.Cells[row, 1].Text;
                    if (string.IsNullOrEmpty(studentName))
                        continue;

                    var studentClass = classDetails.StudentClasses.FirstOrDefault(sc =>
                        string.Equals(sc.Student.FullName, studentName, StringComparison.OrdinalIgnoreCase));

                    if (studentClass == null)
                        continue;

                    try
                    {
                        await UpdateStudentClassScores(studentClass.Id, worksheet, row, criteriaMapping,
                            account.AccountFirebaseId);
                    }
                    catch (BadRequestException ex)
                    {
                        allValidationErrors.Add(ex.Message);
                    }
                }

                if (allValidationErrors.Any())
                {
                    throw new BadRequestException(
                        $"Score import failed due to validation errors:\n{string.Join("\n", allValidationErrors)}");
                }

                await _unitOfWork.SaveChangesAsync();

                for (int row = studentStartRow; row <= rows; row++)
                {
                    string studentName = worksheet.Cells[row, 1].Text;
                    if (string.IsNullOrEmpty(studentName))
                        continue;

                    var studentClass = classDetails.StudentClasses.FirstOrDefault(sc =>
                        string.Equals(sc.Student.FullName, studentName, StringComparison.OrdinalIgnoreCase));

                    if (studentClass == null)
                        continue;

                    // Now calculate and update GPA based on the saved scores
                    await UpdateStudentClassGpa(studentClass.Id,
                        account.AccountFirebaseId,
                        classDetails.Name);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            });
        }

        private void ValidateExcelTemplate(ExcelWorksheet worksheet, int expectedCriteriaCount, int headerRow)
        {
            if (worksheet == null)
            {
                throw new BadRequestException("Invalid Excel template: No worksheet found");
            }

            if (worksheet.Dimension.Columns < expectedCriteriaCount + 1)
            {
                throw new BadRequestException("Invalid Excel template: Missing required columns");
            }

            string studentHeader = worksheet.Cells[headerRow, 1].Text;
            if (string.IsNullOrEmpty(studentHeader) ||
                !studentHeader.Equals("Student Name", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException($"Invalid Excel template: Student column not found in row {headerRow}");
            }
    
            // Verify weights row exists
            string weightLabel = worksheet.Cells[headerRow + 1, 1].Text;
            if (string.IsNullOrEmpty(weightLabel) || 
                !weightLabel.Contains("Weight", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Invalid Excel template: Weight row not found");
            }
        }

        private Dictionary<string, (int Column, Guid Id)> MapCriteriaToColumns(
            ExcelWorksheet worksheet,
            IEnumerable<Criteria> classCriteria,
            int headerRow)
        {
            int startCol = 2;
            var mapping = new Dictionary<string, (int Column, Guid Id)>(StringComparer.OrdinalIgnoreCase);

            // First, check metadata sheet for criteria IDs if available
            var metadataSheet = worksheet.Workbook.Worksheets["Metadata"];
            Dictionary<string, Guid> criteriaIdByName = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            if (metadataSheet != null)
            {
                // Try to read criteria IDs from metadata
                int row = 4; // Criteria IDs start from row 4 in our new template
                while (!string.IsNullOrEmpty(metadataSheet.Cells[row, 1].Text))
                {
                    if (metadataSheet.Cells[row, 1].Text == "CriteriaId" &&
                        !string.IsNullOrEmpty(metadataSheet.Cells[row, 2].Text) &&
                        !string.IsNullOrEmpty(metadataSheet.Cells[row, 3].Text))
                    {
                        string criteriaName = metadataSheet.Cells[row, 3].Text;
                        if (Guid.TryParse(metadataSheet.Cells[row, 2].Text, out Guid criteriaId))
                        {
                            criteriaIdByName[criteriaName] = criteriaId;
                        }
                    }

                    row++;
                }
            }

            // Map columns to criteria
            for (int col = startCol; col <= worksheet.Dimension.Columns; col++)
            {
                string criteriaName = worksheet.Cells[headerRow, col].Text;
                if (!string.IsNullOrEmpty(criteriaName))
                {
                    // Extract the base criteria name (remove any numbering like " (1)")
                    string baseCriteriaName = Regex.Replace(criteriaName, @"\s*\(\d+\)$", "");

                    // First try to get the ID directly from metadata
                    if (criteriaIdByName.TryGetValue(baseCriteriaName, out Guid criteriaId))
                    {
                        mapping[criteriaName] = (col, criteriaId);
                        continue;
                    }

                    // If not found in metadata, look up by name
                    var matchingCriteria = classCriteria.FirstOrDefault(c =>
                        string.Equals(c.Name, baseCriteriaName, StringComparison.OrdinalIgnoreCase));

                    if (matchingCriteria != null)
                    {
                        mapping[criteriaName] = (col, matchingCriteria.Id);
                    }
                }
            }

            return mapping;
        }

        private async Task UpdateStudentClassScores(Guid studentClassId, ExcelWorksheet worksheet, int row,
            Dictionary<string, (int Column, Guid Id)> criteriaMapping, string accountFirebaseId)
        {
            var existingScores =
                await _unitOfWork.StudentClassScoreRepository.FindAsync(scs => scs.StudentClassId == studentClassId);

            List<StudentClassScore> scoresToUpdate = [];
            List<StudentClassScore> scoresToAdd = [];
            List<string> validationErrors = [];

            string studentName = worksheet.Cells[row, 1].Text;

            foreach (var criteriaEntry in criteriaMapping)
            {
                string criteriaName = criteriaEntry.Key;
                int col = criteriaEntry.Value.Column;
                Guid criteriaId = criteriaEntry.Value.Id;

                decimal? score = null;
                string cellValue = worksheet.Cells[row, col].Text;
                if (decimal.TryParse(worksheet.Cells[row, col].Text, out decimal parsedScore))
                {
                    if (parsedScore < 0 || parsedScore > 10)
                    {
                        validationErrors.Add(
                            $"Student '{studentName}', criterion '{criteriaName}': Score {parsedScore} is outside valid range (0-10).");
                        continue;
                    }

                    score = parsedScore;
                }
                else if (string.IsNullOrWhiteSpace(cellValue))
                {
                    validationErrors.Add(
                        $"Student '{studentName}', criterion '{criteriaName}': Score cannot be empty. Please enter a numeric value between 0-10.");
                    continue;
                }
                else
                {
                    validationErrors.Add(
                        $"Student '{studentName}', criterion '{criteriaName}': Value '{cellValue}' is not a valid number.");
                    continue;
                }

                var scoreRecord = existingScores.FirstOrDefault(s => s.CriteriaId == criteriaId);

                if (scoreRecord != null)
                {
                    scoreRecord.Score = score;
                    scoreRecord.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    scoresToUpdate.Add(scoreRecord);
                }
                else
                {
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

            if (validationErrors.Any())
            {
                throw new BadRequestException($"Invalid scores detected:\n{string.Join("\n", validationErrors)}");
            }

            if (scoresToUpdate.Any())
            {
                await _unitOfWork.StudentClassScoreRepository.UpdateRangeAsync(scoresToUpdate);
            }

            if (scoresToAdd.Any())
            {
                await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(scoresToAdd);
            }
        }

        private async Task UpdateStudentClassGpa(Guid studentClassId, string accountFirebaseId, string className)
        {
            var studentClassScores =
                await _unitOfWork.StudentClassScoreRepository.FindAsync(scs => scs.StudentClassId == studentClassId
                );
            if (!studentClassScores.Any())
                return;
            decimal totalWeightedScore = 0;
            decimal totalWeight = 0;

            foreach (var score in studentClassScores)
            {
                var criteria = await _unitOfWork.CriteriaRepository.GetByIdAsync(score.CriteriaId);

                if (score.Score.HasValue && criteria != null)
                {
                    totalWeightedScore += score.Score.Value * criteria.Weight;
                    totalWeight += criteria.Weight;
                }
            }

            decimal gpa = totalWeight > 0
                ? Math.Round(totalWeightedScore / totalWeight, 2)
                : 0;

            var studentClass = await _unitOfWork.StudentClassRepository.GetByIdAsync(studentClassId);
            if (studentClass != null)
            {
                studentClass.GPA = gpa;
                studentClass.UpdateById = accountFirebaseId;
                studentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);
                await _serviceFactory.NotificationService.SendNotificationAsync(
                    studentClass.StudentFirebaseId,
                    "Grade Update",
                    $"Your grades for class {className} have been updated. Your total grade: {gpa}"
                );
            }
        }

        private int DetermineCriteriaOrder(string criteriaName)
        {
            var categoryPriorities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Test", 100 },
                { "Assignment", 200 },
                { "Workshop", 300 },
                { "Training", 400 },
                { "Performance", 500 },
                { "Project", 600 },
                { "Coordination", 700 },
                { "Memorization", 800 }
            };

            // Subcategories for more detailed sorting within main categories
            var subcategoryPriorities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                // Technique types
                { "Tone", 10 },
                { "Rhythmic", 20 },
                { "Articulation", 30 },
                { "Expression", 40 },
                { "Arpeggios", 50 },
                { "Hand", 60 },
                { "Pedal", 70 },
                { "Duet", 80 }
            };

            // Find main category
            int baseOrder = 1000; // Default
            foreach (var category in categoryPriorities)
            {
                if (criteriaName.IndexOf(category.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    baseOrder = category.Value;
                    break;
                }
            }

            // Find subcategory modifier
            int subOrder = 0;
            foreach (var subcategory in subcategoryPriorities)
            {
                if (criteriaName.IndexOf(subcategory.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    subOrder = subcategory.Value;
                    break;
                }
            }

            // Combine for final sort order
            return baseOrder + subOrder;
        }

        public async Task<bool> UpdateStudentStatusAsync(string studentFirbaseId, StudentStatus newStatus,
            AccountModel account, Guid? classId = null)
        {
            var student =
                await _unitOfWork.AccountRepository.FindFirstAsync(a => a.AccountFirebaseId == studentFirbaseId);

            if (student == null || student.Role != Role.Student)
            {
                throw new ArgumentException("Invalid student ID or account is not a student");
            }

            var currentStatus = student.StudentStatus ?? StudentStatus.Unregistered;

            if (!IsValidStatusTransition(currentStatus, newStatus))
            {
                throw new InvalidOperationException(
                    $"Invalid status transition from {currentStatus} to {newStatus}");
            }

            student.StudentStatus = newStatus;

            if (newStatus == StudentStatus.InClass && classId.HasValue)
            {
                student.CurrentClassId = classId.Value;
            }
            else if (newStatus == StudentStatus.DropOut || newStatus == StudentStatus.Leave)
            {
                student.CurrentClassId = null;
            }

            await _unitOfWork.AccountRepository.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        /// Validates if the status transition is allowed according to the state diagram
        public bool IsValidStatusTransition(StudentStatus fromStatus, StudentStatus toStatus)
        {
            return (fromStatus, toStatus) switch
            {
                (StudentStatus.Unregistered, StudentStatus.AttemptingEntranceTest) => true,
                (StudentStatus.AttemptingEntranceTest, StudentStatus.WaitingForClass) => true,
                (StudentStatus.AttemptingEntranceTest, StudentStatus.DropOut) => true,
                (StudentStatus.WaitingForClass, StudentStatus.InClass) => true,
                (StudentStatus.WaitingForClass, StudentStatus.Leave) => true,
                (StudentStatus.InClass, StudentStatus.DropOut) => true,
                (StudentStatus.InClass, StudentStatus.Leave) => true,
                (StudentStatus.InClass, StudentStatus.WaitingForClass) => true, // End of class, waiting for next
                (StudentStatus.Leave, StudentStatus.AttemptingEntranceTest) => true, // Rejoin by taking entrance test
                (StudentStatus.DropOut, StudentStatus
                    .AttemptingEntranceTest) => true, // Rejoin by retaking entrance test
                (StudentStatus.Leave, StudentStatus.WaitingForClass) => true, // Rejoin directly to waiting

                _ => false
            };
        }

        //Update a specific score for a specific criteria
        public async Task<bool> UpdateStudentScore(UpdateStudentScoreModel model, AccountModel account)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var studentClass = await _unitOfWork.StudentClassRepository.GetByIdAsync(model.StudentClassId);
            if (studentClass == null)
            {
                throw new NotFoundException($"Student class with ID {model.StudentClassId} not found");
            }

            var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(studentClass.ClassId);
            if (classInfo == null)
            {
                throw new NotFoundException($"Class with ID {studentClass.ClassId} not found");
            }

            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Cannot update scores for a finished class");
            }

            var criteria = await _unitOfWork.CriteriaRepository.GetByIdAsync(model.CriteriaId);
            if (criteria == null)
            {
                throw new NotFoundException($"Criteria with ID {model.CriteriaId} not found");
            }

            var score = await _unitOfWork.StudentClassScoreRepository.FindSingleAsync(sc =>
                sc.StudentClassId == studentClass.ClassId && sc.CriteriaId == model.CriteriaId);
            if (score == null)
            {
                // Create new score
                score = new StudentClassScore
                {
                    Id = Guid.NewGuid(),
                    StudentClassId = model.StudentClassId,
                    CriteriaId = model.CriteriaId,
                    Score = model.Score
                };
                await _unitOfWork.StudentClassScoreRepository.AddAsync(score);
            }
            else
            {
                // Update existing score
                score.Score = model.Score;
                score.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await _unitOfWork.StudentClassScoreRepository.UpdateAsync(score);
            }

            await UpdateStudentClassGpa(model.StudentClassId, account.AccountFirebaseId, classInfo.Name);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        //Update batch
        public async Task<bool> UpdateBatchStudentClassScores(UpdateBatchStudentClassScoreModel model,
            AccountModel account)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Scores == null || !model.Scores.Any())
            {
                throw new BadRequestException("No scores provided for update");
            }

            var studentClassIds = model.Scores.Select(s => s.StudentClassId).Distinct().ToList();
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc =>
                studentClassIds.Contains(sc.Id) && sc.ClassId == model.ClassId);
            if (studentClasses.Count != studentClassIds.Count)
            {
                throw new BadRequestException(
                    "Some student classes were not found or don't belong to the specified class");
            }

            // Get class info
            var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(model.ClassId);
            if (classInfo == null)
            {
                throw new NotFoundException($"Class with ID {model.ClassId} not found");
            }

            if (classInfo.Status == ClassStatus.NotStarted)
            {
                throw new BadRequestException("Cannot update scores for a class that has not started");
            }

            if (classInfo.IsScorePublished == true)
            {
                throw new BadRequestException("Cannot update scores for a class that has already been published");
            }

            // Get all criteria for validation
            var criteriaIds = model.Scores.Select(s => s.CriteriaId).Distinct().ToList();
            var criteria = await _unitOfWork.CriteriaRepository.FindAsync(c => criteriaIds.Contains(c.Id));

            if (criteria.Count != criteriaIds.Count)
            {
                throw new BadRequestException("Some criteria were not found");
            }

            // Load all existing scores in one query for better performance
            var existingScores = await _unitOfWork.StudentClassScoreRepository.FindAsync(scs =>
                studentClassIds.Contains(scs.StudentClassId) && criteriaIds.Contains(scs.CriteriaId));

            // Create a dictionary for quick lookup
            var scoreMap = existingScores.ToDictionary(
                s => (s.StudentClassId, s.CriteriaId),
                s => s);

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var now = DateTime.UtcNow.AddHours(7);
                    var newScores = new List<StudentClassScore>();
                    var updatedScores = new List<StudentClassScore>();
                    foreach (var scoreUpdate in model.Scores)
                    {
                        if (scoreMap.TryGetValue((scoreUpdate.StudentClassId, scoreUpdate.CriteriaId),
                                out var existingScore))
                        {
                            // Update existing score
                            existingScore.Score = scoreUpdate.Score;
                            existingScore.UpdatedAt = now;
                            updatedScores.Add(existingScore);
                        }
                        else
                        {
                            var newScore = new StudentClassScore
                            {
                                Id = Guid.NewGuid(),
                                StudentClassId = scoreUpdate.StudentClassId,
                                CriteriaId = scoreUpdate.CriteriaId,
                                Score = scoreUpdate.Score
                            };
                            newScores.Add(newScore);
                        }
                    }

                    if (newScores.Any())
                    {
                        await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(newScores);
                    }

                    if (updatedScores.Any())
                    {
                        await _unitOfWork.StudentClassScoreRepository.UpdateRangeAsync(updatedScores);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Then update GPAs for all affected students
                    foreach (var studentClassId in studentClassIds)
                    {
                        await UpdateStudentClassGpa(studentClassId, account.AccountFirebaseId, classInfo.Name);
                    }

                    // Finally save the GPA updates
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            );
        }

        public async Task<bool> UpdateAttendancePercentageStudentClassStatus(Guid classId,
            string staffAccountFirebaseId)
        {
            var studentClasses = await _unitOfWork.StudentClassRepository
                .FindAsync(sc => sc.ClassId == classId);

            foreach (var studentClass in studentClasses)
            {
                var slotStudents = await _unitOfWork.SlotStudentRepository
                    .FindAsync(ss => ss.StudentFirebaseId == studentClass.StudentFirebaseId);

                var totalSlots = slotStudents.Count;
                var attendedSlots = slotStudents.Count(ss => ss.AttendanceStatus == AttendanceStatus.Attended);

                studentClass.AttendancePercentage = (decimal)attendedSlots / totalSlots * 100;
                studentClass.UpdateById = staffAccountFirebaseId;
                studentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}