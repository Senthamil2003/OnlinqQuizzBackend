using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class TestHistoryRepository : IRepository<int, TestHistory>
    {
        private readonly QuizzContext _context;
        public TestHistoryRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<TestHistory> Add(TestHistory item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "TestHistory cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the TestHistory. " + ex);
            }
        }


        public async Task<TestHistory> Delete(int key)
        {
            try
            {
                var TestHistory = await Get(key);
                _context.Remove(TestHistory);
                await _context.SaveChangesAsync();
                return TestHistory;
            }
            catch (TestHistoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the TestHistory. " + ex);
            }
        }


        public virtual async Task<TestHistory> Get(int key)
        {
            try
            {
                return await _context.TestHistories.SingleOrDefaultAsync(u => u.HistoryId == key)
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

        public virtual async Task<IEnumerable<TestHistory>> Get()
        {
            try
            {
                return await _context.TestHistories.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the TestHistorys. " + ex);
            }
        }


        public async Task<TestHistory> Update(TestHistory item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "TestHistory cannot be null");

            try
            {
                var TestHistory = await Get(item.HistoryId);
                _context.Entry(TestHistory).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return TestHistory;
            }
            catch (TestHistoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the TestHistory. " + ex);
            }
        }
    }

}
