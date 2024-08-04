using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneQuizzCreationApp.Models
{
    public class Submission
    {
        [Key] 
        public int SubmissionId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public DateTime StartTime { get; set; }
        public int TotalScore { get; set; }
        public int ObtainedScore { get; set; }
        public double TimeTaken { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public bool IsSubmited { get; set; }=false;
        public bool IsPassed { get; set; }=false ;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("TestId")]
        public CertificationTest CertificationTest { get; set; }
        public ICollection<SubmissionAnswer> SubmissionAnswers { get; set; }


    }
}
