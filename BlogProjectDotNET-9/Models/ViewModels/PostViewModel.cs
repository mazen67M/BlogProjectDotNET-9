using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IFormFile FeatureImage { get; set; }

    }
}
