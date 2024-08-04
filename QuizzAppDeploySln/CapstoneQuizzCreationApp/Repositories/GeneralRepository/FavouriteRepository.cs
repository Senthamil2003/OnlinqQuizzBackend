using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class FavouriteRepository : IRepository<int, Favourite>
    {
        private readonly QuizzContext _context;


        public FavouriteRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Favourite> Add(Favourite item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Favourite cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Favourite. " + ex);
            }
        }


        public async Task<Favourite> Delete(int key)
        {
            try
            {
                var Favourite = await Get(key);
                _context.Remove(Favourite);
                await _context.SaveChangesAsync();
                return Favourite;
            }
            catch (FavouriteNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Favourite. " + ex);
            }
        }


        public virtual async Task<Favourite> Get(int key)
        {
            try
            {
                return await _context.Favourites.SingleOrDefaultAsync(u => u.FavouriteId == key)
                    ?? throw new FavouriteNotFoundException($"No Favourite found with given id {key}");
            }
            catch (FavouriteNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Favourite. " + ex);
            }
        }

        public virtual async Task<IEnumerable<Favourite>> Get()
        {
            try
            {
                return await _context.Favourites.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Favourites. " + ex);
            }
        }


        public async Task<Favourite> Update(Favourite item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Favourite cannot be null");

            try
            {
                var Favourite = await Get(item.FavouriteId);
                _context.Entry(Favourite).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Favourite;
            }
            catch (FavouriteNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Favourite. " + ex);
            }
        }
    }
}
