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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int? categoryID ,string? search )
        {
            var postQuery = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .AsQueryable();

            if (categoryID.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryID);

            }
            if (!string.IsNullOrEmpty(search))
            {
                postQuery = postQuery.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
            }
            var posts = postQuery.ToList();
            ViewBag.Categories = _context.Categories.ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PostListPartial", posts);
            }
            return View(posts);
        }

        [HttpGet]
        [AllowAnonymous]
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
        [Authorize(Roles = "Author")]
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
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Create(PostViewModel PostViewModel)
        {
            ModelState.Remove("Post.AuthorId");

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

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null) 
                {
                    PostViewModel.Post.AuthorId = userId;
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
                PostViewModel.Post.FeatureImagePath = "/images/default.jpg";
            }

            PostViewModel.Post.Id = 0;

            await _context.Posts.AddAsync(PostViewModel.Post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var postFromDb = await _context.Posts.FindAsync(id);
            if (postFromDb == null)
            {
                return NotFound();
            }

            // حذف الصورة من المجلد
            if (!string.IsNullOrEmpty(postFromDb.FeatureImagePath))
            {
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(postFromDb.FeatureImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }

            _context.Posts.Remove(postFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize(Roles = "Author")]
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


        //=============
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            ModelState.Remove("Post.AuthorId");

            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            var postFromDb = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(
                p => p.Id == editViewModel.Post.Id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            editViewModel.Post.AuthorId = postFromDb.AuthorId;

            if (editViewModel.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(editViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("Image", "Invalid image format. Allowed formats are .jpg, .jpeg, .png");
                    return View(editViewModel);
                }

                // حذف الصورة القديمة إذا كانت موجودة
                if (!string.IsNullOrEmpty(postFromDb.FeatureImagePath))
                {
                    var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                        Path.GetFileName(postFromDb.FeatureImagePath));
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }
                editViewModel.Post.FeatureImagePath = await UploadFileToExtension(editViewModel.FeatureImage);
            }
            else
            {
                editViewModel.Post.FeatureImagePath = postFromDb.FeatureImagePath;
            }

            _context.Posts.Update(editViewModel.Post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> MyPosts()
        {
            var userId = _userManager.GetUserId(User);
            var myPosts = await _context.Posts
                .Where(p=>p.AuthorId == userId)
                .Include(p => p.Category)
                .ToListAsync();

            return View(myPosts);
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
