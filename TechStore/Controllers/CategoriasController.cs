
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechStore.Models;
using TechStore.Models.Interface;

namespace TechStore.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly ICategoriesRepository _categoryService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(
            ICategoriesRepository categoryService,
            ILogger<CategoriasController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la consulta del listado de categorías");

                var categorias = _categoryService.GetAllCategories().ToList();

                _logger.LogInformation(
                    "Consulta finalizada. Se encontraron {Cantidad} categorías",
                    categorias.Count);

                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar el listado de categorías");

                throw;
            }
        }

        [Authorize(Roles = "Administrador, Vendedor")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation(
                    "Se abrió el formulario para registrar una categoría");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al mostrar el formulario de creación de categorías");

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Vendedor")]
        public IActionResult Create(Categoria categoria)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició el registro de una nueva categoría");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "No se pudo registrar la categoría porque los datos proporcionados no son válidos");

                    return View(categoria);
                }

                _categoryService.AddCategory(categoria);

                _logger.LogInformation(
                    "La categoría {CategoryName} se guardó correctamente",
                    categoria.Nombre);

                TempData["SuccessMessage"] =
                    "Categoría guardada correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al registrar la categoría {CategoryName}",
                    categoria?.Nombre);

                throw;
            }
        }

        [Authorize(Roles = "Administrador, Vendedor")]
        public IActionResult Edit(int? id)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la consulta para editar la categoría {CategoryId}",
                    id);

                if (id == null)
                {
                    _logger.LogWarning(
                        "No se pudo abrir la edición porque no se proporcionó un ID de categoría");

                    return NotFound();
                }

                var categoria = _categoryService.GetCategoryById(id.Value);

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoryId}",
                        id);

                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar la categoría {CategoryId} para editar",
                    id);

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Vendedor")]
        public IActionResult Edit(int id, Categoria categoria)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la actualización de la categoría {CategoryId}",
                    id);

                if (id != categoria.Id)
                {
                    _logger.LogWarning(
                        "El ID de la URL no coincide con el ID de la categoría enviada. ID recibido: {CategoryId}",
                        id);

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "No se pudo actualizar la categoría {CategoryId} porque los datos no son válidos",
                        id);

                    return View(categoria);
                }

                var categoriaExistente =
                    _categoryService.GetCategoryById(id);

                if (categoriaExistente == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría {CategoryId} que se intentó actualizar",
                        id);

                    return NotFound();
                }

                _categoryService.UpdateCategory(categoria);

                _logger.LogInformation(
                    "La categoría {CategoryId} se actualizó correctamente",
                    id);

                TempData["SuccessMessage"] =
                    "Categoría actualizada correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al actualizar la categoría {CategoryId}",
                    id);

                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int? id)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la consulta para eliminar la categoría {CategoryId}",
                    id);

                if (id == null)
                {
                    _logger.LogWarning(
                        "No se pudo consultar la categoría para eliminar porque no se proporcionó un ID");

                    return NotFound();
                }

                var categoria = _categoryService.GetCategoryById(id.Value);

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría {CategoryId} que se intentó eliminar",
                        id);

                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar la categoría {CategoryId} para eliminar",
                    id);

                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la eliminación de la categoría {CategoryId}",
                    id);

                var categoria = _categoryService.GetCategoryById(id);

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría {CategoryId} que se intentó eliminar",
                        id);

                    return NotFound();
                }

                _categoryService.DeleteCategory(id);

                _logger.LogInformation(
                    "La categoría {CategoryId} se eliminó correctamente",
                    id);

                TempData["SuccessMessage"] =
                    "Categoría eliminada correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al eliminar la categoría {CategoryId}",
                    id);

                throw;
            }
        }
    }
}
