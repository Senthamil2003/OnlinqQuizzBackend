using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class OptionRepository : IRepository<int, Option>
    {
        private readonly QuizzContext _context;


        public OptionRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Option> Add(Option item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Option cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Option. " + ex);
            }
        }


        public async Task<Option> Delete(int key)
        {
            try
            {
                var Option = await Get(key);
                _context.Remove(Option);
                await _context.SaveChangesAsync();
                return Option;
            }
            catch (OptionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Option. " + ex);
            }
        }


        public virtual async Task<Option> Get(int key)
        {
            try
            {
                return await _context.Options.SingleOrDefaultAsync(u => u.OptionId == key)
                    ?? throw new OptionNotFoundException($"No Option found with given id {key}");
            }
            catch (OptionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Option. " + ex);
            }
        }

        public virtual async Task<IEnumerable<Option>> Get()
        {
            try
            {
                return await _context.Options.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Options. " + ex);
            }
        }


        public async Task<Option> Update(Option item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Option cannot be null");

            try
            {
                var Option = await Get(item.OptionId);
                _context.Entry(Option).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Option;
            }
            catch (OptionNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Option. " + ex);
            }
        }
    }
}
