namespace CapstoneQuizzCreationApp.Models.DTO.RequestDTO
{
    public class CreateQuestionDTO
    {
        public string CertificationName { get; set; }
        public string TestDescription { get; set; }
        public string DificultyLeavel { get; set; }
        public string CreatedDate { get; set; }
        public int AttendQuestionCount { get; set; }
        public int TotalAvailableQuestion {  get; set; }
        public int RetakeWaitDays { get; set; }
        public int TestDuration {  get; set; }
        public bool IsActive { get; set; }
        public IFormFile TestImage { get; set; }
        public List<QuestionDTO> questions { get; set;}

    }
}
