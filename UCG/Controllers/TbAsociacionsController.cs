using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;
using UCG.Services;

namespace UCG.Controllers
{
    public class TbAsociacionsController : Controller
    {
        private readonly UcgdbContext _context;
        private readonly HashingService _hashingService;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        
        public TbAsociacionsController(UcgdbContext context, HashingService hashingService)
        {
            _context = context;
            _hashingService = hashingService;
        }

        // GET: TbAsociacions
        public async Task<IActionResult> Index()
        {
              return _context.TbAsociacions != null ? 
                          View(await _context.TbAsociacions.ToListAsync()) :
                          Problem("Entity set 'UcgdbContext.TbAsociacions'  is null.");
        }

        // GET: TbAsociacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbAsociacions == null)
            {
                return NotFound();
            }

            var tbAsociacion = await _context.TbAsociacions
                 .Include(a => a.TbAsociados)
                .FirstOrDefaultAsync(m => m.IdAsociacion == id);
            if (tbAsociacion == null)
            {
                return NotFound();
            }

            return View(tbAsociacion);
        }

        // GET: TbAsociacions/Create
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: TbAsociacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AsociacionViewModel model)
        {
            if (model.Usuario.Contraseña != model.Usuario.ConfirmarContraseña)
            {
                ModelState.AddModelError("Contraseña", "Las contraseñas no coinciden");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Console.WriteLine($"FechaConstitucion: {model.FechaConstitucion}");


            model.FechaConstitucion = DateOnly.ParseExact(model.FechaConstitucionTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var tbAsociacion = new TbAsociacion
            {
                IdAsociacion = model.IdAsociacion,
                CedulaJuridica = model.CedulaJuridica,
                CodigoRegistro = model.CodigoRegistro,
                Nombre = model.Nombre,
                FechaConstitucion = model.FechaConstitucion,
                Telefono = model.Telefono,
                Fax = model.Fax,
                Correo = model.Correo,
                Provincia = model.Provincia,
                Canton = model.Canton,
                Distrito = model.Distrito,
                Pueblo = model.Pueblo,
                Direccion = model.Direccion,
                Descripcion = model.Descripcion,
                Estado = model.Estado
            };
            try
            {

                _context.Add(tbAsociacion);
                await _context.SaveChangesAsync();

                var tbUsuario = new TbUsuario
                {
                    IdAsociacion = tbAsociacion.IdAsociacion,
                    NombreUsuario = model.Usuario.NombreUsuario,
                    Contraseña = _hashingService.GenerateHash(model.Usuario.Contraseña),
                    Correo = model.Usuario.Correo,
                    Rol = model.Usuario.Rol ?? TbUsuario.RolUsuario.Admin,
                    Estado = model.Usuario.Estado ?? TbUsuario.EstadoUsuario.Activo
                };

                _context.Add(tbUsuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al registrar la asociación y el usuario.");
                Console.WriteLine($"Error al registrar: {ex.Message}");
                return View(model);
            }
        }



        //private Tb MapearAcuerdo(AcuerdoViewModel model)
        //{
        //    return new TbAcuerdo
        //    {

        //        IdAsociacion = model.IdActa.Value,
        //        NumeroAcuerdo = model.NumeroAcuerdo,
        //        Nombre = model.Nombre,
        //        Descripcion = model.Descripcion,
        //        MontoAcuerdo = model.MontoAcuerdo
        //    };
        //}


        //private async Task PrepararViewDataAsync(AcuerdoViewModel model)
        //{
        //    ViewData["IdActa"] = new SelectList(
        //        await _context.TbAsociacions.ToListAsync(),
        //        "IdActa",
        //        "NumeroActa",
        //        model.IdActa);
        //}
        // GET: TbAsociacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbAsociacions == null)
            {
                return NotFound();
            }

            var tbAsociacion = await _context.TbAsociacions.FindAsync(id);
            if (tbAsociacion == null)
            {
                return NotFound();
            }
            return View(tbAsociacion);
        }

        // POST: TbAsociacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsociacion,CedulaJuridica,CodigoRegistro,Nombre,FechaConstitucion,Telefono,Fax,Correo,Provincia,Canton,Distrito,Pueblo,Direccion,Descripcion,Estado")] TbAsociacion tbAsociacion)
        {
            if (id != tbAsociacion.IdAsociacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbAsociacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbAsociacionExists(tbAsociacion.IdAsociacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tbAsociacion);
        }

        // GET: TbAsociacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbAsociacions == null)
            {
                return NotFound();
            }

            var tbAsociacion = await _context.TbAsociacions
                .FirstOrDefaultAsync(m => m.IdAsociacion == id);
            if (tbAsociacion == null)
            {
                return NotFound();
            }

            return View(tbAsociacion);
        }

        // POST: TbAsociacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbAsociacions == null)
            {
                return Problem("Entity set 'UcgdbContext.TbAsociacions'  is null.");
            }
            var tbAsociacion = await _context.TbAsociacions.FindAsync(id);
            if (tbAsociacion != null)
            {
                _context.TbAsociacions.Remove(tbAsociacion);
                TempData["SuccessMessage"] = "La Asociacion fue eliminada correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbAsociacionExists(int id)
        {
          return (_context.TbAsociacions?.Any(e => e.IdAsociacion == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
