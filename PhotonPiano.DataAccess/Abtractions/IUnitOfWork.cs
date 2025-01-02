using Microsoft.EntityFrameworkCore.Storage;

namespace PhotonPiano.DataAccess.Abtractions
{
    public interface IUnitOfWork
    {
        // repositories here 

        Task<int> SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();

        Task ExecuteInTransactionAsync(Func<Task> action);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
