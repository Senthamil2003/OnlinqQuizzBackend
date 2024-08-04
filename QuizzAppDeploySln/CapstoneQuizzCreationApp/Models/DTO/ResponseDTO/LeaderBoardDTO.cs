namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class LeaderBoardDTO
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } 
        public int MaxObtainedMark {  get; set; }
        public double TimeTaken { get; set; }
        public DateTime SubmissionTime { get; set; }

    }
}
