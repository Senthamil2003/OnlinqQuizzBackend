using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneQuizzCreationApp.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public int TestId { get; set; }
        public string QuestionDescription { get; set; }
        public int Points { get; set; }
        public string QuestionType { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsActive { get; set; }


        [ForeignKey("TestId")]
        public CertificationTest CertificationTest { get; set; }

      

        public ICollection<Option> Options { get; set; }




    }
}
