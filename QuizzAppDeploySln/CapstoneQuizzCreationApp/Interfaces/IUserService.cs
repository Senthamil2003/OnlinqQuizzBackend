using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;

namespace CapstoneQuizzCreationApp.Interfaces
{
    public interface IUserService
    {
        public Task<List<AllTestsDTO>> GetAllTestsWithUsers(int userId);
        public Task<UpdateFavouriteDTO> RemoveFromFavourite(int userId, int favouriteId);
        public Task<UpdateFavouriteDTO> AddToFavourate(int userId, int TestId);
        public Task<List<AllTestsDTO>> GetUserHistory(int userId);
        public Task<List<AllTestsDTO>> GetMyFavourite(int userId);
        public Task<List<AllTestsDTO>> GetPopularTests(int userId);
    }
}
