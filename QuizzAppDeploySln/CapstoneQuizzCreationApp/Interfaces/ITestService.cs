using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;

namespace CapstoneQuizzCreationApp.Interfaces
{
    public interface ITestService
    {
        public  Task<StartTestDTO> StartTest(int certificationTestId, int UserId);
        public Task<QuestionWithExpiryDate> ResumeTest(int userId, int SubmissionId);
        public Task<SuccessSynchronieDTO> SynchronizeDb(List<SynchronousDataDTO> answers);
        public Task<TestResultDTO> SubmitAnswer(SubmissionAnswerDTO submissionAnswer);
        public Task<List<LeaderBoardDTO>> GetLeaderBoard(int TestId);
        public Task<TestStatsDTO> GetTestStats(int userId, int testId);
        public Task<List<TestSubmissionDTO>> GetUserSubmission(int userId, int testId);
        public Task<CertificateDataDTO> GetCertificateData(int userId, int testId);
        public Task<TestPreviewDTO> TestPreviewPage(int testId, int userId);



    }
}
