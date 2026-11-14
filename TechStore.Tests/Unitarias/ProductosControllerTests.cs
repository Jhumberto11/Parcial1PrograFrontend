using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using TechStore.Controllers;
using TechStore.Data;
using TechStore.Models;
using TechStore.Models.Interface;
using TechStore.Services;
using TechStore.Tests.Helpers;

namespace TechStore.Tests.Unitarias
{
    // Pruebas del controlador de productos.
    // Se simula el repositorio y el logger con Moq.
    public class ProductosControllerTests
    {
        private readonly Mock<IProductoRepository> _repoMock = new();
        private readonly Mock<ILogger<ProductosController>> _loggerMock = new();
        private readonly ProductosController _controller;

        public ProductosControllerTests()
        {
            var context = new AppDbContext(TestData.NuevasOpciones());
            var categoryService = new CategoryService(context);
            var productService = new ProductService(_repoMock.Object);

            _controller = new ProductosController(productService, categoryService, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task Index_DevuelveVistaConTodosLosProductos()
        {
            _repoMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(TestData.ListaProductos());

            var resultado = await _controller.Index(null);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);
            Assert.Equal(3, modelo.Count());
        }

        [Fact]
        public async Task Index_ConCategoria_FiltraLosProductos()
        {
            _repoMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(TestData.ListaProductos());

            var resultado = await _controller.Index(2);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);
            Assert.Single(modelo);
            Assert.All(modelo, p => Assert.Equal(2, p.CategoriaId));
        }

        [Fact]
        public async Task Detalles_ProductoInexistente_DevuelveNotFound()
        {
            // El repositorio real lanza KeyNotFoundException cuando no encuentra el producto
            _repoMock.Setup(r => r.GetProductByIdAsync(99)).ThrowsAsync(new KeyNotFoundException());

            var resultado = await _controller.Detalles(99);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Create_Post_ModeloInvalido_DevuelveVistaYNoGuarda()
        {
            var producto = TestData.Producto(0, "");
            _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            var resultado = await _controller.Create(producto);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(producto, vista.Model);
            _repoMock.Verify(r => r.AddProductAsync(It.IsAny<Producto>()), Times.Never);
        }

        [Fact]
        public async Task Create_Post_Valido_GuardaYRedirigeAIndex()
        {
            var producto = TestData.Producto(0, "Parlante Bose");
            _repoMock.Setup(r => r.AddProductAsync(producto))
                     .ReturnsAsync(() => { producto.Id = 20; return producto; });

            var resultado = await _controller.Create(producto);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal(nameof(ProductosController.Index), redireccion.ActionName);
            Assert.Equal("Producto guardado correctamente", _controller.TempData["SuccessMessage"]);
            _repoMock.Verify(r => r.AddProductAsync(producto), Times.Once);
        }

        [Fact]
        public async Task Create_Post_Valido_RegistraEventoEnElLog()
        {
            var producto = TestData.Producto(0, "Hub USB");
            _repoMock.Setup(r => r.AddProductAsync(producto)).ReturnsAsync(producto);

            await _controller.Create(producto);

            // Comprueba que se escribió al menos un mensaje de nivel Information con ILogger
            _loggerMock.Verify(l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Create_Post_ErrorEnRepositorio_RegistraErrorYRelanza()
        {
            var producto = TestData.Producto(0, "Cargador USB-C");
            _repoMock.Setup(r => r.AddProductAsync(producto)).ThrowsAsync(new InvalidOperationException("Fallo BD"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Create(producto));

            _loggerMock.Verify(l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<InvalidOperationException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Edit_Post_ProductoValido_ActualizaYRedirige()
        {
            var producto = TestData.Producto(1, "Laptop Editada");
            _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(producto);
            _repoMock.Setup(r => r.UpdateProductAsync(producto)).ReturnsAsync(producto);

            var resultado = await _controller.Edit(producto);

            Assert.IsType<RedirectToActionResult>(resultado);
            _repoMock.Verify(r => r.UpdateProductAsync(producto), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_ProductoExistente_EliminaYRedirige()
        {
            _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(TestData.Producto(1));
            _repoMock.Setup(r => r.DeleteProductAsync(1)).ReturnsAsync(true);

            var resultado = await _controller.DeleteConfirmed(1);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal(nameof(ProductosController.Index), redireccion.ActionName);
            _repoMock.Verify(r => r.DeleteProductAsync(1), Times.Once);
        }
    }
}
