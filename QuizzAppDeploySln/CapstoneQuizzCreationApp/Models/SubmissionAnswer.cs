using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneQuizzCreationApp.Models
{
    public class SubmissionAnswer
    {
        [Key]
        public int AnswerId { get; set; }
        public int SubmissionId { get; set; }
        public int QuestionId { get; set; }
        public string? Option {  get; set; }
        public bool IsCorrect { get; set; }=false;
        public bool IsMarked { get; set; }=false;
        [ForeignKey("SubmissionId")]
        public Submission Submission { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        //[ForeignKey("OptionId")]
        //public Option? Option { get; set; }  
    }
}
