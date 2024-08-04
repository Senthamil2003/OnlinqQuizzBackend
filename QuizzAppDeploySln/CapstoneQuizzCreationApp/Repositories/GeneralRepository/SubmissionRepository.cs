using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class SubmissionRepository : IRepository<int, Submission>
    {
        private readonly QuizzContext _context;


        public SubmissionRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Submission> Add(Submission item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Submission cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Submission. " + ex);
            }
        }


        public async Task<Submission> Delete(int key)
        {
            try
            {
                var Submission = await Get(key);
                _context.Remove(Submission);
                await _context.SaveChangesAsync();
                return Submission;
            }
            catch (SubmissionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Submission. " + ex);
            }
        }


        public virtual async Task<Submission> Get(int key)
        {
            try
            {
                return await _context.Submissions.SingleOrDefaultAsync(u => u.SubmissionId == key)
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

        public virtual async Task<IEnumerable<Submission>> Get()
        {
            try
            {
                return await _context.Submissions.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Submissions. " + ex);
            }
        }


        public async Task<Submission> Update(Submission item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Submission cannot be null");

            try
            {
                var Submission = await Get(item.SubmissionId);
                _context.Entry(Submission).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Submission;
            }
            catch (SubmissionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Submission. " + ex);
            }
        }
    }
}
