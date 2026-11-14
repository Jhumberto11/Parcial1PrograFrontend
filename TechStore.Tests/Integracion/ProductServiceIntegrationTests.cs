using TechStore.Data;
using TechStore.Repositorios;
using TechStore.Services;
using TechStore.Tests.Helpers;

namespace TechStore.Tests.Integracion
{
    // Prueba de integración: ProductService + ProductRepository + AppDbContext (EF Core InMemory)
    // trabajando juntos, sin mocks.
    public class ProductServiceIntegrationTests
    {
        private static ProductService CrearServicio(AppDbContext context) =>
            new ProductService(new ProductRepository(context));

        [Fact]
        public async Task CrudCompleto_RegistrarConsultarEditarEliminar()
        {
            var opciones = TestData.NuevasOpciones();

            using (var context = new AppDbContext(opciones))
            {
                context.Categories.Add(TestData.Categoria(1, "Computadoras"));
                await context.SaveChangesAsync();
            }

            int idGenerado;

            // 1. Registrar
            using (var context = new AppDbContext(opciones))
            {
                var guardado = await CrearServicio(context).AddProductAsync(TestData.Producto(0, "Laptop Asus"));
                idGenerado = guardado.Id;
                Assert.True(idGenerado > 0);
            }

            // 2. Consultar todos y por ID (incluye la categoría)
            using (var context = new AppDbContext(opciones))
            {
                var servicio = CrearServicio(context);

                var todos = await servicio.GetAllProductsAsync();
                Assert.Single(todos);

                var producto = await servicio.GetProductByIdAsync(idGenerado);
                Assert.Equal("Laptop Asus", producto.Nombre);
                Assert.NotNull(producto.Categoria);
                Assert.Equal("Computadoras", producto.Categoria!.Nombre);
            }

            // 3. Editar
            using (var context = new AppDbContext(opciones))
            {
                var cambios = TestData.Producto(idGenerado, "Laptop Asus ROG", stock: 0);
                cambios.Precio = 1200m;

                await CrearServicio(context).UpdateProductAsync(cambios);
            }

            using (var context = new AppDbContext(opciones))
            {
                var editado = await CrearServicio(context).GetProductByIdAsync(idGenerado);
                Assert.Equal("Laptop Asus ROG", editado.Nombre);
                Assert.Equal(1200m, editado.Precio);
                Assert.Equal("Agotado", editado.Estado);
            }

            // 4. Eliminar
            using (var context = new AppDbContext(opciones))
            {
                var eliminado = await CrearServicio(context).DeleteProductAsync(idGenerado);
                Assert.True(eliminado);
            }

            using (var context = new AppDbContext(opciones))
            {
                Assert.Empty(await CrearServicio(context).GetAllProductsAsync());
            }
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductoInexistente_LanzaKeyNotFound()
        {
            using var context = new AppDbContext(TestData.NuevasOpciones());

            // El repositorio real lanza KeyNotFoundException; el controlador la convierte en 404
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => CrearServicio(context).GetProductByIdAsync(123));
        }
    }
}
