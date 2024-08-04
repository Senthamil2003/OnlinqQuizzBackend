using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Repositories.GeneralRepository
{
    public class CertificateRepository : IRepository<int, Certificate>
    {
        private readonly QuizzContext _context;


        public CertificateRepository(QuizzContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Certificate> Add(Certificate item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Certificate cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Certificate. " + ex);
            }
        }


        public async Task<Certificate> Delete(int key)
        {
            try
            {
                var Certificate = await Get(key);
                _context.Remove(Certificate);
                await _context.SaveChangesAsync();
                return Certificate;
            }
            catch (CertificateNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Certificate. " + ex);
            }
        }


        public virtual async Task<Certificate> Get(int key)
        {
            try
            {
                return await _context.Certificates.SingleOrDefaultAsync(u => u.CertificateId == key)
                    ?? throw new CertificateNotFoundException($"No Certificate found with given id {key}");
            }
            catch (CertificateNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Certificate. " + ex);
            }
        }

        public virtual async Task<IEnumerable<Certificate>> Get()
        {
            try
            {
                return await _context.Certificates.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Certificates. " + ex);
            }
        }


        public async Task<Certificate> Update(Certificate item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Certificate cannot be null");

            try
            {
                var Certificate = await Get(item.CertificateId);
                _context.Entry(Certificate).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Certificate;
            }
            catch (CertificateNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Certificate. " + ex);
            }
        }
    }
}
