using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
