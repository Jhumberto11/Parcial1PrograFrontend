using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Tests.Helpers
{
    public static class TestData
    {
        // Cada prueba usa su propia base en memoria (nombre único) para no mezclar datos
        public static DbContextOptions<AppDbContext> NuevasOpciones()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"TechStoreTest_{Guid.NewGuid()}")
                .Options;
        }

        public static Categoria Categoria(int id = 1, string nombre = "Computadoras") => new()
        {
            Id = id,
            Nombre = nombre,
            Descripcion = "Categoría de prueba",
            Imagen = "/images/computadoras.jpg"
        };

        public static Producto Producto(int id = 1, string nombre = "Laptop Lenovo", int stock = 10, int categoriaId = 1) => new()
        {
            Id = id,
            Nombre = nombre,
            Descripcion = "Producto de prueba",
            Precio = 799.99m,
            Stock = stock,
            CategoriaId = categoriaId,
            Imagen = "/images/laptop-lenovo.jpg"
        };

        public static List<Producto> ListaProductos() => new()
        {
            Producto(1, "Laptop Lenovo", 10, 1),
            Producto(2, "iPhone 15", 0, 2),
            Producto(3, "Mouse Logitech", 25, 3)
        };
    }
}
