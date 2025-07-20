using BlogProjectDotNET_9.Data;
using BlogProjectDotNET_9.Models;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace BlogProjectDotNET_9.Controllers
{
    public class CategoryController : Controller
    {
        public readonly AppDbContext context;
        public CategoryController(AppDbContext _context )
        {
            context = _context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await context.Categories.Include(p => p.Posts)
                 .Select(c => new CategoryViewModel
                 {
                     Id = c.Id,
                     Name = c.Name,
                     Description = c.Description,
                     PostCount = c.Posts.Count()
                 }).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description,

                };
                context.Add(category);
                context.Categories.Add(category);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var caegory = await context.Categories.FindAsync(id);
            if (caegory == null)
                return NotFound();

            var model = new CategoryViewModel
            {
                Id = caegory.Id,
                Name = caegory.Name,
                Description = caegory.Description
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var category = await context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound();

                category.Name = model.Name;
                category.Description = model.Description;

                context.Update(category);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Author , Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var CategoryFromDb = await context.Categories
                .Include(c=>c.Posts).
                FirstOrDefaultAsync(c=>c.Id ==id);

            if (CategoryFromDb == null)
                return NotFound();

            var viewModel = new CategoryViewModel { 
                Id = CategoryFromDb.Id,
                Name = CategoryFromDb.Name,
                Description = CategoryFromDb.Description,
                PostCount = CategoryFromDb.Posts.Count
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Author , Admin")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var categoryFormDb = await context.Categories.FindAsync(id);
            if (categoryFormDb == null)
            {
                return NotFound();
            }

            context.Categories.Remove(categoryFormDb);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
