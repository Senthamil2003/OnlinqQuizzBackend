using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;

namespace CapstoneQuizzCreationApp.Interfaces
{
    public interface IAdminService
    {
        public Task<SuccessCertificationTestCreatedDTO> CreateCertificationTest(CreateQuestionDTO createQuestion);
        public Task<TestVisibleResponseDTO> ChangeVisibility(int testId);
        public Task<List<AdminAllTestDTO>> GetAllTest();

    }
}
