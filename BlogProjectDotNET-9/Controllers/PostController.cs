using BlogProjectDotNET_9.Data;
using BlogProjectDotNET_9.Models;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace BlogProjectDotNET_9.Controllers
{
    public class PostController : Controller
    {

        readonly AppDbContext _context;
        readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public UserManager<ApplicationUser> _userManager { get; }

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(int? categoryID)
        {
            var postQuery = _context.Posts.Include(p => p.Category).AsQueryable();
            if (categoryID.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryID);

            }
            var posts = postQuery.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts.Include(p => p.Category).Include(p => p.Comments)
                            .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<JsonResult> AddComment([FromBody] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTime.Now;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                comment.UserId = userId; 

                _context.Comments.Add(comment);
               await _context.SaveChangesAsync();
                var user = await _userManager.FindByIdAsync(userId);

                return Json(new
                {
                    username = user.UserName,
                    CommentDate = comment.CommentDate.ToString("yyyy-MM-dd HH:mm"),
                    Content = comment.CommentText,
                });
            }
            return Json(new { success = false, message = "Invalid comment data." });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var postFromDb = await _context.Posts.FindAsync(id);
            if (postFromDb == null)
            {
                return NotFound();
            }
            return View(postFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var postFromDb = await _context.Posts.FindAsync(id);
            if (postFromDb == null)
            {
                return NotFound();
            }

            // حذف الصورة من المجلد
            var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                Path.GetFileName(postFromDb.FeatureImagePath));
            if (System.IO.File.Exists(existingFilePath))
            {
                System.IO.File.Delete(existingFilePath);
            }

            _context.Posts.Remove(postFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel PostViewModel)
        {
            if (!ModelState.IsValid)
            {
                PostViewModel.Categories = _context.Categories.Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                    }
                ).ToList();

                return View(PostViewModel);
            }

            // التحقق من رفع صورة
            if (PostViewModel.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(PostViewModel.FeatureImage.FileName).ToLower();
                bool isAllowedExtension = _allowedExtensions.Contains(inputFileExtension);

                if (!isAllowedExtension)
                {
                    ModelState.AddModelError("", "Invalid image format. Allowed formats are .jpg, .jpeg, .png");

                    PostViewModel.Categories = _context.Categories.Select(c =>
                        new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.Name,
                        }
                    ).ToList();

                    return View(PostViewModel);
                }

                // رفع الصورة
                PostViewModel.Post.FeatureImagePath = await UploadFileToExtension(PostViewModel.FeatureImage);
            }
            else
            {
                // صورة افتراضية في حال لم تُرفع صورة
                PostViewModel.Post.FeatureImagePath = "/images/default.jpg";
            }

            // ✅ هذا السطر مهم لحل المشكلة
            PostViewModel.Post.Id = 0;

            await _context.Posts.AddAsync(PostViewModel.Post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (postFromDb == null)
            {
                return NotFound();
            }

            EditViewModel editViewModel = new EditViewModel()
            {
                Post = postFromDb,
                Categories = _context.Categories.Select(c =>
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }
                ).ToList()
            };
            return View(editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel postViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(postViewModel);
            }

            var postFromDb = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(
                p => p.Id == postViewModel.Post.Id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            if (postViewModel.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("Image", "Invalid image format. Allowed formats are .jpg, .jpeg, .png");
                    return View(postViewModel);
                }

                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                    Path.GetFileName(postFromDb.FeatureImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                postViewModel.Post.FeatureImagePath = await UploadFileToExtension(postViewModel.FeatureImage);
            }
            else
            {
                postViewModel.Post.FeatureImagePath = postFromDb.FeatureImagePath;
            }

            _context.Posts.Update(postViewModel.Post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private async Task<string> UploadFileToExtension(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var ImagesFolderPath = Path.Combine(wwwRootPath, "Images");

            if (!Directory.Exists(ImagesFolderPath))
            {
                Directory.CreateDirectory(ImagesFolderPath);
            }
            var filePath = Path.Combine(ImagesFolderPath, fileName);
            try
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return "Error Uploading Image: " + ex.Message;
            }
            return "/images/" + fileName;
        }
    }
}
