using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using TechStore.Controllers;
using TechStore.Data;
using TechStore.Models;
using TechStore.Models.Identiry;
using TechStore.Repositorios;
using TechStore.Services;
using TechStore.Tests.Helpers;

namespace TechStore.Tests.E2E
{
    public class FlujosUsuarioE2ETests
    {
        private static ProductosController CrearProductosControllerFlujoReal(AppDbContext context, string username, string rol)
        {
            var repo = new ProductRepository(context);
            var productService = new ProductService(repo);
            var categoryService = new CategoryService(context);

            var controller = new ProductosController(
                productService,
                categoryService,
                NullLogger<ProductosController>.Instance);

            ConfigurarContextoUsuario(controller, username, rol);
            return controller;
        }

        private static void ConfigurarContextoUsuario(Controller controller, string username, string rol)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, rol)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        private static (AccountController controller, Mock<UserManager<IdentityUser>> userManagerMock, Mock<SignInManager<IdentityUser>> signInManagerMock) CrearAccountController()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            contextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

            var signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                claimsFactoryMock.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                NullLogger<SignInManager<IdentityUser>>.Instance,
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<IdentityUser>>());

            var controller = new AccountController(
                signInManagerMock.Object,
                userManagerMock.Object,
                NullLogger<AccountController>.Instance);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            return (controller, userManagerMock, signInManagerMock);
        }

        [Fact]
        public async Task E2E_01_InicioDeSesion_UsuarioValido_RedirigeAHomeIndex()
        {
            var (accountController, userManagerMock, signInManagerMock) = CrearAccountController();
            var usuario = new IdentityUser { Id = "user-1", UserName = "vendedor@techstore.com", Email = "vendedor@techstore.com" };

            userManagerMock.Setup(u => u.FindByNameAsync("vendedor@techstore.com")).ReturnsAsync(usuario);
            signInManagerMock.Setup(s => s.PasswordSignInAsync(usuario, "Pass123!", false, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var loginModel = new LoginModel
            {
                UserNameOrEmail = "vendedor@techstore.com",
                Password = "Pass123!",
                RememberMe = false
            };

            var resultado = await accountController.Login(loginModel);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal("Home", redireccion.ControllerName);
        }

        [Fact]
        public async Task E2E_02_ConsultaDeProductos_ClienteAutenticado_VisualizaCatalogoYDetalles()
        {
            var opciones = TestData.NuevasOpciones();

            using (var context = new AppDbContext(opciones))
            {
                context.Set<Categoria>().Add(TestData.Categoria(1, "Computadoras"));
                context.Set<Categoria>().Add(TestData.Categoria(2, "Accesorios"));
                context.Set<Producto>().AddRange(
                    TestData.Producto(1, "Laptop Lenovo", 10, 1),
                    TestData.Producto(2, "Mouse Logitech", 25, 2));
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(opciones))
            {
                var controller = CrearProductosControllerFlujoReal(context, "cliente@correo.com", "Cliente");

                // Flujo 1: Consultar todo el catálogo
                var vistaCatalogo = Assert.IsType<ViewResult>(await controller.Index(null));
                var listaCompleta = Assert.IsAssignableFrom<IEnumerable<Producto>>(vistaCatalogo.Model);
                Assert.Equal(2, listaCompleta.Count());

                // Flujo 2: Filtrar catálogo por categoría (Computadoras = 1)
                var vistaFiltrada = Assert.IsType<ViewResult>(await controller.Index(1));
                var listaFiltrada = Assert.IsAssignableFrom<IEnumerable<Producto>>(vistaFiltrada.Model);
                Assert.Single(listaFiltrada);

                // Flujo 3: Consultar detalles de un producto específico
                var vistaDetalles = Assert.IsType<ViewResult>(await controller.Detalles(1));
                var productoDetalle = Assert.IsType<Producto>(vistaDetalles.Model);
                Assert.Equal("Laptop Lenovo", productoDetalle.Nombre);
            }
        }

        [Fact]
        public async Task E2E_03_RegistroYEdicionDeProductos_UsuarioAutorizado_PersisteCambiosFlujoCompleto()
        {
            var opciones = TestData.NuevasOpciones();

            using (var context = new AppDbContext(opciones))
            {
                context.Set<Categoria>().Add(TestData.Categoria(1, "Computadoras"));
                await context.SaveChangesAsync();
            }

            int idCreado;

            using (var context = new AppDbContext(opciones))
            {
                var controller = CrearProductosControllerFlujoReal(context, "vendedor@techstore.com", "Vendedor");
                var nuevoProducto = TestData.Producto(0, "Monitor Gamer 144Hz", 15, 1);

                var resultadoCrear = await controller.Create(nuevoProducto);
                var redireccion = Assert.IsType<RedirectToActionResult>(resultadoCrear);
                Assert.Equal("Index", redireccion.ActionName);
                Assert.Equal("Producto guardado correctamente", controller.TempData["SuccessMessage"]);

                idCreado = context.Set<Producto>().Single().Id;
            }

            using (var context = new AppDbContext(opciones))
            {
                var controller = CrearProductosControllerFlujoReal(context, "vendedor@techstore.com", "Vendedor");
                var productoEditado = TestData.Producto(idCreado, "Monitor Gamer 240Hz Pro", 0, 1);
                productoEditado.Precio = 349.99m;

                var resultadoEditar = await controller.Edit(productoEditado);
                var redireccion = Assert.IsType<RedirectToActionResult>(resultadoEditar);
                Assert.Equal("Index", redireccion.ActionName);
                Assert.Equal("Producto actualizado correctamente", controller.TempData["SuccessMessage"]);
            }

            using (var context = new AppDbContext(opciones))
            {
                var controller = CrearProductosControllerFlujoReal(context, "vendedor@techstore.com", "Vendedor");
                var vistaDetalle = Assert.IsType<ViewResult>(await controller.Detalles(idCreado));
                var productoFinal = Assert.IsType<Producto>(vistaDetalle.Model);

                Assert.Equal("Monitor Gamer 240Hz Pro", productoFinal.Nombre);
                Assert.Equal(349.99m, productoFinal.Precio);
                Assert.Equal("Agotado", productoFinal.Estado);
            }
        }

        [Fact]
        public async Task E2E_04_AccesoSegunRol_AdministradorEliminaProductoYGeneraReportes()
        {
            var opciones = TestData.NuevasOpciones();

            using (var context = new AppDbContext(opciones))
            {
                context.Set<Categoria>().Add(TestData.Categoria(1, "Computadoras"));
                context.Set<Producto>().Add(TestData.Producto(1, "Teclado Mecánico RGB", 8, 1));
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(opciones))
            {
                var repo = new ProductRepository(context);
                var productService = new ProductService(repo);
                var reportesController = new ReportesController(productService, NullLogger<ReportesController>.Instance);
                ConfigurarContextoUsuario(reportesController, "admin@techstore.com", "Administrador");

                var archivoExcel = Assert.IsType<FileContentResult>(await reportesController.ExportarProductosExcel());
                Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", archivoExcel.ContentType);
                Assert.Equal("ReporteProductos.xlsx", archivoExcel.FileDownloadName);
                Assert.NotEmpty(archivoExcel.FileContents);
            }

            using (var context = new AppDbContext(opciones))
            {
                var controller = CrearProductosControllerFlujoReal(context, "admin@techstore.com", "Administrador");
                var resultadoEliminar = await controller.DeleteConfirmed(1);

                var redireccion = Assert.IsType<RedirectToActionResult>(resultadoEliminar);
                Assert.Equal("Index", redireccion.ActionName);
                Assert.Empty(context.Set<Producto>());
            }
        }

        [Fact]
        public void E2E_05_RestriccionDeAcceso_PermisosPorRolCumplenPoliticasDeSeguridad()
        {
            var authControladorProductos = typeof(ProductosController).GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(authControladorProductos);

            var metodoCreate = typeof(ProductosController).GetMethod(nameof(ProductosController.Create), Type.EmptyTypes);
            var authCreate = metodoCreate?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(authCreate);
            Assert.Contains("Administrador", authCreate.Roles);
            Assert.Contains("Vendedor", authCreate.Roles);
            Assert.DoesNotContain("Cliente", authCreate.Roles);

            var metodoDelete = typeof(ProductosController).GetMethod(nameof(ProductosController.Delete), new[] { typeof(int) });
            var authDelete = metodoDelete?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(authDelete);
            Assert.Equal("Administrador", authDelete.Roles);

            var authReportes = typeof(ReportesController).GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(authReportes);
            Assert.Contains("Administrador", authReportes.Roles);
            Assert.Contains("Vendedor", authReportes.Roles);
            Assert.DoesNotContain("Cliente", authReportes.Roles);
        }

        [Fact]
        public async Task E2E_06_MensajesDeErrorYAccesoDenegado_SeMuestranCorrectamente()
        {
            var (accountController, userManagerMock, _) = CrearAccountController();

            ConfigurarContextoUsuario(accountController, "cliente@correo.com", "Cliente");
            var resultadoAccesoDenegado = accountController.AccessDenied();
            Assert.IsType<ViewResult>(resultadoAccesoDenegado);

            userManagerMock.Setup(u => u.FindByNameAsync("inexistente@correo.com")).ReturnsAsync((IdentityUser?)null);
            userManagerMock.Setup(u => u.FindByEmailAsync("inexistente@correo.com")).ReturnsAsync((IdentityUser?)null);

            var loginFallido = new LoginModel
            {
                UserNameOrEmail = "inexistente@correo.com",
                Password = "ClaveIncorrecta"
            };

            var vistaLoginError = Assert.IsType<ViewResult>(await accountController.Login(loginFallido));
            Assert.False(accountController.ModelState.IsValid);
            var errores = accountController.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errores);
            Assert.Contains(errores, e => e.ErrorMessage == "Credenciales no válidas.");

            var opciones = TestData.NuevasOpciones();
            using var context = new AppDbContext(opciones);
            var productosController = CrearProductosControllerFlujoReal(context, "admin@techstore.com", "Administrador");

            var resultadoNoEncontrado = await productosController.Detalles(999);
            Assert.IsType<NotFoundResult>(resultadoNoEncontrado);
        }
    }
}