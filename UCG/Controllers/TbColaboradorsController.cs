using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbColaboradorsController : Controller
    {
        private readonly UcgdbContext _context;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        public TbColaboradorsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbColaboradors
        public async Task<IActionResult> Index()
        {
            IQueryable<TbColaborador> filtroAsociacion = _context.TbColaboradors.Include(t => t.IdAsociacionNavigation);
            try
            {
                if (rol == "Admin"){
                    var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                    if (int.TryParse(idAsociacionClaim, out int idAsociacion))
                    {
                        filtroAsociacion = filtroAsociacion.Where(t => t.IdAsociacion == idAsociacion);
                    }
                }
                return View(await filtroAsociacion.ToListAsync());

            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los Colaboradores.";
                return RedirectToAction("Error");
            }
            //   return _context.TbColaboradors != null ? 
            //               View(await _context.TbColaboradors.ToListAsync()) :
            //               Problem("Entity set 'UcgdbContext.TbColaboradors'  is null.");
        }

        // GET: TbColaboradors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbColaboradors == null)
            {
                return NotFound();
            }

            var tbColaborador = await _context.TbColaboradors
                .Include(t => t.IdAsociacionNavigation)
                .Include(c => c.TbFacturas)
                .FirstOrDefaultAsync(m => m.IdColaborador == id);
            if (tbColaborador == null)
            {
                return NotFound();
            }

            return View(tbColaborador);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ColaboradorViewModel();
            await ConfigurarAsociacionColaboradorAsync(model);
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ColaboradorViewModel model)
        {
            var validator = new ColaboradorViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionColaboradorAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevo = MapearColaborador(model);



                _context.TbColaboradors.Add(nuevo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Colaborador creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el colaborador.";
                await ConfigurarAsociacionColaboradorAsync(model);
                return View(model);
            }
        }


        private TbColaborador MapearColaborador(ColaboradorViewModel model)
        {
            return new TbColaborador
            {
                IdColaborador = model.IdColaborador,
                IdAsociacion = model.IdAsociacion!.Value,
                Nombre = model.Nombre,
                Cedula = model.Cedula,
                Telefono = model.Telefono,
                Correo = model.Correo,
                Observaciones = model.Observaciones
            };
        }

        private async Task ConfigurarAsociacionColaboradorAsync(ColaboradorViewModel model)
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                if (int.TryParse(idAsociacionClaim, out int idAsociacion))
                {
                    var nombre = await _context.TbAsociacions
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .Select(a => a.Nombre)
                        .FirstOrDefaultAsync();

                    model.IdAsociacion = idAsociacion;
                    ViewBag.IdAsociacion = idAsociacion;
                    ViewBag.Nombre = nombre;
                    ViewBag.EsAdmin = true;
                }
            }
            else
            {
                ViewBag.EsAdmin = false;
                ViewBag.IdAsociacion = new SelectList(
                    await _context.TbAsociacions.ToListAsync(),
                    "IdAsociacion", "Nombre", model.IdAsociacion);
            }
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var colaborador = await _context.TbColaboradors.FindAsync(id);
            if (colaborador == null)
                return NotFound();

            var model = new ColaboradorViewModel
            {
                IdColaborador = colaborador.IdColaborador,
                IdAsociacion = colaborador.IdAsociacion,
                Nombre = colaborador.Nombre,
                Cedula = colaborador.Cedula,
                Telefono = colaborador.Telefono,
                Correo = colaborador.Correo,
                Observaciones = colaborador.Observaciones
            };

            await ConfigurarAsociacionColaboradorAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ColaboradorViewModel model)
        {
            var validator = new ColaboradorViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionColaboradorAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existente = await _context.TbColaboradors
                    .FirstOrDefaultAsync(c => c.IdColaborador == model.IdColaborador);

                if (existente == null)
                    return NotFound();

                // Actualización
                existente.IdAsociacion = model.IdAsociacion!.Value;
                existente.Nombre = model.Nombre;
                existente.Cedula = model.Cedula;
                existente.Telefono = model.Telefono;
                existente.Correo = model.Correo;
                existente.Observaciones = model.Observaciones;

                _context.Update(existente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Colaborador actualizado correctamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el colaborador.";
                await ConfigurarAsociacionColaboradorAsync(model);
                return View(model);
            }
        }


        // GET: TbColaboradors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbColaboradors == null)
            {
                return NotFound();
            }

            var tbColaborador = await _context.TbColaboradors
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdColaborador == id);
            if (tbColaborador == null)
            {
                return NotFound();
            }

            return View(tbColaborador);
        }

        // POST: TbColaboradors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbColaboradors == null)
            {
                return Problem("Entity set 'UcgdbContext.TbColaboradors'  is null.");
            }
            var tbColaborador = await _context.TbColaboradors.FindAsync(id);
            if (tbColaborador != null)
            {
                _context.TbColaboradors.Remove(tbColaborador);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbColaboradorExists(int id)
        {
          return (_context.TbColaboradors?.Any(e => e.IdColaborador == id)).GetValueOrDefault();
        }
    }
}
