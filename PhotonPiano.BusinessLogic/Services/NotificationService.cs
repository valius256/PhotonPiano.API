using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Notification;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
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

    public async Task SendNotificationAsync(string userFirebaseId, string title, string message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Content = $"{title}: {message}",
            Thumbnail = "",
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        var accountNotification = new AccountNotification
        {
            Id = Guid.NewGuid(),
            AccountFirebaseId = userFirebaseId,
            NotificationId = notification.Id,
            IsViewed = false
        };


        await _unitOfWork.AccountNotificationRepository.AddAsync(accountNotification);
        await _unitOfWork.SaveChangesAsync();

        await _serviceFactory.NotificationServiceHub.SendNotificationAsync(userFirebaseId, "", title, message);
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
            await _unitOfWork.NotificationRepository.FindSingleAsync(n => n.Id == id, hasTrackings: false);

        if (notification is null)
        {
            throw new NotFoundException("Notification not found.");
        }

        var account =
            await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == accountId,
                hasTrackings: false);

        if (account is null)
        {
            throw new NotFoundException("Account not found.");
        }

        var accountNotification = await _unitOfWork.AccountNotificationRepository.FindSingleAsync(an =>
            an.NotificationId == id
            && an.AccountFirebaseId == account.AccountFirebaseId);

        if (accountNotification is null)
        {
            throw new ForbiddenMethodException("You're not allowed to change this notification view status.");
        }

        accountNotification.IsViewed = true;
        accountNotification.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }
}