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
        // =============================
        // SECCIÓN: EDITAR PRODUCTO
        // =============================

        public IActionResult Edit(int id)
        {
            var producto = Datos.Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

           
            ViewBag.Categorias = Datos.Categorias;
            return View(producto);
        }


        [HttpPost]
        public IActionResult Edit(Producto productoModificado)
        {
            var productoOriginal = Datos.Productos.FirstOrDefault(p => p.Id == productoModificado.Id);

            if (productoOriginal != null)
            {
                
                productoOriginal.Nombre = productoModificado.Nombre;
                productoOriginal.Descripcion = productoModificado.Descripcion;
                productoOriginal.Precio = productoModificado.Precio;
                productoOriginal.Stock = productoModificado.Stock;
                productoOriginal.Imagen = productoModificado.Imagen;
                productoOriginal.CategoriaId = productoModificado.CategoriaId;
                productoOriginal.Destacado = productoModificado.Destacado;

                return RedirectToAction("Index");
            }

         
            ViewBag.Categorias = Datos.Categorias;
            return View(productoModificado);
        }

        // =============================
        // SECCIÓN: ELIMINAR PRODUCTO
        // =============================

    
        public IActionResult Delete(int id)
        {
            var producto = Datos.Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var producto = Datos.Productos.FirstOrDefault(p => p.Id == id);

            if (producto != null)
            {
                Datos.Productos.Remove(producto);
            }

            return RedirectToAction("Index");
        }
    } 
}