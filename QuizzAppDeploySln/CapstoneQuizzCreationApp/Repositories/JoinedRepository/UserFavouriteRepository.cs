using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.JoinedRepository
{
    public class UserFavouriteRepository : UserRepository
    {
        private readonly QuizzContext _context;
        public UserFavouriteRepository(QuizzContext context) : base(context)
        {
            _context = context;

        }
        public override async Task<User> Get(int key)
        {
            try
            {
                return await _context.Users
                   
                    .Include(u => u.Favourites)
                    .SingleOrDefaultAsync(u => u.UserId == key)
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

        public override async Task<IEnumerable<User>> Get()
        {
            try
            {
                return await _context.Users
                   
                    .Include(u => u.Favourites)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Users. " + ex);
            }
        }

    }
}
