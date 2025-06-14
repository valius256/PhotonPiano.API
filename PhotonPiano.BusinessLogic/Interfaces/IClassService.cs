﻿using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.DataAccess.Models.Paging;
using Role = PhotonPiano.DataAccess.Models.Enum.Role;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IClassService
{
    Task<PagedResult<ClassModel>> GetPagedClasses(QueryClassModel queryClass, string? currentAccountId);

    Task<ClassDetailModel> GetClassDetailById(Guid id);

    Task<ClassScoreboardModel> GetClassScoreboard(Guid id);

    Task<List<ClassModel>> GetClassByUserFirebaseId(string userFirebaseId, Role role);

    Task<List<ClassModel>> AutoArrangeClasses(ArrangeClassModel arrangeClassModel, string userId);

    Task UpdateClassStartTime(Guid classId);

    Task<ClassModel> CreateClass(CreateClassModel model, string accountFirebaseId);

    Task UpdateClass(UpdateClassModel model, string accountFirebaseId);

    Task DeleteClass(Guid classId, string accountFirebaseId);

    Task PublishClass(Guid classId, string accountFirebaseId);

    Task ScheduleClass(ScheduleClassModel scheduleClassModel, string accountFirebaseId);

    Task ClearClassSchedule(Guid classId, string accountFirebaseId);

    Task<PagedResult<AccountSimpleModel>> GetAvailableTeacher(GetAvailableTeacherForClassModel model);

    Task ShiftClassSchedule(ShiftClassScheduleModel shiftClassScheduleModel, string accountFirebaseId);
    Task MergeClass(MergeClassModel shiftClassScheduleModel, string accountFirebaseId);
    Task<List<ClassWithSlotsModel>> GetMergableClass(Guid classId);
}