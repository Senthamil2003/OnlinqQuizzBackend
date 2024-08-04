namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class AdminAllTestDTO
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string TestDescription { get; set; }
        public string Difficulty { get; set; }
        public int Duration { get; set; }
        public bool IsActive { get; set; }

    }

}
