using System.ComponentModel.DataAnnotations;

namespace CapstoneQuizzCreationApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool IsSubcribed { get; set; } = false;
        public ICollection<TestHistory> TestHistories { get; set; }   
        public ICollection<Favourite> Favourites { get; set; }
    }
}
