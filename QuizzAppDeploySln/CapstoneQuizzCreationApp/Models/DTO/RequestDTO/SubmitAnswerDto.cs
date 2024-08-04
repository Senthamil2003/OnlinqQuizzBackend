namespace CapstoneQuizzCreationApp.Models.DTO.RequestDTO
{
    public class SubmitAnswerDto
    {
        public int UserId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int TestId { get; set; }
        public int SubmissionId { get; set; }

    }
}
