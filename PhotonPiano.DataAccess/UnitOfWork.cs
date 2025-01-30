using Microsoft.EntityFrameworkCore.Storage;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Repositories;

namespace PhotonPiano.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly Lazy<IAccountRepository> _accountRepository;

    private readonly Lazy<IClassRepository> _classRepository;

    private readonly ApplicationDbContext _context;

    private readonly Lazy<ICriteriaRepository> _criteriaRepository;

    private readonly Lazy<IEntranceTestRepository> _entranceTestRepository;

    private readonly Lazy<IEntranceTestStudentRepository> _entranceTestStudentRepository;

    private readonly Lazy<IRoomRepository> _roomRepository;

    private readonly Lazy<ISlotRepository> _slotRepository;

    private readonly Lazy<ISlotStudentRepository> _slotStudentRepository;

    private readonly Lazy<IStudentClassRepository> _studentClassRepository;

    private readonly Lazy<ISystemConfigRepository> _systemConfigRepository;

    private readonly Lazy<ITransactionRepository> _transactionRepository;

    private readonly Lazy<ITuitionRepository> _tuitionRepository;

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