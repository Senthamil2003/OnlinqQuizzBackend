using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class TransactionRepository : ITransactionService
    {
        private readonly QuizzContext _context;
        public TransactionRepository(QuizzContext context)
        {
            _context = context;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

    }
}
