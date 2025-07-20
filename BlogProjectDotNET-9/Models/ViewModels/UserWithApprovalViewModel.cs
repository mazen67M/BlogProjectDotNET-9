using System.ComponentModel.DataAnnotations;

namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class UserWithApprovalViewModel
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string Email { get; set; }

        [Display(Name ="Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Is Approved")]
        public bool isApproved { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        public string IsApprovedDisplay => isApproved ? "Yes" : "No";

        public string? ProfilePictureUrl { get; set; }
    }
}
