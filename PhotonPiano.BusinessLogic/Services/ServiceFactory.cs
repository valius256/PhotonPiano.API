using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using StackExchange.Redis;

namespace PhotonPiano.BusinessLogic.Services;

public class ServiceFactory : IServiceFactory
{
    private readonly Lazy<IAccountService> _accountService;

    private readonly Lazy<IAuthService> _authService;

    private readonly Lazy<IClassService> _classService;

    private readonly Lazy<ICriteriaService> _criteriaService;

    private readonly Lazy<IEntranceTestService> _entranceTestService;

    private readonly Lazy<IEntranceTestStudentService> _entranceTestStudentService;
    
    private readonly Lazy<IRedisCacheService> _redisCacheService;

    private readonly Lazy<IRoomService> _roomService;

    private readonly Lazy<ISlotService> _slotService;

    private readonly Lazy<ISLotStudentService> _sLotStudentService;

    private readonly Lazy<ISystemConfigService> _systemConfigService;

    private readonly Lazy<ITutionService> _tutionService;
    
    private readonly Lazy<IPaymentService> _paymentService;
    
    private readonly Lazy<ITransactionService> _transactionService;
    
    private readonly Lazy<ISchedulerService> _schedulerService;

    public ServiceFactory(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration,
        IConnectionMultiplexer redis, IOptions<VnPay> vnPay)
    {
        _accountService = new Lazy<IAccountService>(() => new AccountService(unitOfWork));
        _authService =
            new Lazy<IAuthService>(() => new AuthService(httpClientFactory, unitOfWork, this, configuration));
        _redisCacheService = new Lazy<IRedisCacheService>(() => new RedisCacheService(redis));
        _entranceTestStudentService =
            new Lazy<IEntranceTestStudentService>(() => new EntranceTestStudentService(unitOfWork, this));
        _entranceTestService = new Lazy<IEntranceTestService>(() => new EntranceTestService(unitOfWork, this));
        _roomService = new Lazy<IRoomService>(() => new RoomService(unitOfWork));
        _criteriaService = new Lazy<ICriteriaService>(() => new CriteriaService(unitOfWork, this));
        _slotService = new Lazy<ISlotService>(() => new SlotService(unitOfWork, this));
        _paymentService = new Lazy<IPaymentService>(() => new PaymentService(configuration, vnPay));
        _classService = new Lazy<IClassService>(() => new ClassService(unitOfWork, this));
        _systemConfigService = new Lazy<ISystemConfigService>(() => new SystemConfigService(unitOfWork));
        _sLotStudentService = new Lazy<ISLotStudentService>(() => new SLotStudentService(this, unitOfWork));
        _tutionService = new Lazy<ITutionService>(() => new TutionService(unitOfWork, this));
        _paymentService = new Lazy<IPaymentService>(() => new PaymentService(configuration, vnPay));
        _transactionService = new Lazy<ITransactionService>(() => new TransactionService(unitOfWork));
        _schedulerService = new Lazy<ISchedulerService>(() => new SchedulerService());
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

    public ISLotStudentService SLotStudentService => _sLotStudentService.Value;

    public ITutionService TutionService => _tutionService.Value;
    
    public ITransactionService TransactionService => _transactionService.Value;
    
    public ISchedulerService SchedulerService => _schedulerService.Value;
}