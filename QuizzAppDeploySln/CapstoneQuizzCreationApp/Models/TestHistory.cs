using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneQuizzCreationApp.Models
{
    public class TestHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public  int MaxObtainedScore { get; set; }
        public int? CertificateId { get; set; }
        public bool IsPassed { get; set; }=false;
        public bool LatestIsSubmited { get; set; }=false ;
        public int? PassSubmissionId { get; set; }
        public double? TimeTaken { get; set; }
        public int LatestSubmissionId { get; set; }
        public DateTime? SubmissionTIme { get; set; }
        public DateTime LatesttestEndTime { get; set; }
     

        [ForeignKey("UserId")]   
        public User User { get; set; }
        [ForeignKey("TestId")]
        public CertificationTest CertificationTest { get; set; }

        [ForeignKey("CertificateId")]
        public Certificate? Certificate { get; set; }

        [ForeignKey("LatestSubmissionId")]
        public Submission Submission { get; set; }
    }
}
