using Microsoft.EntityFrameworkCore.Storage;

namespace PhotonPiano.DataAccess.Abstractions
{
    public interface IUnitOfWork
    {
        // repositories here 
        IEntranceTestStudentRepository EntranceTestStudentRepository { get; }
        
        IAccountRepository AccountRepository { get; }
        
        IEntranceTestRepository EntranceTestRepository { get; }
        
        IRoomRepository RoomRepository { get; }

        Task<int> SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();

        Task ExecuteInTransactionAsync(Func<Task> action);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
