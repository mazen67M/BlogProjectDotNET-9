using BlogProjectDotNET_9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogProjectDotNET_9.Views.Account
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        public IActionResult PendingUsers()
        {
            var users = _userManager.Users.Where(u => !u.IsApproved).ToList();
            return View(users);
        }

        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsApproved = true;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("PendingUsers");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
