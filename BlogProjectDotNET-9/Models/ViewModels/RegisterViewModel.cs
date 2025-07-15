using System.ComponentModel.DataAnnotations;

namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select your role.")]
        [Display(Name = "Account Type")]
        public string Role { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public static readonly List<string> AvailableRoles = new()
        {
            "User",
            "Author"
        };

    }
}
