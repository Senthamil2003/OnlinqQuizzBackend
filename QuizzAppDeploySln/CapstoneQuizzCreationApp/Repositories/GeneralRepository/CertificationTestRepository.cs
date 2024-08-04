using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class CertificationTestRepository : IRepository<int, CertificationTest>
    {
        private readonly QuizzContext _context;


        public CertificationTestRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<CertificationTest> Add(CertificationTest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "CertificationTest cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the CertificationTest. " + ex);
            }
        }


        public async Task<CertificationTest> Delete(int key)
        {
            try
            {
                var CertificationTest = await Get(key);
                _context.Remove(CertificationTest);
                await _context.SaveChangesAsync();
                return CertificationTest;
            }
            catch (CertificationTestNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the CertificationTest. " + ex);
            }
        }


        public virtual async Task<CertificationTest> Get(int key)
        {
            try
            {
                return await _context.CertificationTests.SingleOrDefaultAsync(u => u.TestId == key)
                    ?? throw new CertificationTestNotFoundException($"No CertificationTest found with given id {key}");
            }
            catch (CertificationTestNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from CertificationTest. " + ex);
            }
        }

        public virtual async Task<IEnumerable<CertificationTest>> Get()
        {
            try
            {
                return await _context.CertificationTests.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the CertificationTests. " + ex);
            }
        }


        public async Task<CertificationTest> Update(CertificationTest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "CertificationTest cannot be null");

            try
            {
                var CertificationTest = await Get(item.TestId);
                _context.Entry(CertificationTest).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return CertificationTest;
            }
            catch (CertificationTestNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the CertificationTest. " + ex);
            }
        }
    }
}
