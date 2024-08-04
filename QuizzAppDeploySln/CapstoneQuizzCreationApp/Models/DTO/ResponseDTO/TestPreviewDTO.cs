namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class TestPreviewDTO
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int TotalQuestion {  get; set; }
        public int TotalMark { get; set; }
        public double Duration { get; set; }
        public int PassMark { get; set; }
        public bool IsWait { get; set; }

        public int SubmissionId { get; set; }
        public bool IsPending { get; set; }

        public DateTime LastTakenTime { get; set; }
        public DateTime NextTestTime { get; set; }
        
        public bool IsResume { get; set; }
    }
}
