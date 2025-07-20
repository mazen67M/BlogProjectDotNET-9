using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogProjectDotNET_9.Models;
using BlogProjectDotNET_9.Models.ViewModels;
using BlogProjectDotNET_9.Data;

namespace BlogProjectDotNET_9.Controllers
{
    [Authorize]
    public class CommentController :Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(AppDbContext context , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody]CommentViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                PostId = model.PostId,
                CommentText = model.Content,
                UserId = user.Id,
                CommentDate = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            string badgeClass = role switch
            {
                "Admin" => "bg-danger",
                "Author" => "bg-warning text-dark",
                "User" => "bg-secondary",
                _ => "bg-secondary"
            };

            return Json(new
            {
                userFullName = user.FullName,
                role = role,
                badgeClass = badgeClass,
                commentText = comment.CommentText,
                commentDate = comment.CommentDate.ToString("g"),
                profileImage = user.ProfilePictureUrl ?? "/images/default.jpg",
            });
        }
    }
}
