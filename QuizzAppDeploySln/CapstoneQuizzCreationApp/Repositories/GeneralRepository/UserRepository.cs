using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class UserRepository : IRepository<int, User>
    {
        private readonly QuizzContext _context;


        public UserRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<User> Add(User item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "User cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the User. " + ex);
            }
        }


        public async Task<User> Delete(int key)
        {
            try
            {
                var User = await Get(key);
                _context.Remove(User);
                await _context.SaveChangesAsync();
                return User;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the User. " + ex);
            }
        }


        public virtual async Task<User> Get(int key)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(u => u.UserId == key)
                    ?? throw new UserNotFoundException($"No User found with given id {key}");
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from User. " + ex);
            }
        }

        public virtual async Task<IEnumerable<User>> Get()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Users. " + ex);
            }
        }


        public async Task<User> Update(User item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "User cannot be null");

            try
            {
                var User = await Get(item.UserId);
                _context.Entry(User).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return User;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the User. " + ex);
            }
        }
    }
}
