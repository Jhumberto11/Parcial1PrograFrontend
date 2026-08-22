using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Los productos destacados se envian desde el controlador hacia la vista
            var destacados = Datos.Productos.Where(p => p.Destacado).ToList();

            ViewBag.Categorias = Datos.Categorias.Take(3).ToList();

            return View(destacados);
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        // El formulario no hace un envio real, solo muestra un mensaje de confirmacion
        [HttpPost]
        public IActionResult ContactUs(string nombre, string correo, string asunto, string mensaje)
        {
            ViewBag.Mensaje = "Gracias " + nombre + ", recibimos tu mensaje. Te contactaremos pronto.";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
