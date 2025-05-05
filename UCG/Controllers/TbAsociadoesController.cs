using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbAsociadoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbAsociadoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbAsociadoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbAsociados.Include(t => t.IdAsociacionNavigation).Include(t => t.IdUsuarioNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbAsociadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbAsociados == null)
            {
                return NotFound();
            }

            var tbAsociado = await _context.TbAsociados
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsociado == id);
            if (tbAsociado == null)
            {
                return NotFound();
            }

            return View(tbAsociado);
        }

        // GET: TbAsociadoes/Create
        public IActionResult Create()
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var model = new AsociadoViewModel();

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                bool tieneAsociacion = int.TryParse(idAsociacionClaim, out int idAsociacion);

                // Obtener el nombre de la asociación desde la base de datos
                var Nombre = _context.TbAsociacions
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => a.Nombre)
                .FirstOrDefault();

                // Se mantiene seleccionable el usuario
                ViewBag.IdAsociacion = idAsociacion;
                ViewBag.Nombre = Nombre;
                ViewBag.EsAdmin = true;
                model.IdAsociacion = idAsociacion;


                ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "NombreUsuario");

                return View(model);
            }
            else{
                ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
                ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "NombreUsuario");
                ViewBag.EsAdmin = false;
                return View();
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AsociadoViewModel model)
        {
            var validator = new AsociadoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

            model.FechaNacimiento = DateOnly.ParseExact(model.FechaTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var asociado = MapearAsociado(model);

                _context.TbAsociados.Add(asociado);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El asociado fue creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar el asociado: " + ex.Message;
                await PrepararViewDataAsync(model);
                return View(model);
            }
        }

        private TbAsociado MapearAsociado(AsociadoViewModel model)
        {
            return new TbAsociado
            {

                IdAsociacion = model.IdAsociacion.Value,
                IdUsuario = model.IdUsuario,
                Nacionalidad = model.Nacionalidad,
                Cedula = model.Cedula,
                Apellido1 = model.Apellido1,
                Apellido2 = model.Apellido2,
                Nombre = model.Nombre,
                FechaNacimiento = model.FechaNacimiento,
                Sexo = model.Sexo,
                EstadoCivil = model.EstadoCivil,
                Telefono = model.Telefono,
                Correo = model.Correo,
                Direccion = model.Direccion,
                Estado = model.Estado
            };
        }


        private async Task PrepararViewDataAsync(AsociadoViewModel model)
        {
            ViewData["IdAsociacion"] = new SelectList(
               await _context.TbAsociacions.ToListAsync(),
               "IdAsociacion",
               "Nombre",
               model?.IdAsociacion);

            ViewData["IdUsuario"] = new SelectList(
                await _context.TbUsuarios.ToListAsync(),
                "IdUsuario",
                "NombreUsuario", // Usa el nombre que de verdad quieres mostrar
                model?.IdUsuario);
        }

        // GET: TbAsociadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbAsociados == null)
            {
                return NotFound();
            }

            var tbAsociado = await _context.TbAsociados.FindAsync(id);
            if (tbAsociado == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbAsociado.IdAsociacion);
            ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "IdUsuario", tbAsociado.IdUsuario);
            return View(tbAsociado);
        }

        // POST: TbAsociadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsociado,IdAsociacion,IdUsuario,Nacionalidad,Cedula,Apellido1,Apellido2,Nombre,FechaNacimiento,Sexo,EstadoCivil,Telefono,Correo,Direccion,Estado")] TbAsociado tbAsociado)
        {
            if (id != tbAsociado.IdAsociado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbAsociado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbAsociadoExists(tbAsociado.IdAsociado))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbAsociado.IdAsociacion);
            ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "IdUsuario", tbAsociado.IdUsuario);
            return View(tbAsociado);
        }

        // GET: TbAsociadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbAsociados == null)
            {
                return NotFound();
            }

            var tbAsociado = await _context.TbAsociados
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsociado == id);
            if (tbAsociado == null)
            {
                return NotFound();
            }

            return View(tbAsociado);
        }

        // POST: TbAsociadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbAsociados == null)
            {
                return Problem("Entity set 'UcgdbContext.TbAsociados'  is null.");
            }
            var tbAsociado = await _context.TbAsociados.FindAsync(id);
            if (tbAsociado != null)
            {
                _context.TbAsociados.Remove(tbAsociado);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbAsociadoExists(int id)
        {
          return (_context.TbAsociados?.Any(e => e.IdAsociado == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
