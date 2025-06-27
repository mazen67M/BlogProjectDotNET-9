using BlogProjectDotNET_9.Data;
using Microsoft.AspNetCore.Mvc;
using BlogProjectDotNET_9.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BlogProjectDotNET_9.Controllers
{
    public class PostController : Controller
    {

        readonly AppDbContext _context;
        readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> Create(PostViewModel PostViewModel)
        {
            if (ModelState.IsValid)
            {
                var inputFileExtentsion = Path.GetExtension(PostViewModel.FeatureImage.FileName).ToLower();
                bool isAllowedExtenstion = _allowedExtensions.Contains(inputFileExtentsion);
                if (!isAllowedExtenstion)
                {
                    ModelState.AddModelError("", "Invalid image format The allowed Formats are .jpg, .jpeg, .png");
                    return View(PostViewModel);
                }
                PostViewModel.Post.FeatureImagePath = await UploadFileToExtension(PostViewModel.FeatureImage);
                await _context.AddAsync(PostViewModel.Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(PostViewModel);

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
            try {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                { 
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex) {
                return "Error Uploading Image: "+ ex.Message;
            }
            return "/images/" + fileName;
        }
    } 
}
