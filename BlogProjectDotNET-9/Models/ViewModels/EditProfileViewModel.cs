using System.ComponentModel.DataAnnotations;

namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public string fullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public string? ExistingImagePath { get; set; }
    }
}
