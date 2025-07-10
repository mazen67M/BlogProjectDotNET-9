using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogProjectDotNET_9.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogProjectDotNET_9.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        public string? ProfilePictureUrl { get; set; } = "/Images/default.jpg";

        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
