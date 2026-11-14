using Microsoft.AspNetCore.Mvc;
using TechStore.Models.ViewModels;
using TechStore.Services;

namespace TechStore.ViewComponents
{
    // Muestra los botones de filtro por categoría.
    // Consulta las categorías por sí mismo, así el controlador ya no necesita ViewBag.
    public class FiltroCategoriasViewComponent : ViewComponent
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<FiltroCategoriasViewComponent> _logger;

        public FiltroCategoriasViewComponent(
            CategoryService categoryService,
            ILogger<FiltroCategoriasViewComponent> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        public IViewComponentResult Invoke(int? categoriaSeleccionada)
        {
            try
            {
                var modelo = new FiltroCategoriasViewModel
                {
                    Categorias = _categoryService.GetAllCategories().ToList(),
                    CategoriaSeleccionada = categoriaSeleccionada
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                // Si falla, la página de productos se sigue mostrando, solo sin filtros
                _logger.LogError(ex, "Ocurrió un error al cargar el filtro de categorías");
                return View(new FiltroCategoriasViewModel { CategoriaSeleccionada = categoriaSeleccionada });
            }
        }
    }
}
