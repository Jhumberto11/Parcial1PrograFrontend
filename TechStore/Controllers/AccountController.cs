
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechStore.Models.Identiry;

namespace TechStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            try
            {
                _logger.LogInformation(
                    "Se mostró el formulario de inicio de sesión");

                return View(new LoginModel { ReturnUrl = returnUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al mostrar el formulario de inicio de sesión");

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                await _signInManager.SignOutAsync();

                _logger.LogInformation(
                    "El usuario {UserId} cerró sesión correctamente",
                    userId);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al cerrar la sesión del usuario {UserId}",
                    userId);

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel login)
        {
            _logger.LogInformation(
                "Se inició un intento de inicio de sesión");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "El inicio de sesión no pudo continuar porque los datos no son válidos");

                    return View(login);
                }

                var user = await _userManager.FindByNameAsync(login.UserNameOrEmail)
                    ?? await _userManager.FindByEmailAsync(login.UserNameOrEmail);

                if (user == null)
                {
                    _logger.LogWarning(
                        "Intento de inicio de sesión fallido: no se encontró la cuenta");

                    ModelState.AddModelError(
                        string.Empty,
                        "Credenciales no válidas.");

                    return View(login);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    login.Password,
                    login.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "El usuario {UserId} inició sesión correctamente",
                        user.Id);

                    if (!string.IsNullOrEmpty(login.ReturnUrl)
                        && Url.IsLocalUrl(login.ReturnUrl))
                    {
                        return Redirect(login.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(
                        "La cuenta del usuario {UserId} fue bloqueada temporalmente",
                        user.Id);

                    ModelState.AddModelError(
                        string.Empty,
                        "La cuenta está bloqueada temporalmente por intentos fallidos");

                    return View(login);
                }

                _logger.LogWarning(
                    "Inicio de sesión fallido para el usuario {UserId}",
                    user.Id);

                ModelState.AddModelError(
                    string.Empty,
                    "Credenciales no válidas.");

                return View(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error inesperado durante el inicio de sesión");

                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            try
            {
                _logger.LogInformation(
                    "Se mostró el formulario de registro de usuarios");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al mostrar el formulario de registro");

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel register)
        {
            _logger.LogInformation(
                "Se inició el registro de un nuevo usuario");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "No se pudo registrar el usuario porque los datos no son válidos");

                    return View(register);
                }

                var user = new IdentityUser
                {
                    UserName = register.Username,
                    Email = register.Email
                };

                var result = await _userManager.CreateAsync(
                    user,
                    register.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "La cuenta del usuario {UserId} fue creada correctamente",
                        user.Id);

                    var resultadoRol = await _userManager.AddToRoleAsync(
                        user,
                        "Cliente");

                    if (!resultadoRol.Succeeded)
                    {
                        _logger.LogError(
                            "No se pudo asignar el rol Cliente al usuario {UserId}",
                            user.Id);

                        foreach (var error in resultadoRol.Errors)
                        {
                            ModelState.AddModelError(
                                string.Empty,
                                error.Description);
                        }

                        // Si falla la asignación del rol,
                        // intentamos eliminar la cuenta recién creada.

                        var eliminacion = await _userManager.DeleteAsync(user);

                        if (!eliminacion.Succeeded)
                        {
                            _logger.LogError(
                                "No se pudo eliminar la cuenta {UserId} después de fallar la asignación del rol",
                                user.Id);
                        }

                        return View(register);
                    }

                    _logger.LogInformation(
                        "Se asignó correctamente el rol Cliente al usuario {UserId}",
                        user.Id);

                    await _signInManager.SignInAsync(
                        user,
                        isPersistent: false);

                    _logger.LogInformation(
                        "El usuario {UserId} inició sesión después de registrarse",
                        user.Id);

                    return RedirectToAction("Index", "Home");
                }

                _logger.LogWarning(
                    "No se pudo crear la cuenta. Identity devolvió {CantidadErrores} errores",
                    result.Errors.Count());

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(register);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error inesperado durante el registro de usuarios");

                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            try
            {
                var userId = _userManager.GetUserId(User) ?? "Anónimo";

                _logger.LogWarning(
                    "El usuario {UserId} intentó acceder a una funcionalidad sin los permisos necesarios. Ruta: {Path}",
                    userId,
                    Request.Path);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al mostrar la página de acceso denegado");

                throw;
            }
        }
    }
}
