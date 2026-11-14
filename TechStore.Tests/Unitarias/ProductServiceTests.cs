using Moq;
using TechStore.Models;
using TechStore.Models.Interface;
using TechStore.Services;
using TechStore.Tests.Helpers;

namespace TechStore.Tests.Unitarias
{
    // Pruebas unitarias del ProductService.
    // El repositorio se simula con Moq, así se prueba solo la lógica del servicio.
    public class ProductServiceTests
    {
        private readonly Mock<IProductoRepository> _repoMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductoRepository>();
            _service = new ProductService(_repoMock.Object);
        }

        // ---------- Consultar productos ----------

        [Fact]
        public async Task GetAllProductsAsync_DevuelveTodosLosProductos()
        {
            var productos = TestData.ListaProductos();
            _repoMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(productos);

            var resultado = await _service.GetAllProductsAsync();

            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            Assert.Contains(resultado, p => p.Nombre == "iPhone 15");
            _repoMock.Verify(r => r.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_SinProductos_DevuelveListaVacia()
        {
            _repoMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(new List<Producto>());

            var resultado = await _service.GetAllProductsAsync();

            Assert.Empty(resultado);
        }

        // ---------- Obtener por ID ----------

        [Fact]
        public async Task GetProductByIdAsync_IdExistente_DevuelveProducto()
        {
            var producto = TestData.Producto(5, "Teclado Redragon");
            _repoMock.Setup(r => r.GetProductByIdAsync(5)).ReturnsAsync(producto);

            var resultado = await _service.GetProductByIdAsync(5);

            Assert.Equal(5, resultado.Id);
            Assert.Equal("Teclado Redragon", resultado.Nombre);
        }

        [Fact]
        public async Task GetProductByIdAsync_IdInexistente_LanzaExcepcion()
        {
            _repoMock.Setup(r => r.GetProductByIdAsync(99)).ReturnsAsync((Producto)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetProductByIdAsync(99));

            Assert.Contains("99", ex.Message);
        }

        // ---------- Registrar ----------

        [Fact]
        public async Task AddProductAsync_ProductoValido_LlamaAlRepositorioYDevuelveProducto()
        {
            var nuevo = TestData.Producto(0, "Audífonos JBL");
            _repoMock.Setup(r => r.AddProductAsync(It.IsAny<Producto>()))
                     .ReturnsAsync((Producto p) => { p.Id = 10; return p; });

            var resultado = await _service.AddProductAsync(nuevo);

            Assert.Equal(10, resultado.Id);
            Assert.Equal("Audífonos JBL", resultado.Nombre);
            _repoMock.Verify(r => r.AddProductAsync(nuevo), Times.Once);
        }

        [Fact]
        public async Task AddProductAsync_ProductoNulo_LanzaExcepcionYNoGuarda()
        {
            await Assert.ThrowsAsync<Exception>(() => _service.AddProductAsync(null!));

            _repoMock.Verify(r => r.AddProductAsync(It.IsAny<Producto>()), Times.Never);
        }

        // ---------- Editar ----------

        [Fact]
        public async Task UpdateProductAsync_ProductoValido_ActualizaYDevuelveProducto()
        {
            var producto = TestData.Producto(1, "Laptop Lenovo Actualizada");
            producto.Precio = 650m;
            _repoMock.Setup(r => r.UpdateProductAsync(producto)).ReturnsAsync(producto);

            var resultado = await _service.UpdateProductAsync(producto);

            Assert.Equal("Laptop Lenovo Actualizada", resultado.Nombre);
            Assert.Equal(650m, resultado.Precio);
            _repoMock.Verify(r => r.UpdateProductAsync(producto), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ProductoNulo_LanzaExcepcionYNoActualiza()
        {
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateProductAsync(null!));

            _repoMock.Verify(r => r.UpdateProductAsync(It.IsAny<Producto>()), Times.Never);
        }

        // ---------- Eliminar ----------

        [Fact]
        public async Task DeleteProductAsync_IdExistente_EliminaYDevuelveTrue()
        {
            _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(TestData.Producto(1));
            _repoMock.Setup(r => r.DeleteProductAsync(1)).ReturnsAsync(true);

            var resultado = await _service.DeleteProductAsync(1);

            Assert.True(resultado);
            _repoMock.Verify(r => r.DeleteProductAsync(1), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task DeleteProductAsync_IdNoPositivo_LanzaExcepcion(int id)
        {
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteProductAsync(id));

            Assert.Equal("Id debe ser un valor positivo", ex.Message);
            _repoMock.Verify(r => r.DeleteProductAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_IdInexistente_LanzaExcepcionYNoElimina()
        {
            _repoMock.Setup(r => r.GetProductByIdAsync(50)).ReturnsAsync((Producto)null!);

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteProductAsync(50));

            _repoMock.Verify(r => r.DeleteProductAsync(It.IsAny<int>()), Times.Never);
        }

        // ---------- Propiedades calculadas del modelo ----------

        [Theory]
        [InlineData(10, "Disponible", true)]
        [InlineData(0, "Agotado", false)]
        public void Producto_Estado_SeCalculaSegunStock(int stock, string estadoEsperado, bool hayExistencias)
        {
            var producto = TestData.Producto(stock: stock);

            Assert.Equal(estadoEsperado, producto.Estado);
            Assert.Equal(hayExistencias, producto.HayExistencias);
        }
    }
}
