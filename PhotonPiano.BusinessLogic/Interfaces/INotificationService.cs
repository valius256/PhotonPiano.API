using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Notification;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface INotificationService
{
    Task<PagedResult<NotificationDetailsModel>> GetPagedNotifications(QueryPagedNotificationsModel queryModel,
        AccountModel currentAccount);

    Task ToggleBatchViewStatus(AccountModel currentAccount, params List<Guid> notificationIds);

    Task SendNotificationAsync(string userFirebaseId, string title, string message, string thumbnail = "");
    Task<List<AccountNotification>> GetUserNotificationsAsync(string userId);
    Task ToggleNotificationViewStatus(Guid id, string accountId);
    Task SendNotificationsToAllStaffsAsync(string title, string message, bool requiresSavingChanges = true);

    Task SendNotificationToManyAsync(List<string> userFirebaseIds, string message, string thumbnail,
        bool requiresSavingChanges = true);

    Task CronAutoRemovedOutDateNotifications();
}