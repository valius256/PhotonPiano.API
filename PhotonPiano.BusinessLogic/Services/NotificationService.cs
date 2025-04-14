using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Notification;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class NotificationService : INotificationService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
    {
        _serviceFactory = serviceFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<NotificationDetailsModel>> GetPagedNotifications(
        QueryPagedNotificationsModel queryModel,
        AccountModel currentAccount)
    {
        var (page, size, column, desc, isViewed) = queryModel;

        var pagedResult = await _unitOfWork.NotificationRepository
            .GetPaginatedWithProjectionAsync<NotificationDetailsModel>(
                page, size, column, desc,
                expressions:
                [
                    n => n.AccountNotifications.Any(an => an.AccountFirebaseId == currentAccount.AccountFirebaseId),
                    n => !isViewed.HasValue || n.AccountNotifications.Any(an => an.IsViewed == isViewed.Value)
                ]);

        return pagedResult;
    }

    public async Task ToggleBatchViewStatus(AccountModel currentAccount, params List<Guid> notificationIds)
    {
        var notifications = await _unitOfWork.NotificationRepository.FindAsync(n => notificationIds.Contains(n.Id));

        if (notifications.Count != notificationIds.Count)
        {
            throw new NotFoundException("Some notifications are Not Found");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.AccountNotificationRepository.ExecuteUpdateAsync(
                expression: an => notificationIds.Contains(an.NotificationId)
                                  && an.AccountFirebaseId == currentAccount.AccountFirebaseId && an.IsViewed == false,
                setter => setter.SetProperty(an => an.IsViewed, true)
            );
        });
    }

    public async Task SendNotificationAsync(string userFirebaseId, string title, string message, string thumbnail = "")
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Content = $"{title}: {message}",
            Thumbnail = thumbnail
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        var accountNotification = new AccountNotification
        {
            AccountFirebaseId = userFirebaseId,
            NotificationId = notification.Id,
            IsViewed = false
        };


        await _unitOfWork.AccountNotificationRepository.AddAsync(accountNotification);
        await _unitOfWork.SaveChangesAsync();

        await _serviceFactory.NotificationServiceHub.SendNotificationAsync(userFirebaseId, "", title, message);
    }
    public async Task SendNotificationToManyAsync(List<string> userFirebaseIds, string message, string thumbnail)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Content = message,
            Thumbnail = thumbnail
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        var accountNotifications = userFirebaseIds.Select(id => new AccountNotification
        {
            AccountFirebaseId = id,
            NotificationId = notification.Id,
            IsViewed = false
        }).ToList();


        await _unitOfWork.AccountNotificationRepository.AddRangeAsync(accountNotifications);
        await _unitOfWork.SaveChangesAsync();

        foreach (var accountNotification in accountNotifications)
        {
            await _serviceFactory.NotificationServiceHub.SendNotificationAsync(accountNotification.AccountFirebaseId, "", "", message);
        }
    }

    public async Task CronJobAutoRemovedOutDateNotifications()
    {
        // && x.CreatedAt.Date < DateTime.Now.Date.AddDays(15)

        var notificationRemovedList = await _unitOfWork.AccountNotificationRepository.FindAsync(x => x.IsViewed == true);

        var notificationIds = notificationRemovedList.Select(x => x.NotificationId).ToList();

        await _unitOfWork.AccountNotificationRepository
           .ExecuteDeleteAsync(x => notificationIds.Contains(x.NotificationId));

        await _unitOfWork.NotificationRepository.ExecuteDeleteAsync(x => notificationIds.Contains(x.Id));
    }

    public async Task<List<AccountNotification>> GetUserNotificationsAsync(string userId)
    {
        var result = await _unitOfWork.AccountNotificationRepository
            .FindProjectedAsync<AccountNotification>(an => an.AccountFirebaseId == userId && !an.IsViewed);
        return result;
    }

    public async Task ToggleNotificationViewStatus(Guid id, string accountId)
    {
        var notification =
            await _unitOfWork.NotificationRepository.FindSingleAsync(n => n.Id == id, false);

        if (notification is null) throw new NotFoundException("Notification not found.");

        var account =
            await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == accountId,
                false);

        if (account is null) throw new NotFoundException("Account not found.");

        var accountNotification = await _unitOfWork.AccountNotificationRepository.FindSingleAsync(an =>
            an.NotificationId == id
            && an.AccountFirebaseId == account.AccountFirebaseId);

        if (accountNotification is null)
            throw new ForbiddenMethodException("You're not allowed to change this notification view status.");

        accountNotification.IsViewed = true;
        accountNotification.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SendNotificationsToAllStaffsAsync(string title, string message)
    {
        var staffs = await _unitOfWork.AccountRepository.FindAsync(a => a.Role == Role.Staff,
            hasTrackings: false);

        List<Task> pushNotificationTasks = [];

        await SendNotificationToManyAsync(staffs.Select(s => s.AccountFirebaseId).ToList(), title + ": " + message, "");
        //foreach (var staff in staffs)
        //{
        //    pushNotificationTasks.Add((staff.AccountFirebaseId, title, ));
        //}

        //await Task.WhenAll(pushNotificationTasks);
    }
}