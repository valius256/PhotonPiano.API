using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Extensions;

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

    public async Task UpdateAttentStudent(UpdateAttentdanceModel model, string teacherId)
    {
        var slotEntity = await _serviceFactory.SlotService.GetSLotDetailById(model.SlotId);

        if (slotEntity.Class.InstructorId != teacherId)
            throw new IllegalArgumentException("You are not allowed to update attendance for this slot.");


        if (slotEntity == null) throw new IllegalArgumentException("The specified slot does not exist.");

        var shiftStartTime = _serviceFactory.SlotService.GetShiftStartTime(slotEntity.Shift);


        var slotDateTime = slotEntity.Date.ToDateTime(shiftStartTime);
        var currentDateTime = DateTime.UtcNow.AddHours(7);

        // Todo: allow for future attendance or not? 
        // Validate if the slot has passed 24 hours
        if ((currentDateTime - slotDateTime).TotalHours > 24)
            throw new BadRequestException("Cannot update attendance for a slot that has already passed 24 hours.");

        foreach (var studentId in model.StudentAttentIds)
        {
            var slotStudent = await _unitOfWork.SlotStudentRepository
                .FindFirstAsync(x => x.SlotId == model.SlotId && x.StudentFirebaseId == studentId);

            if (slotStudent != null)
            {
                slotStudent.AttendanceStatus = AttendanceStatus.Attended;
                slotStudent.UpdateById = teacherId;
                slotStudent.UpdatedAt = DateTime.UtcNow.ToVietnamTime();
                await _unitOfWork.SlotStudentRepository.UpdateAsync(slotStudent);
            }
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
                    slotStudent.UpdatedAt = DateTime.UtcNow.ToVietnamTime();
                    await _unitOfWork.SlotStudentRepository.UpdateAsync(slotStudent);
                }
            }
    }
}