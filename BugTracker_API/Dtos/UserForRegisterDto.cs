using System.ComponentModel.DataAnnotations;

namespace BugTracker_API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(70, ErrorMessage = "The Name must have a maximum of 70 caracters")]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(255, ErrorMessage = "The Email must have a maximum of 255 caracters")]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "The Password must have at least 8 caracters")]
        public string Password { get; set; }
    }
}