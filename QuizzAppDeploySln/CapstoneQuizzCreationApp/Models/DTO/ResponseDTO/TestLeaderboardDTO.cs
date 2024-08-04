namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class TestLeaderboardDTO
    {
        public int MyRank { get; set; }
        public bool IsPassed { get; set; }
        public int MaxMark {  get; set; }
        public double TestTakenCount { get; set; }
        public double PassCount {  get; set; }
        public List<LeaderBoardDTO> Leaders { get; set; }

    }
}
