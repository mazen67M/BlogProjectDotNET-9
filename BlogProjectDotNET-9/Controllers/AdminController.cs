using BlogProjectDotNET_9.Data;
using BlogProjectDotNET_9.Models;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProjectDotNET_9.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager
                               ,RoleManager<IdentityRole> roleManager
                               ,AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            var viewModel = new List<UserWithApprovalViewModel>();

            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                viewModel.Add(new UserWithApprovalViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    isApproved = user.IsApproved,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsApproved = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }

            TempData["Error"] = "Failed to approve user: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction("Users");
        }


        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
            await _userManager.AddToRoleAsync(user, newRole);

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var posts = _context.Posts.Where(p => p.AuthorId == user.Id).ToList();
            _context.Posts.RemoveRange(posts);
            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Users");
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Dashboard()
        {
            var usersCount = await _userManager.Users.CountAsync();
            var authorsCount = await _userManager.GetUsersInRoleAsync("Author");
            var postsCount = await _context.Posts.CountAsync();
            var commentsCount = await _context.Comments.CountAsync();

            var unapprovedUsers = _userManager.Users
                                    .Where(u => !u.IsApproved)
                                    .ToList();

            ViewBag.UsersCount = usersCount;
            ViewBag.AuthorsCount = authorsCount.Count;
            ViewBag.PostsCount = postsCount;
            ViewBag.CommentsCount = commentsCount;

            var model = new List<UserWithApprovalViewModel>();
            foreach (var user in unapprovedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();
                model.Add(new UserWithApprovalViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    isApproved = user.IsApproved,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }


            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
