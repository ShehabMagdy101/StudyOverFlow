using System.ComponentModel.DataAnnotations;

namespace StudyOverFlow.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
