using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
