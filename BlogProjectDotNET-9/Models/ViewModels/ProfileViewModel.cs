namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; } = "/Images/default.jpg";
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
        public string Role { get; set; }
    }
}
