using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;

namespace CapstoneQuizzCreationApp.Interfaces
{
    public interface IAuthService
    {
        public Task<SuccessLoginDTO> Login(LoginDTO loginDTO);
        public Task<SuccessRegisterDTO> Register(RegisterDTO employeeDTO);
        (bool isValid, string? role) ValidateUserTokenAndGetRole(string token);

    }
}
