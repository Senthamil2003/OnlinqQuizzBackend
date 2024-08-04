using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.JoinedRepository
{
    public class SubmissionTestQuestionRepository:SubmissionRepository
    {
        private readonly QuizzContext _context;
        public SubmissionTestQuestionRepository(QuizzContext context):base(context)
        {
            _context = context;
        }
        public override async Task<Submission> Get(int key)
        {
            try
            {
                return await _context.Submissions
                    .Include(s=>s.CertificationTest)
                    .Include(s=>s.SubmissionAnswers)
                    .ThenInclude(a=>a.Question)
                    .ThenInclude(q=>q.Options)
                    .SingleOrDefaultAsync(u => u.SubmissionId == key)
                    ?? throw new SubmissionNotFoundException($"No Submission found with given id {key}");
            }
            catch (SubmissionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Submission. " + ex);
            }
        }

        public override async Task<IEnumerable<Submission>> Get()
        {
            try
            {
                return await _context.Submissions
                    .Include(s => s.SubmissionAnswers)
                    .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.Options)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Submissions. " + ex);
            }
        }



    }

}
