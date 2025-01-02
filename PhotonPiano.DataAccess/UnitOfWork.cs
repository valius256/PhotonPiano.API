using Microsoft.EntityFrameworkCore.Storage;
using PhotonPiano.DataAccess.Abtractions;
using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        private IDbContextTransaction? _currentTransaction;

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
}
