using Microsoft.AspNetCore.Mvc;

namespace BlogProjectDotNET_9.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
