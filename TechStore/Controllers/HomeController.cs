using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        
        public HomeController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
       
        public IActionResult PaginaNoEncontrada()
        {
             return View();
         }
        
        public async Task<IActionResult> Index()
        {
            // Los productos destacados se envian desde el controlador hacia la vista
             var productos = await _productService.GetAllProductsAsync();
            var destacados = productos.Where(p => p.Destacado).ToList();
            
            ViewBag.Categorias = _categoryService.GetAllCategories().Take(3).ToList();

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
