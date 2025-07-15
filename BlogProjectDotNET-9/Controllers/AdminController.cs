using BlogProjectDotNET_9.Data;
using BlogProjectDotNET_9.Models;
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
            return View(users);
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
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Users", _userManager.Users.ToList());
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

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Dashboard()
        {
            var usersCount = await _userManager.Users.CountAsync();
            var authorsCount = await _userManager.GetUsersInRoleAsync("Author");
            var postsCount = await _context.Posts.CountAsync();
            var commentsCount = await _context.Comments.CountAsync();
            var DefaultusersCount = await _userManager.GetUsersInRoleAsync("User");

            var unapprovedUsers = _userManager.Users
                                    .Where(u => !u.IsApproved)
                                    .ToList();

            ViewBag.UsersCount = usersCount;
            ViewBag.AuthorsCount = authorsCount.Count;
            ViewBag.PostsCount = postsCount;
            ViewBag.CommentsCount = commentsCount;

            return View(unapprovedUsers); // 🔁 بدلًا من return View() فقط
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
