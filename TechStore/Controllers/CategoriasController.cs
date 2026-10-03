using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Models.Interface;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriesRepository _categoryService;

        // Se inyecta la interfaz: el controlador no interactúa directamente con DbContext
        public CategoriasController(ICategoriesRepository categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var categorias = _categoryService.GetAllCategories();
            return View(categorias);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _categoryService.AddCategory(categoria);
                TempData["SuccessMessage"] = "Categoría guardada correctamente";
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoria = _categoryService.GetCategoryById(id.Value);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(categoria);
                TempData["SuccessMessage"] = "Categoría actualizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoria = _categoryService.GetCategoryById(id.Value);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryService.DeleteCategory(id);
            TempData["SuccessMessage"] = "Categoría eliminada correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}