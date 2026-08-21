using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
