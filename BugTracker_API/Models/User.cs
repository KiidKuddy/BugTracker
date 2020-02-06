using System.ComponentModel.DataAnnotations;

namespace BugTracker_API.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        
        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }
        
        [Required]
        [MaxLength(15)]
        public string Type { get; set; }
    }
}