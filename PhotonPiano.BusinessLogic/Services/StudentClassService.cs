

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

        public async Task ChangeClassOfStudent(CreateStudentClassModel changeClassModel, string accountFirebaseId)
        {
            var oldStudentClass = await _unitOfWork.StudentClassRepository.FindSingleAsync(sc => sc.StudentFirebaseId == changeClassModel.StudentFirebaseId && sc.ClassId == changeClassModel.ClassId);
            if (oldStudentClass is null)
            {
                throw new NotFoundException("Student class not found");
            }
            var oldClassInfo = (await _unitOfWork.ClassRepository.Entities.Include(oc => oc.StudentClasses)
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
                .FirstOrDefaultAsync(c => c.Id == changeClassModel.ClassId);
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


            var studentClass = changeClassModel.Adapt<StudentClass>();
            studentClass.CreatedById = accountFirebaseId;
            studentClass.IsPassed = false;
            student.CurrentClassId = classInfo.Id;

            //Delete old studentClass
            oldStudentClass.RecordStatus = RecordStatus.IsDeleted;
            oldStudentClass.DeletedById = accountFirebaseId;
            oldStudentClass.DeletedAt = DateTime.UtcNow.AddHours(7);

            //Delete old studentSlots
            var oldSlotIds = classInfo.Slots.Select(s => s.Id).ToList();
            var oldStudentSlots = await _unitOfWork.SlotStudentRepository.FindAsync(ss => oldSlotIds.Contains(ss.SlotId));
            foreach (var oldStudentSlot in oldStudentSlots)
            {
                oldStudentSlot.RecordStatus = RecordStatus.IsDeleted;
                oldStudentSlot.DeletedAt = DateTime.UtcNow.AddHours(7);
                oldStudentSlot.DeletedById = accountFirebaseId;
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var addedStudentClass = await _unitOfWork.StudentClassRepository.AddAsync(studentClass);

                //Change student class score to preserve to score
                var studentClassScores = await _unitOfWork.StudentClassScoreRepository.FindAsync(scs => scs.StudentClassId == oldStudentClass.Id);
                foreach (var studentClassScore in studentClassScores)
                {
                    studentClassScore.StudentClassId = studentClass.Id;
                }
                await _unitOfWork.StudentClassScoreRepository.UpdateRangeAsync(studentClassScores);
                
                await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
                await _unitOfWork.AccountRepository.UpdateAsync(student);
                //Delete
                await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(oldStudentSlots);
                await _unitOfWork.StudentClassRepository.UpdateAsync(oldStudentClass);
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

        public async Task<StudentClassModel> CreateStudentClass(CreateStudentClassModel createStudentClassModel, string accountFirebaseId)
        {
            var student = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == createStudentClassModel.StudentFirebaseId);
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
                await _serviceFactory.NotificationService.SendNotificationAsync(createStudentClassModel.StudentFirebaseId, "Thông tin lớp mới",
                    $"Chúc mừng bạn đã được thêm vào lớp mới {classInfo.Name}. Vui lòng kiểm tra lại lịch học. Chúc các bạn gặt hái được nhiều thành công!");
                
                var receiverIds = classInfo.StudentClasses.Select(c => c.StudentFirebaseId).ToList();
                if (classInfo.InstructorId != null)
                {
                    receiverIds.Add(classInfo.InstructorId);
                }
                await _serviceFactory.NotificationService.SendNotificationToManyAsync(receiverIds, 
                    $"Học sinh mới {student.FullName ?? student.UserName} được thêm vào lớp {classInfo.Name}. Hãy giúp đỡ bạn ấy hết mình!",student.AvatarUrl ?? "");
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
