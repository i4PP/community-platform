
using System.ComponentModel.DataAnnotations;


namespace BMW.ASP.Models

{
    public class RegisterViewModel
    {

        public int? Id { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).+$", 
            ErrorMessage = "Password must contain at least one uppercase letter and one special character")]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string? ConfirmedPassword { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
