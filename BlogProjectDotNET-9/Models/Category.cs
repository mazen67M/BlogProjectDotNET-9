using System.ComponentModel.DataAnnotations;

namespace BlogProjectDotNET_9.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100,ErrorMessage ="The Name can't exceed 100 character!")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
