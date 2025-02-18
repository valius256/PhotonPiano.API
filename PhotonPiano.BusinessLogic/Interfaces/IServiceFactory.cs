using PhotonPiano.BackgroundJob;
using PhotonPiano.PubSub.Notification;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IServiceFactory
{
    IRedisCacheService RedisCacheService { get; }

    IAuthService AuthService { get; }

    IAccountService AccountService { get; }

    IEntranceTestStudentService EntranceTestStudentService { get; }

    IEntranceTestService EntranceTestService { get; }

    IRoomService RoomService { get; }

    ICriteriaService CriteriaService { get; }

    ISlotService SlotService { get; }

    IPaymentService PaymentService { get; }

    IClassService ClassService { get; }

    ISystemConfigService SystemConfigService { get; }

    ISlotStudentService SlotStudentService { get; }

    ITutionService TutionService { get; }

    ITransactionService TransactionService { get; }

    IDefaultScheduleJob DefaultScheduleJob { get; }

    IEmailService EmailService { get; }

    ISchedulerService SchedulerService { get; }

    IApplicationService ApplicationService { get; }
  
    IPinataService PinataService { get; }

    INotificationService NotificationService { get; }

    INotificationServiceHub NotificationServiceHub { get; }

    IPinataService PinataService { get; }
}