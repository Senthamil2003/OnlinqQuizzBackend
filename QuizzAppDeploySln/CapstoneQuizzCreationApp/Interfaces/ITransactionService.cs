using Microsoft.EntityFrameworkCore.Storage;

namespace CapstoneQuizzCreationApp.Interfaces
{
    public interface ITransactionService
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
