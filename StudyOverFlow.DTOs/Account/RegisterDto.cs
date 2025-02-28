
using StudyOverFlow.DTOs.Consts;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudyOverFlow.DTOs.Account
{
    public class RegisterDto
    {
        //[Required]
        //[StringLength(15, MinimumLength = 2, ErrorMessage = "Full Name must be at least {2} and maximum {1} characters")]
        //public string FirstName { get; set; }

        //[Required]
        //[StringLength(15, MinimumLength = 2, ErrorMessage = "Full Name must be at least {2} and maximum {1} characters")]
        //public string LastName { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Full Name must be at least {2} and maximum {1} characters")]
        public string UserName { get; set; }
        [Required]
        [RegularExpression("^((?!\\.)[\\w-_.]*[^.])(@\\w+)(\\.\\w+(\\.\\w+)?[^.\\W])$", ErrorMessage = "Invalid email address !")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password),
            RegularExpression(RegaxStatic.password, ErrorMessage = "that passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least eight characters long.")]
        // [StringLength(15, MinimumLength = 6, ErrorMessage = "Password Name must be at least {2} and maximum {1} characters")]
        public string Password { get; set; }
        public string? Phone { get; set; }


    }
}
