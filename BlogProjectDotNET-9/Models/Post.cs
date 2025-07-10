using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BlogProjectDotNET_9.Models
    {
        public class Post
        {
            [Key]
            public int Id { get; set; }

            [Required, MaxLength(100)]
            public string Title { get; set; }

            [Required]
            public string Content { get; set; }

            public DateTime PublishedDate { get; set; } = DateTime.Now;

            [ValidateNever]
            public string? FeatureImagePath { get; set; }

            // علاقة مع المستخدم (الكاتب)
            [ForeignKey("Author")]
            public string AuthorId { get; set; }
            [ValidateNever]
            public ApplicationUser Author { get; set; }

            // علاقة مع التصنيف
            public int CategoryId { get; set; }
            [ValidateNever]
            public Category Category { get; set; }

            [ValidateNever]
            public ICollection<Comment> Comments { get; set; }
        }
    }
