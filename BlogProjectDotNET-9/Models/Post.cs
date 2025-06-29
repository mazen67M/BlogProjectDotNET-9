using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace BlogProjectDotNET_9.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="The Title Field is Required!")]
        [MaxLength(100,ErrorMessage ="Cannot Exceed 100 character!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Content Field is Required!")]
        public string Content { get; set; }

        [Required(ErrorMessage = "The Author Field is Required!")]
        public string Author { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [ValidateNever]
        public string FeatureImagePath { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

            [ValidateNever]
        public ICollection<Comment> Comments { get; set; }

    }
}
