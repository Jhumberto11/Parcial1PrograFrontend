using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        // Las categorias se envian desde el controlador hacia la vista
        public IActionResult Index()
        {
            return View(Datos.Categorias);
        }
    }
}
