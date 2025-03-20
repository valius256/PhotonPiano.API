

using Mapster;
using Microsoft.EntityFrameworkCore;
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
            if (classInfo.LevelId != student.LevelId)
            {
                throw new BadRequestException("Student is not in the same level as the class");
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
            if (students.Any(s => s.LevelId != classInfo.LevelId))
            {
                throw new BadRequestException("Some of students is not in the same level as the class");
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
                    $"Học viên {student.FullName ?? student.UserName} đã bị xóa khỏi lớp {classInfo.Name}. Nếu có thắc mắc hoặc cho rằng đây là sự nhầm lẫn, vui lòng gửi đơn khiếu nại hoặc liên hệ trực tiếp bộ phận hỗ trợ!", student.AvatarUrl ?? "");
            }
        }
    }
}
