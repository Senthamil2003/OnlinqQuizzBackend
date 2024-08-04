namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class QuestionWithExpiryDate
    {
        public DateTime TestEndTime { get; set; }
        public int SubmissionId { get; set; }
        public bool IsSubmited { get; set; } = false;   
        public int TestDuration { get; set; }
        public string TestName { get; set; }
        public List<TestQuestionDTO> testQuestion {get; set; }
    }
}
