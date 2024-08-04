using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class QuestionRepository : IRepository<int, Question>
    {
        private readonly QuizzContext _context;


        public QuestionRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Question> Add(Question item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Question cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Question. " + ex);
            }
        }


        public async Task<Question> Delete(int key)
        {
            try
            {
                var Question = await Get(key);
                _context.Remove(Question);
                await _context.SaveChangesAsync();
                return Question;
            }
            catch (QuestionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Question. " + ex);
            }
        }


        public virtual async Task<Question> Get(int key)
        {
            try
            {
                return await _context.Questions.SingleOrDefaultAsync(u => u.QuestionId == key)
                    ?? throw new QuestionNotFoundException($"No Question found with given id {key}");
            }
            catch (QuestionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Question. " + ex);
            }
        }

        public virtual async Task<IEnumerable<Question>> Get()
        {
            try
            {
                return await _context.Questions.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Questions. " + ex);
            }
        }


        public async Task<Question> Update(Question item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Question cannot be null");

            try
            {
                var Question = await Get(item.QuestionId);
                _context.Entry(Question).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Question;
            }
            catch (QuestionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Question. " + ex);
            }
        }
    }
}
