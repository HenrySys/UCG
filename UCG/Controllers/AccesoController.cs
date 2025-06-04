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
       public async Task<IActionResult> Registrarse(UsuarioViewModel modelo)
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
                    .Include(u => u.IdAsociacionNavigation)
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
                    new Claim(ClaimTypes.Email,usuario_encontrado.Correo),
                    new Claim(ClaimTypes.Role, usuario_encontrado.Rol.ToString())
                };

                        if (usuario_encontrado.Rol == TbUsuario.RolUsuario.Admin)
                        {
                            if (usuario_encontrado.IdAsociacion.HasValue)
                            {
                                claims.Add(new Claim("IdAsociacion", usuario_encontrado.IdAsociacion.Value.ToString()));
                            }
                        }

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProps = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                        };

                        // Inicia sesión
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity),
                            authProps
                        );

                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewData["Mensaje"] = "Correo o contraseña incorrectos.";
            }

            return View(modelo);
        }

        public IActionResult Error()
        {
            return View("Error");
        }

   }
}
