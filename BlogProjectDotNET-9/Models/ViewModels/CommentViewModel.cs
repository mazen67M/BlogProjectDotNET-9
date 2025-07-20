using System.ComponentModel.DataAnnotations;

namespace BlogProjectDotNET_9.Models.ViewModels
{
    public class CommentViewModel
    {
        public int PostId { set; get; }

        [Required]
        public string Content { get; set; }
    }

}
