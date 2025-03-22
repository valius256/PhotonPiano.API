using Microsoft.EntityFrameworkCore.Storage;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Repositories;

namespace PhotonPiano.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly Lazy<IAccountNotificationRepository> _accountNotificationRepository;
    private readonly Lazy<IAccountRepository> _accountRepository;

    private readonly Lazy<IApplicationRepository> _applicationRepository;

    private readonly Lazy<IClassRepository> _classRepository;

    private readonly ApplicationDbContext _context;

    private readonly Lazy<ICriteriaRepository> _criteriaRepository;

    private readonly Lazy<IDayOffRepository> _dayOffRepository;

    private readonly Lazy<IEntranceTestRepository> _entranceTestRepository;

    private readonly Lazy<IEntranceTestStudentRepository> _entranceTestStudentRepository;

    private readonly Lazy<INotificationRepository> _notificationRepository;

    private readonly Lazy<IRoomRepository> _roomRepository;

    private readonly Lazy<ISlotRepository> _slotRepository;

    private readonly Lazy<ISlotStudentRepository> _slotStudentRepository;

    private readonly Lazy<IStudentClassRepository> _studentClassRepository;

    private readonly Lazy<ISystemConfigRepository> _systemConfigRepository;

    private readonly Lazy<ITransactionRepository> _transactionRepository;

    private readonly Lazy<ITuitionRepository> _tuitionRepository;
    
    private readonly Lazy<IEntranceTestResultRepository> _entranceTestResultRepository;


    private readonly Lazy<IStudentClassScoreRepository> _studentClassScoreRepository;
    
    private readonly Lazy<ISurveyQuestionRepository> _surveyQuestionRepository;
    
    private readonly Lazy<ILearnerSurveyRepository> _learnerSurveyRepository;
    
    private readonly Lazy<IPianoSurveyRepository> _pianoSurveyRepository;
    
    private readonly Lazy<ILearnerAnswerRepository> _learnerAnswerRepository;

    private readonly Lazy<ILevelRepository> _levelRepository;
    
    private readonly Lazy<IPianoSurveyQuestionRepository> _pianoSurveyQuestionRepository;

    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _accountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(context));
        _entranceTestStudentRepository =
            new Lazy<IEntranceTestStudentRepository>(() => new EntranceTestStudentRepository(context));
        _entranceTestRepository = new Lazy<IEntranceTestRepository>(() => new EntranceTestRepository(context));
        _roomRepository = new Lazy<IRoomRepository>(() => new RoomRepository(context));
        _criteriaRepository = new Lazy<ICriteriaRepository>(() => new CriteriaRepository(context));
        _slotRepository = new Lazy<ISlotRepository>(() => new SlotRepository(context));
        _transactionRepository = new Lazy<ITransactionRepository>(() => new TransactionRepository(context));
        _classRepository = new Lazy<IClassRepository>(() => new ClassRepository(context));
        _systemConfigRepository = new Lazy<ISystemConfigRepository>(() => new SystemConfigRepository(context));
        _studentClassRepository = new Lazy<IStudentClassRepository>(() => new StudentClassRepository(context));
        _slotStudentRepository = new Lazy<ISlotStudentRepository>(() => new SlotStudentRepository(context));
        _tuitionRepository = new Lazy<ITuitionRepository>(() => new TuitionRepository(context));
        _dayOffRepository = new Lazy<IDayOffRepository>(() => new DayOffRepository(context));
        _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(context));
        _applicationRepository = new Lazy<IApplicationRepository>(() => new ApplicationRepository(context));
        _accountNotificationRepository =
            new Lazy<IAccountNotificationRepository>(() => new AccountNotificationRepository(context));
        _studentClassScoreRepository = new Lazy<IStudentClassScoreRepository>(() => new StudentClassScoreRepository(context));
        _entranceTestResultRepository = new Lazy<IEntranceTestResultRepository>(() => new EntranceTestResultRepository(context));
        _surveyQuestionRepository = new Lazy<ISurveyQuestionRepository>(() => new SurveyQuestionRepository(context));  
        _learnerSurveyRepository = new Lazy<ILearnerSurveyRepository>(() => new LearnerSurveyRepository(context));
        _pianoSurveyRepository = new Lazy<IPianoSurveyRepository>(() => new PianoSurveyRepository(context));
        _learnerAnswerRepository = new Lazy<ILearnerAnswerRepository>(() => new LearnerAnswerRepository(context));
        _levelRepository = new Lazy<ILevelRepository>(() => new LevelRepository(context));
        _pianoSurveyQuestionRepository = new Lazy<IPianoSurveyQuestionRepository>(() => new PianoSurveyQuestionRepository(context));
    }

    public IEntranceTestStudentRepository EntranceTestStudentRepository => _entranceTestStudentRepository.Value;

    public IAccountRepository AccountRepository => _accountRepository.Value;

    public IEntranceTestRepository EntranceTestRepository => _entranceTestRepository.Value;

    public IRoomRepository RoomRepository => _roomRepository.Value;

    public ICriteriaRepository CriteriaRepository => _criteriaRepository.Value;

    public ISlotRepository SlotRepository => _slotRepository.Value;

    public ITransactionRepository TransactionRepository => _transactionRepository.Value;

    public IClassRepository ClassRepository => _classRepository.Value;

    public ISystemConfigRepository SystemConfigRepository => _systemConfigRepository.Value;

    public IStudentClassRepository StudentClassRepository => _studentClassRepository.Value;

    public ISlotStudentRepository SlotStudentRepository => _slotStudentRepository.Value;

    public ITuitionRepository TuitionRepository => _tuitionRepository.Value;

    public IDayOffRepository DayOffRepository => _dayOffRepository.Value;

    public IApplicationRepository ApplicationRepository => _applicationRepository.Value;

    public INotificationRepository NotificationRepository => _notificationRepository.Value;

    public IAccountNotificationRepository AccountNotificationRepository => _accountNotificationRepository.Value;
    
    public IEntranceTestResultRepository EntranceTestResultRepository => _entranceTestResultRepository.Value;


    public IStudentClassScoreRepository StudentClassScoreRepository => _studentClassScoreRepository.Value;
    
    public ISurveyQuestionRepository SurveyQuestionRepository => _surveyQuestionRepository.Value;
    
    public ILearnerSurveyRepository LearnerSurveyRepository => _learnerSurveyRepository.Value;
    
    public IPianoSurveyRepository PianoSurveyRepository => _pianoSurveyRepository.Value;
    
    public ILearnerAnswerRepository LearnerAnswerRepository => _learnerAnswerRepository.Value;

    public ILevelRepository LevelRepository => _levelRepository.Value;
    
    public IPianoSurveyQuestionRepository PianoSurveyQuestionRepository => _pianoSurveyQuestionRepository.Value;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync();
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.CommitAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.RollbackAsync();
            _currentTransaction = null;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await BeginTransactionAsync();
        try
        {
            // Execute the provided action and get the result
            await action();

            // Commit the transaction
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            // Rollback the transaction on failure
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        await using var transaction = await BeginTransactionAsync();
        try
        {
            // Execute the provided action and get the result
            var result = await action();

            // Commit the transaction
            await SaveChangesAsync();
            await transaction.CommitAsync();

            // Return the result
            return result;
        }
        catch
        {
            // Rollback the transaction on failure
            await transaction.RollbackAsync();
            throw;
        }
    }
}