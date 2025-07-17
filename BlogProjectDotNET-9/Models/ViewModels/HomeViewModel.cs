namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Post> SliderPosts { get; set; } = new();
        public List<Post> RecentPosts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
