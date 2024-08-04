using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.JoinedRepository
{
    public class HistoryWithUserRepository:TestHistoryRepository
    {
        private readonly QuizzContext _context;
        public HistoryWithUserRepository(QuizzContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<TestHistory> Get(int key)
        {
            try
            {
                return await _context.TestHistories
                    .Include(x => x.User)   
                    .SingleOrDefaultAsync(u => u.HistoryId == key)
                    ?? throw new TestHistoryNotFoundException($"No TestHistory found with given id {key}");
            }
            catch (TestHistoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from TestHistory. " + ex);
            }
        }

        public override async Task<IEnumerable<TestHistory>> Get()
        {
            try
            {
                return await _context.TestHistories
                       .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the TestHistorys. " + ex);
            }
        }

    }
}
