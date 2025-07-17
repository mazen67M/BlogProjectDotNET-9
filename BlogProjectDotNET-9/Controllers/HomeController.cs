using BlogProjectDotNET_9.Data;
using BlogProjectDotNET_9.Models;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogProjectDotNET_9.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; // Fixed assignment of _context
        }

        public IActionResult Index()
{
    var sliderPosts = _context.Posts
        .Include(p => p.Author)
        .Include(p => p.Category)
        .OrderByDescending(p => p.PublishedDate)
        .Take(5)
        .ToList();

    var recentPosts = _context.Posts
        .Include(p => p.Author)
        .Include(p => p.Category)
        .OrderByDescending(p => p.PublishedDate)
        .Skip(5)
        .Take(5)
        .ToList();

    var categories = _context.Categories.ToList();

    var viewModel = new HomeViewModel
    {
        SliderPosts = sliderPosts,
        RecentPosts = recentPosts,
        Categories = categories
    };

    return View(viewModel);
}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
