using BlogProjectDotNET_9.Data;
using Microsoft.AspNetCore.Mvc;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogProjectDotNET_9.Controllers
{
    public class PostController : Controller
    {

        readonly AppDbContext _context;
        public PostController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            var PostViewModel = new PostViewModel();
            PostViewModel.Categories = _context.Categories.Select(c =>
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
            }
            ).ToList();

            return View(PostViewModel);
        }
    }
}
