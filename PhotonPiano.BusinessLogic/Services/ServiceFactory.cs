using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotonPiano.BackgroundJob;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.PubSub.Notification;
using PhotonPiano.PubSub.Progress;
using Razor.Templating.Core;
using StackExchange.Redis;

namespace PhotonPiano.BusinessLogic.Services;

public class ServiceFactory : IServiceFactory
{
    private readonly Lazy<IAccountService> _accountService;

    private readonly Lazy<IApplicationService> _applicationService;

    private readonly Lazy<IAuthService> _authService;

    private readonly Lazy<IClassService> _classService;

    private readonly Lazy<ICriteriaService> _criteriaService;

    private readonly Lazy<IDefaultScheduleJob> _defaultScheduleJob;

    private readonly Lazy<IEmailService> _emailService;

    private readonly Lazy<IEntranceTestService> _entranceTestService;

    private readonly Lazy<IEntranceTestStudentService> _entranceTestStudentService;


    // Add ILogger  for logging
    private readonly ILogger<ServiceFactory> _logger;

    private readonly Lazy<INotificationService> _notificationService;

    private readonly Lazy<INotificationServiceHub> _notificationServiceHub;

    private readonly Lazy<IPaymentService> _paymentService;


    private readonly Lazy<IPinataService> _pinataService;

    private readonly Lazy<IRedisCacheService> _redisCacheService;

    private readonly Lazy<IRoomService> _roomService;

    private readonly Lazy<ISchedulerService> _schedulerService;

    private readonly Lazy<ISlotService> _slotService;

    private readonly Lazy<ISlotStudentService> _slotStudentService;

    private readonly Lazy<ISystemConfigService> _systemConfigService;

    private readonly Lazy<ITransactionService> _transactionService;

    private readonly Lazy<ITuitionService> _tutionService;

    private readonly Lazy<IStudentClassService> _studentClassService;

    private readonly Lazy<IDayOffService> _dayOffService;
    
    private readonly Lazy<ILearnerSurveyService> _learnerSurveyService;
    
    private readonly Lazy<ISurveyQuestionService> _surveryQuestionService;
    
    private readonly Lazy<ILevelService> _levelService;
    
    private readonly Lazy<IProgressServiceHub> _progressServiceHub;

    public ServiceFactory(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration,
        IOptions<SmtpAppSetting> smtpAppSettings, IHubContext<NotificationHub> hubContext,
        IHubContext<ProgressHub> progressHubContext,
        IConnectionMultiplexer redis, IOptions<VnPay> vnPay, ILogger<ServiceFactory> logger,
        IDefaultScheduleJob defaultScheduleJob, IRazorTemplateEngine razorTemplateEngine)
    {
        _logger = logger;
        _accountService = new Lazy<IAccountService>(() => new AccountService(unitOfWork, this));
        _authService =
            new Lazy<IAuthService>(() => new AuthService(httpClientFactory, unitOfWork, this, configuration));
        _redisCacheService = new Lazy<IRedisCacheService>(() => new RedisCacheService(redis));
        _entranceTestStudentService =
            new Lazy<IEntranceTestStudentService>(() => new EntranceTestStudentService(unitOfWork, this));
        _entranceTestService = new Lazy<IEntranceTestService>(() => new EntranceTestService(unitOfWork, this));
        _roomService = new Lazy<IRoomService>(() => new RoomService(unitOfWork));
        _criteriaService = new Lazy<ICriteriaService>(() => new CriteriaService(unitOfWork, this));
        _slotService = new Lazy<ISlotService>(() => new SlotService(this, unitOfWork));
        _paymentService = new Lazy<IPaymentService>(() => new PaymentService(configuration, vnPay));
        _classService = new Lazy<IClassService>(() => new ClassService(unitOfWork, this));
        _systemConfigService = new Lazy<ISystemConfigService>(() => new SystemConfigService(unitOfWork));
        _slotStudentService = new Lazy<ISlotStudentService>(() => new SlotStudentService(this, unitOfWork));
        _tutionService = new Lazy<ITuitionService>(() => new TuitionService(unitOfWork, this));
        _transactionService = new Lazy<ITransactionService>(() => new TransactionService(unitOfWork));
        _emailService =
            new Lazy<IEmailService>(() => new EmailService(razorTemplateEngine, defaultScheduleJob, smtpAppSettings));
        _schedulerService = new Lazy<ISchedulerService>(() => new SchedulerService(unitOfWork));
        _applicationService = new Lazy<IApplicationService>(() => new ApplicationService(unitOfWork, this));
        _pinataService = new Lazy<IPinataService>(() => new PinataService(configuration, httpClientFactory));
        _notificationServiceHub = new Lazy<INotificationServiceHub>(() => new NotificationServiceHub(hubContext));
        _notificationService = new Lazy<INotificationService>(() => new NotificationService(this, unitOfWork));
        _studentClassService = new Lazy<IStudentClassService>(() => new StudentClassService(this, unitOfWork));
        _dayOffService = new Lazy<IDayOffService>(() => new DayOffService(unitOfWork, this));
        // _logger.LogInformation("ServiceFactory has been initialized.");
        _learnerSurveyService = new Lazy<ILearnerSurveyService>(() => new LearnerSurveyService(unitOfWork));
        _surveryQuestionService = new Lazy<ISurveyQuestionService>(() => new SurveyQuestionService(unitOfWork));
        _progressServiceHub = new Lazy<IProgressServiceHub>(() => new ProgressServiceHub(progressHubContext));
        _levelService = new Lazy<ILevelService>(() => new LevelService(unitOfWork));
    }

    public IAccountService AccountService => _accountService.Value;

    public IAuthService AuthService => _authService.Value;

    public IRedisCacheService RedisCacheService => _redisCacheService.Value;

    public IEntranceTestStudentService EntranceTestStudentService => _entranceTestStudentService.Value;

    public IEntranceTestService EntranceTestService => _entranceTestService.Value;

    public IRoomService RoomService => _roomService.Value;

    public ICriteriaService CriteriaService => _criteriaService.Value;

    public ISlotService SlotService => _slotService.Value;

    public IPaymentService PaymentService => _paymentService.Value;

    public IClassService ClassService => _classService.Value;

    public ISystemConfigService SystemConfigService => _systemConfigService.Value;

    public ISlotStudentService SlotStudentService => _slotStudentService.Value;

    public ITuitionService TuitionService => _tutionService.Value;

    public ITransactionService TransactionService => _transactionService.Value;

    public IDefaultScheduleJob DefaultScheduleJob => _defaultScheduleJob.Value;

    public IEmailService EmailService => _emailService.Value;

    public ISchedulerService SchedulerService => _schedulerService.Value;

    public IApplicationService ApplicationService => _applicationService.Value;

    public IPinataService PinataService => _pinataService.Value;

    public INotificationService NotificationService => _notificationService.Value;

    public INotificationServiceHub NotificationServiceHub => _notificationServiceHub.Value;

    public IStudentClassService StudentClassService => _studentClassService.Value;

    public IDayOffService DayOffService => _dayOffService.Value;

    public IProgressServiceHub ProgressServiceHub => _progressServiceHub.Value;
    
    public ISurveyQuestionService ISurveyQuestionService => _surveryQuestionService.Value;
    
    public ILearnerSurveyService LearnerSurveyService => _learnerSurveyService.Value;

    public ILevelService LevelService => _levelService.Value;
}