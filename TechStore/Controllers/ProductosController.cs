using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechStore.Models;
using TechStore.Services;
using System.Collections.Generic;

namespace TechStore.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            ProductService productService,
            CategoryService categoryService,
            ILogger<ProductosController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoriaId)
        {
            _logger.LogInformation("Se inició la consulta del listado de productos");

            try
            {
                var productos = await _productService.GetAllProductsAsync();

                if (categoriaId != null)
                {
                    _logger.LogInformation(
                        "Se filtraron los productos por la categoría {CategoriaId}",
                        categoriaId);

                    productos = productos.Where(p => p.CategoriaId == categoriaId);
                }

                ViewBag.CategoriaId = categoriaId;
                ViewBag.Categorias = _categoryService.GetAllCategories().ToList();

                var listaProductos = productos.ToList();

                _logger.LogInformation(
                    "Consulta finalizada. Se encontraron {Cantidad} productos",
                    listaProductos.Count);

                return View(listaProductos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al consultar el listado de productos");
                throw;
            }
        }

        public async Task<IActionResult> Detalles(int id)
        {
            _logger.LogInformation(
                "Se inició la consulta de detalles del producto {ProductId}", id);

            try
            {
                var producto = await BuscarProducto(id);

                if (producto == null)
                {
                    return NotFound();
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar los detalles del producto {ProductId}",
                    id);

                throw;
            }
        }

        [Authorize(Roles = "Administrador, Vendedor")]
        public IActionResult Create()
        {
            _logger.LogInformation("Se abrió el formulario para registrar un producto");

            try
            {
                ViewBag.Categorias = _categoryService.GetAllCategories().ToList();

                return View(new Producto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al cargar el formulario de creación de productos");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Vendedor")]
        public async Task<IActionResult> Create(Producto producto)
        {
            _logger.LogInformation("Se inició el registro de un nuevo producto");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "No se pudo registrar el producto porque los datos proporcionados no son válidos");

                    ViewBag.Categorias = _categoryService.GetAllCategories().ToList();

                    return View(producto);
                }

                var productoGuardado = await _productService.AddProductAsync(producto);

                _logger.LogInformation(
                    "El producto {ProductId} se registró correctamente",
                    productoGuardado.Id);

                TempData["SuccessMessage"] = "Producto guardado correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al registrar un producto");
                throw;
            }
        }

        [Authorize(Roles = "Administrador, Vendedor")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation(
                "Se inició la edición del producto {ProductId}",
                id);

            try
            {
                var producto = await BuscarProducto(id);

                if (producto == null)
                {
                    return NotFound();
                }

                ViewBag.Categorias = _categoryService.GetAllCategories().ToList();

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al cargar el formulario de edición del producto {ProductId}",
                    id);

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Vendedor")]
        public async Task<IActionResult> Edit(Producto producto)
        {
            _logger.LogInformation(
                "Se inició la actualización del producto {ProductId}",
                producto.Id);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "No se pudo actualizar el producto {ProductId} porque los datos no son válidos",
                        producto.Id);

                    ViewBag.Categorias = _categoryService.GetAllCategories().ToList();

                    return View(producto);
                }

                if (await BuscarProducto(producto.Id) == null)
                {
                    return NotFound();
                }

                await _productService.UpdateProductAsync(producto);

                _logger.LogInformation(
                    "El producto {ProductId} se actualizó correctamente",
                    producto.Id);

                TempData["SuccessMessage"] = "Producto actualizado correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al actualizar el producto {ProductId}",
                    producto.Id);

                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Se inició la consulta para eliminar el producto {ProductId}",
                id);

            try
            {
                var producto = await BuscarProducto(id);

                if (producto == null)
                {
                    return NotFound();
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al cargar la confirmación de eliminación del producto {ProductId}",
                    id);

                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Se inició la eliminación del producto {ProductId}",
                id);

            try
            {
                if (await BuscarProducto(id) == null)
                {
                    return NotFound();
                }

                var eliminado = await _productService.DeleteProductAsync(id);

                if (!eliminado)
                {
                    _logger.LogWarning(
                        "No se pudo eliminar el producto {ProductId}",
                        id);

                    return NotFound();
                }

                _logger.LogInformation(
                    "El producto {ProductId} se eliminó correctamente",
                    id);

                TempData["SuccessMessage"] = "Producto eliminado correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al eliminar el producto {ProductId}",
                    id);

                throw;
            }
        }

        private async Task<Producto?> BuscarProducto(int id)
        {
            try
            {
                return await _productService.GetProductByIdAsync(id);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning(
                    "No se encontró ningún producto con el ID {ProductId}",
                    id);

                return null;
            }
        }
    }
}