using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging.Abstractions;
using TechStore.Data;
using TechStore.Models.ViewModels;
using TechStore.Services;
using TechStore.Tests.Helpers;
using TechStore.ViewComponents;

namespace TechStore.Tests.Unitarias
{
    public class FiltroCategoriasViewComponentTests
    {
        [Fact]
        public void Invoke_DevuelveCategoriasYMarcaLaSeleccionada()
        {
            var opciones = TestData.NuevasOpciones();
            using (var context = new AppDbContext(opciones))
            {
                context.Categories.AddRange(
                    TestData.Categoria(1, "Computadoras"),
                    TestData.Categoria(2, "Teléfonos"));
                context.SaveChanges();
            }

            using var contextoPrueba = new AppDbContext(opciones);
            var componente = new FiltroCategoriasViewComponent(
                new CategoryService(contextoPrueba),
                NullLogger<FiltroCategoriasViewComponent>.Instance);

            var resultado = componente.Invoke(2);

            var vista = Assert.IsType<ViewViewComponentResult>(resultado);
            var modelo = Assert.IsType<FiltroCategoriasViewModel>(vista.ViewData!.Model);
            Assert.Equal(2, modelo.Categorias.Count);
            Assert.Equal(2, modelo.CategoriaSeleccionada);
        }
    }
}
