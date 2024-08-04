using System.ComponentModel.DataAnnotations;

namespace CapstoneQuizzCreationApp.Models
{
    public class CertificationTest
    {
        [Key]
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string ImageUrl { get; set; }
        public string TestDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public double TestTakenCount { get; set; }
        public double PassCount { get; set; }
        public int TotalQuestionCount { get; set; }
        public int QuestionNeedTotake {  get; set; }
        public int RetakeWaitDays { get; set; }
        public int TestDurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public string dificultyLeavel {  get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
