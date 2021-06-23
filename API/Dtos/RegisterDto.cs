using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        //u can find other regular expression on www.regexlib.com
        [RegularExpression("^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$",
            ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than six characters.")]
        public string Password { get; set; }
    }
}
