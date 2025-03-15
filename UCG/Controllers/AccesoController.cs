using Microsoft.AspNetCore.Mvc;


using UCG.Models;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UCG.Services;
using static UCG.Models.TbUsuario;

namespace UCG.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UcgdbContext _context;
        private static Dictionary<string, int> _intentosPorUsuario = new Dictionary<string, int>();
        private readonly HashingService _hashingService;
        public AccesoController(UcgdbContext context, HashingService hashingService)
        {
            _context = context;
            _hashingService = hashingService;

        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrarse(usuarioViewModel modelo)
        {
            if (modelo.Contraseña != modelo.ConfirmarContraseña)
            {
                ModelState.AddModelError("Contraseña", "Las contraseñas no coinciden");
                return View(modelo);
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new TbUsuario
            {
                NombreUsuario = modelo.NombreUsuario,
                Contraseña = _hashingService.GenerateHash(modelo.Contraseña), // Hashear la contraseña
                Rol = RolUsuario.Admin, // Asigna el rol correcto
                Correo = modelo.Correo,
                Estado = EstadoUsuario.Activo
            };

            try
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Acceso"); // Redirigir al login tras el registro exitoso
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al registrar el usuario.");
                // Loggear el error si es necesario
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return View(modelo);
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Busca al usuario por correo
                TbUsuario? usuario_encontrado = await _context.TbUsuarios
                    .FirstOrDefaultAsync(u => u.Correo == modelo.Correo);

                // Verifica si se encontró el usuario
                if (usuario_encontrado != null)
                {
                    // Compara la contraseña ingresada con la almacenada
                    if (_hashingService.VeryfyHash(modelo.Contraseña, usuario_encontrado.Contraseña))
                    {
                        // Si la contraseña es correcta, crea los claims
                        List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario_encontrado.NombreUsuario),
                };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                        };

                        // Inicia sesión
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            properties
                        );

                        // Restablece los intentos
                        if (_intentosPorUsuario.ContainsKey(modelo.Correo))
                        {
                            _intentosPorUsuario[modelo.Correo] = 0; // Resetea los intentos
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                // Si la autenticación falla, aumenta los intentos
                if (!_intentosPorUsuario.ContainsKey(modelo.Correo))
                {
                    _intentosPorUsuario[modelo.Correo] = 0;
                }
                _intentosPorUsuario[modelo.Correo]++;

                // Verifica si se superaron los 3 intentos
                if (_intentosPorUsuario[modelo.Correo] >= 3)
                {
                    ViewData["Mensaje"] = "Has superado el número de intentos permitidos. Intenta más tarde.";
                    // Aquí deberías retornar la vista "Bloqueado".
                    return View("Bloqueado");
                }
                else
                {
                    ViewData["Mensaje"] = "Correo o contraseña incorrectos.";
                }
            }

            return View(modelo);
        }

    }
}
