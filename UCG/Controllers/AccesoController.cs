using Microsoft.AspNetCore.Mvc;


using UCG.Models;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UCG.Services;


namespace UCG.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UcgdbContext _appDBcontext;
        private static Dictionary<string, int> _intentosPorUsuario = new Dictionary<string, int>();
        private readonly HashingService _hashingService;
        public AccesoController(UcgdbContext context, HashingService hashingService)
        {
            _appDBcontext = context;
            _hashingService = hashingService;

        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Contraseña != modelo.ConfirmarContraseña)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            TbUsuario usuario = new TbUsuario
            {
                NombreUsuario = modelo.NombreUsuario,
                Contraseña = _hashingService.GenerateHash(modelo.Contraseña),
                Rol= RolUsuario.Admin, //el rol 3 pertenece a usuario
                Correo = modelo.Correo,
                Estado = EstadoUsuario.Activo
            };

            if (ModelState.IsValid)
            {
                var nombreUsuario = usuario.NombreUsuario;
                var contraseña = usuario.Contraseña;
                var idRol = usuario.Rol;
                var correo = usuario.Correo;
                var estado = usuario.Estado;

                // Llamar al procedimiento almacenado usando MySqlConnector
                var result = await _appDBcontext.Database.ExecuteSqlRawAsync(
                    "CALL sp_insertar_usuario(@p_nombre_usuario, @p_contraseña, @p_rol, @p_correo, @p_estado)",
                    new MySqlConnector.MySqlParameter("@p_nombre_usuario", nombreUsuario),
                    new MySqlConnector.MySqlParameter("@p_contraseña", contraseña),
                    new MySqlConnector.MySqlParameter("@p_rol", idRol),
                    new MySqlConnector.MySqlParameter("@p_correo", correo),
                    new MySqlConnector.MySqlParameter("@p_estado", estado)
                );

                // Verifica si la inserción fue exitosa (dependiendo del retorno de tu procedimiento)
                if (result > 0) // Cambia esto según cómo tu procedimiento maneje la inserción
                {
                    return RedirectToAction("Login", "Acceso");
                }
                else
                {
                    ViewData["Mensaje"] = "Error al registrar al usuario.";
                    return View();
                }
            }

            return View();
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
        public async Task<IActionResult> Login(LogInVM modelo)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Busca al usuario por correo
                TbUsuario? usuario_encontrado = await _appDBcontext.TbUsuarios
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
