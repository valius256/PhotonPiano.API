using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;

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

    public async Task SendNotificationAsync(string userFirebaseId, string title, string message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Content = $"{title}: {message}",
            Thumbnail = ""
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
}