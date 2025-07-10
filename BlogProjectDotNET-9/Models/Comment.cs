using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace BlogProjectDotNET_9.Models
    {
        public class Comment
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string CommentText { get; set; }

            public DateTime CommentDate { get; set; } = DateTime.Now;

            // علاقة مع البوست
            public int PostId { get; set; }
            [ValidateNever]
            public Post Post { get; set; }

            // علاقة مع المستخدم
            [ForeignKey("User")]
            public string UserId { get; set; }
            [ValidateNever]
            public ApplicationUser User { get; set; }
        }
    }
