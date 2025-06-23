using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProjectDotNET_9.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required,MaxLength(100,ErrorMessage ="Can't Exceed 100 character!")]
        public string UserName { get; set; }

        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; } = DateTime.Now;

        [Required]
        public string CommentText { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

    }
}
