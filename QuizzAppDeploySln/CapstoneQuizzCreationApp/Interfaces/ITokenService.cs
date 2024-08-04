using CapstoneQuizzCreationApp.Models;
using System.Security.Claims;

namespace CapstoneQuizzCreationApp.Interfaces
{
    public interface ITokenService
    {
        public (bool isValid, ClaimsPrincipal? claimsPrincipal) ValidateToken(string token);
        public Task<string> GenerateToken(User login);
    }
}
