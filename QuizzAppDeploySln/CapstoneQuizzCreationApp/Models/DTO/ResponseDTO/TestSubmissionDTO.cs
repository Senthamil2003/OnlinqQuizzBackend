namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class TestSubmissionDTO
    {
        public int SubmissionId { get; set; }
        public int ObtainedScore { get; set; }
        public bool IsPassed { get; set; }
        public DateTime SubmissionDate { get; set; }
        public double TimeTaken { get; set; }

    }
}
