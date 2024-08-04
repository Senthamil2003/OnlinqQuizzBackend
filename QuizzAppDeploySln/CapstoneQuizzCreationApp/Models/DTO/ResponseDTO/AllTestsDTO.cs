namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class AllTestsDTO
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public double TestTakenCount { get; set; }
        public double PassCount { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; } 
        public bool IsPassed { get; set; }=false;
        public bool IsResume {  get; set; }=false ;
        public bool IsAttend { get; set; } = false;
        public bool IsPending { get; set; }=false ;
        public bool IsActive { get; set; }
        public string TestDifficult { get; set; }
        public bool IsFavourite { get; set; } = false;
        public bool IsWait { get; set; } = false ;
        public int FavouriteId { get; set; } = 0;
        public string ImageUrl { get; set; }

    }
}
