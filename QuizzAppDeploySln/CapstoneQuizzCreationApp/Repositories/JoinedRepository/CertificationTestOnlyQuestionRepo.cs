using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.JoinedRepository
{
    public class CertificationTestOnlyQuestionRepo : CertificationTestRepository
    {
        private readonly QuizzContext _context;
        public CertificationTestOnlyQuestionRepo(QuizzContext context) : base(context)
        {

            _context = context;

        }
        public async override Task<IEnumerable<CertificationTest>> Get()
        {
            try
            {
                return await _context.CertificationTests
                    .Include(c => c.Questions)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the CertificationTests. " + ex);
            }
        }
        public async override Task<CertificationTest> Get(int key)
        {
            try
            {
                return await _context.CertificationTests
                     .Include(c => c.Questions)
                     .SingleOrDefaultAsync(u => u.TestId == key)
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



    }
}
