namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class TestResultDTO
    {
        public int TestId { get; set; }

        public string TestName { get; set; }
        public int ObtainedScore { get; set; }
        public int TotalScore { get; set; }
        public int MaxObtainedScore { get; set; }
        public bool IsPassed { get; set; }
        public int CertificateId { get; set; }
        public double TotalTimeTaken { get; set; }

    }
}
