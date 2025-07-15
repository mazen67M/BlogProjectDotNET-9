using BlogProjectDotNET_9.Models;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BlogProjectDotNET_9.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var user = new ApplicationUser
            {
                UserName = model.userName,
                Email = model.Email,
                FullName = model.FullName,
                IsApproved = false
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                string roleToAssign = model.Role == "Author" ? "Author" : "User";
                await _userManager.AddToRoleAsync(user, roleToAssign);

                return RedirectToAction("PendingApproval");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Invalid Login");
                return View(model);
            }

            if (!user.IsApproved)
            {
                ModelState.AddModelError("", "Your Account is Not Approved yet");
                return View(model);
            }
            await _signInManager.SignInAsync(user, model.RememberMe);
            return RedirectToAction("Index", "Post");                    
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        [AllowAnonymous]
        public IActionResult PendingApproval()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
