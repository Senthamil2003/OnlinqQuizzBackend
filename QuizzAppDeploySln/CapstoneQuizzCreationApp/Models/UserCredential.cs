using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CapstoneQuizzCreationApp.Models
{
    public class UserCredential
    {
        [Key]
        public string Email { get; set; }
        public int UserId { get; set; }
        public byte[] Password { get; set; }
        public byte[] HasedPassword { get; set; }
        
        public string AccountStatus { get; set; } = "Enable";

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
