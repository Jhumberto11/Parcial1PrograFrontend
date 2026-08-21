using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers
{
    
    public class ProductosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
