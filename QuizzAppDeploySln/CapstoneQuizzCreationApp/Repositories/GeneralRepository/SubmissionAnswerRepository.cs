using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class SubmissionAnswerRepository : IRepository<int, SubmissionAnswer>
    {
        private readonly QuizzContext _context;


        public SubmissionAnswerRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<SubmissionAnswer> Add(SubmissionAnswer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "SubmissionAnswer cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the SubmissionAnswer. " + ex);
            }
        }


        public async Task<SubmissionAnswer> Delete(int key)
        {
            try
            {
                var SubmissionAnswer = await Get(key);
                _context.Remove(SubmissionAnswer);
                await _context.SaveChangesAsync();
                return SubmissionAnswer;
            }
            catch (SubmissionAnswerNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the SubmissionAnswer. " + ex);
            }
        }


        public virtual async Task<SubmissionAnswer> Get(int key)
        {
            try
            {
                return await _context.SubmissionAnswers.SingleOrDefaultAsync(u => u.AnswerId == key)
                    ?? throw new SubmissionAnswerNotFoundException($"No SubmissionAnswer found with given id {key}");
            }
            catch (SubmissionAnswerNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from SubmissionAnswer. " + ex);
            }
        }

        public virtual async Task<IEnumerable<SubmissionAnswer>> Get()
        {
            try
            {
                return await _context.SubmissionAnswers.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the SubmissionAnswers. " + ex);
            }
        }


        public async Task<SubmissionAnswer> Update(SubmissionAnswer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "SubmissionAnswer cannot be null");

            try
            {
                var SubmissionAnswer = await Get(item.AnswerId);
                _context.Entry(SubmissionAnswer).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return SubmissionAnswer;
            }
            catch (SubmissionAnswerNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the SubmissionAnswer. " + ex);
            }
        }
    }
}
