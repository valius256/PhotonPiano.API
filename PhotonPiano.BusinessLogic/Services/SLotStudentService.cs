using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SlotStudentService : ISlotStudentService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public SlotStudentService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
    {
        _serviceFactory = serviceFactory;
        _unitOfWork = unitOfWork;
    }


    public async Task<bool> UpdateAttentStudent(UpdateAttentdanceModel model, string teacherId)
    {
        var slotEntity = await _serviceFactory.SlotService.GetSLotDetailById(model.SlotId);

        if (slotEntity.Class.InstructorId != teacherId)
            throw new IllegalArgumentException("You are not allowed to update attendance for this slot.");
        
        if(model.StudentAttentIds.Count == 0 && model.StudentAbsentIds != null && model.StudentAbsentIds.Count == 0)
            throw new IllegalArgumentException("Student list sending cannot be empty.");
        
        if (slotEntity == null) throw new IllegalArgumentException("The specified slot does not exist.");

        var shiftStartTime = _serviceFactory.SlotService.GetShiftStartTime(slotEntity.Shift);

        var teacherName = await _serviceFactory.AccountService.GetAccountById(teacherId);

        var slotDateTime = slotEntity.Date.ToDateTime(shiftStartTime);
        var currentDateTime = DateTime.UtcNow.AddHours(7);

        // Validate if the slot has passed 24 hours
        // if ((currentDateTime - slotDateTime).TotalHours > 24)
        //     throw new BadRequestException("Cannot update attendance for a slot that has already passed 24 hours.");

        foreach (var studentId in model.StudentAttentIds)
        {
            var slotStudent = await _unitOfWork.SlotStudentRepository
                .FindFirstProjectedAsync<SlotStudent>(x =>
                    x.SlotId == model.SlotId && x.StudentFirebaseId == studentId);

            if (slotStudent is null)
            {
                throw new IllegalArgumentException("The specified student does not exist in this slot.");
            }
            
      
                slotStudent.AttendanceStatus = AttendanceStatus.Attended;
                slotStudent.UpdateById = teacherId;
                slotStudent.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await _unitOfWork.SlotStudentRepository.UpdateAsync(slotStudent);

                await _serviceFactory.NotificationService.SendNotificationAsync(studentId,
                    $"Đã điểm danh cho lớp {slotEntity.Class.Name}", $"trạng thái: {AttendanceStatus.Attended}");
        }

        if (model.StudentAbsentIds is not null)
            foreach (var studentId in model.StudentAbsentIds)
            {
                if (model.StudentAttentIds.Contains(studentId))
                    throw new BadRequestException("Student cannot be both present and absent at the same time.");

                var slotStudent = await _unitOfWork.SlotStudentRepository
                    .FindFirstAsync(x => x.SlotId == model.SlotId && x.StudentFirebaseId == studentId);

                if (slotStudent != null)
                {
                    slotStudent.AttendanceStatus = AttendanceStatus.Absent;
                    slotStudent.UpdateById = teacherId;
                    slotStudent.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await _unitOfWork.SlotStudentRepository.UpdateAsync(slotStudent);

                    await _serviceFactory.NotificationService.SendNotificationAsync(studentId,
                        $"Đã điểm danh cho lớp {slotEntity.Class.Name}", $"trạng thái: {AttendanceStatus.Absent}");
                }
            }

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}