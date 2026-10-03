using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductosController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoriaId)
        {
            var productos = await _productService.GetAllProductsAsync();

            if (categoriaId != null)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId);
            }

            ViewBag.CategoriaId = categoriaId;
            ViewBag.Categorias = _categoryService.GetAllCategories().ToList();

            return View(productos.ToList());
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var producto = await BuscarProducto(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = _categoryService.GetAllCategories().ToList();
            return View(new Producto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoryService.GetAllCategories().ToList();
                return View(producto);
            }

            await _productService.AddProductAsync(producto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await BuscarProducto(id);
            if (producto == null) return NotFound();

            ViewBag.Categorias = _categoryService.GetAllCategories().ToList();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoryService.GetAllCategories().ToList();
                return View(producto);
            }

            if (await BuscarProducto(producto.Id) == null) return NotFound();

            await _productService.UpdateProductAsync(producto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var producto = await BuscarProducto(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await BuscarProducto(id) == null) return NotFound();

            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<Producto?> BuscarProducto(int id)
        {
            try
            {
                return await _productService.GetProductByIdAsync(id);
            }
            catch
            {
                return null;
            }
        }
    }
}
