using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using TechStore.Controllers;

namespace TechStore.Tests.Unitarias
{
    // Verifica que los controladores y acciones tengan los [Authorize] correctos por rol
    public class AutorizacionTests
    {
        private static List<string> RolesDe(MethodInfo metodo)
        {
            var atributo = metodo.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(atributo);
            Assert.False(string.IsNullOrWhiteSpace(atributo!.Roles));

            return atributo.Roles!.Split(',').Select(r => r.Trim()).ToList();
        }

        private static IEnumerable<MethodInfo> Acciones(Type controlador, string nombre) =>
            controlador.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                       .Where(m => m.Name == nombre);

        [Theory]
        [InlineData(typeof(ProductosController))]
        [InlineData(typeof(CategoriasController))]
        public void Controlador_RequiereUsuarioAutenticado(Type controlador)
        {
            Assert.NotNull(controlador.GetCustomAttribute<AuthorizeAttribute>());
        }

        [Theory]
        [InlineData(typeof(ProductosController), "Create")]
        [InlineData(typeof(ProductosController), "Edit")]
        [InlineData(typeof(CategoriasController), "Create")]
        [InlineData(typeof(CategoriasController), "Edit")]
        public void CrearYEditar_SoloAdministradorYVendedor(Type controlador, string accion)
        {
            var metodos = Acciones(controlador, accion).ToList();
            Assert.NotEmpty(metodos);

            foreach (var metodo in metodos)
            {
                var roles = RolesDe(metodo);
                Assert.Contains("Administrador", roles);
                Assert.Contains("Vendedor", roles);
                Assert.DoesNotContain("Cliente", roles);
            }
        }

        [Theory]
        [InlineData(typeof(ProductosController), "Delete")]
        [InlineData(typeof(ProductosController), "DeleteConfirmed")]
        [InlineData(typeof(CategoriasController), "Delete")]
        [InlineData(typeof(CategoriasController), "DeleteConfirmed")]
        public void Eliminar_SoloAdministrador(Type controlador, string accion)
        {
            var metodos = Acciones(controlador, accion).ToList();
            Assert.NotEmpty(metodos);

            foreach (var metodo in metodos)
            {
                var roles = RolesDe(metodo);
                Assert.Equal(new[] { "Administrador" }, roles);
            }
        }
    }
}
