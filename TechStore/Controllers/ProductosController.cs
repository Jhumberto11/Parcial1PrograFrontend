using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ProductosController : Controller
    {
        // Si viene un categoriaId se filtra la lista, si no se muestran todos
        public IActionResult Index(int? categoriaId)
        {
            var productos = Datos.Productos.ToList();

            if (categoriaId != null)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId).ToList();

                var categoria = Datos.Categorias.FirstOrDefault(c => c.Id == categoriaId);
                ViewBag.CategoriaActual = categoria != null ? categoria.Nombre : "Categoria";
            }

            ViewBag.CategoriaId = categoriaId;
            ViewBag.Categorias = Datos.Categorias;

            return View(productos);
        }

        public IActionResult Detalles(int id)
        {
            var producto = Datos.Productos.FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }
    }
}
