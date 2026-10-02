using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Models.Interface;
using TechStore.Services;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly CategoryService _categoriesService;
        public CategoriasController(CategoryService categoryService)
        {
            _categoriesService = categoryService;
        }

        // Las categorias se envian desde el controlador hacia la vista
        public IActionResult Index()
        {
            return View(Datos.Categorias);
        }
    }
}
