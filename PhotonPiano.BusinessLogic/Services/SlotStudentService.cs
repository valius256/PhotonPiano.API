using Newtonsoft.Json;
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
        var slotEntity = await _serviceFactory.SlotService.GetSlotDetailById(model.SlotId);

        if (slotEntity.Class.InstructorId != teacherId)
            throw new IllegalArgumentException("You are not allowed to update attendance for this slot.");

        if (model.SlotStudentInfoRequests.Count == 0 || model.SlotStudentInfoRequests == null)
            throw new IllegalArgumentException("Student list sending cannot be empty.");

        if (slotEntity == null) throw new NotFoundException("The specified slot does not exist.");

        var shiftStartTime = _serviceFactory.SlotService.GetShiftStartTime(slotEntity.Shift);

        var teacherAccount = await _serviceFactory.AccountService.GetAccountById(teacherId);

        var slotDateTime = slotEntity.Date.ToDateTime(shiftStartTime);
        var currentDateTime = DateTime.UtcNow.AddHours(7);

        // Validate if the slot has passed 24 hours
        // if ((currentDateTime - slotDateTime).TotalHours > 24)
        //     throw new BadRequestException("Cannot update attendance for a slot that has already passed 24 hours.");


        foreach (var studentModel in model.SlotStudentInfoRequests)
        {
            var slotStudent = await _unitOfWork.SlotStudentRepository
                .FindFirstProjectedAsync<SlotStudent>(x =>
                    x.SlotId == model.SlotId && x.StudentFirebaseId == studentModel.StudentId);

            if (slotStudent is null)
            {
                throw new IllegalArgumentException("The specified student does not exist in this slot.");
            }

            // update properties
            slotStudent.AttendanceStatus = studentModel.AttendanceStatus;
            slotStudent.UpdateById = teacherId;
            slotStudent.GestureComment = studentModel.GestureComment;
            slotStudent.AttendanceComment = studentModel.AttendanceComment;
            slotStudent.PedalComment = studentModel.PedalComment;
            slotStudent.FingerNoteComment = studentModel.FingerNoteComment;
            if (studentModel.GestureUrls != null)
            {
                slotStudent.GestureUrl = JsonConvert.SerializeObject(studentModel.GestureUrls);
            }
            slotStudent.UpdatedAt = DateTime.UtcNow.AddHours(7);

            await _unitOfWork.SlotStudentRepository.UpdateAsync(slotStudent);

            await _serviceFactory.NotificationService.SendNotificationAsync(studentModel.StudentId,
                $"Bạn {slotStudent.StudentAccount.FullName ?? slotStudent.StudentAccount.UserName} đã {ConvertAttendanceStatusToVietnamese(studentModel.AttendanceStatus)} lớp {slotEntity.Class.Name} ngày {DateTime.UtcNow.AddHours(7)}",
                "");
        }

        await _unitOfWork.SaveChangesAsync();
        
        // removed cache 
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("schedule");

        return true;
    }

    private string ConvertAttendanceStatusToVietnamese(AttendanceStatus status)
    {
        return status switch
        {
            AttendanceStatus.Attended => "đã có mặt",
            AttendanceStatus.Absent => "vắng mặt",
            _ => "chưa tham gia"
        };
    }
}