using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbFondosRecaudadosActividadsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbFondosRecaudadosActividadsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbFondosRecaudadosActividads
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbFondosRecaudadosActividads.Include(t => t.IdActividadNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbFondosRecaudadosActividads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbFondosRecaudadosActividads == null)
            {
                return NotFound();
            }

            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads
                .Include(t => t.IdActividadNavigation)
                .FirstOrDefaultAsync(m => m.IdFondosRecaudadosActividad == id);
            if (tbFondosRecaudadosActividad == null)
            {
                return NotFound();
            }

            return View(tbFondosRecaudadosActividad);
        }

        [HttpGet]
        public IActionResult Create(int? idActividad)
        {
            if (idActividad == null)
            {
                // Acceso general desde el menú: mostrar combo con todas las actividades
                ViewData["IdActividad"] = new SelectList(_context.TbActividads.ToList(), "IdActividad", "Nombre");
                return View(new FondosRecaudadosActividadViewModel());
            }

            // Acceso desde Detalle de Actividad: fijar el IdActividad
            var actividad = _context.TbActividads.FirstOrDefault(a => a.IdActividad == idActividad);
            if (actividad == null)
                return NotFound();

            var model = new FondosRecaudadosActividadViewModel
            {
                IdActividad = idActividad.Value,
            };

            ViewData["IdActividad"] = idActividad; // Para JS o mostrar ID si es necesario
            ViewBag.NombreActividad = actividad.Nombre; // Opcional para mostrar en la vista
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FondosRecaudadosActividadViewModel model)
        {
            // Validar y convertir la fecha desde el campo de texto
            if (!ValidarYAsignarFechaRegistro(model))
            {
                return View(model);
            }

            // Validación del modelo con FluentValidation
            var validator = new FondosRecaudadosActividadViewModelValidator();
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var fondo = MapearFondosRecaudados(model);

                _context.TbFondosRecaudadosActividads.Add(fondo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Fondo recaudado registrado correctamente.";
                ViewData["IdActividad"] = model.IdActividad; // Para JS

                return RedirectToAction("Create", new { idActividad = model.IdActividad });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar los fondos recaudados.";
                return View(model);
            }
        }



        private TbFondosRecaudadosActividad MapearFondosRecaudados(FondosRecaudadosActividadViewModel model)
        {
            return new TbFondosRecaudadosActividad
            {
                IdActividad = model.IdActividad,
                Detalle = model.Detalle,
                Monto = model.Monto,
                FechaRegistro = model.FechaRegistro
            };
        }


       

        private bool ValidarYAsignarFechaRegistro(FondosRecaudadosActividadViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoRegistro))
            {
                if (DateTime.TryParseExact(model.FechaTextoRegistro, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaRegistro = DateOnly.FromDateTime(fecha);
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = "Debe ingresar una fecha de registro válida (formato: yyyy-MM-dd).";
                    TempData.Keep("ErrorMessage");
                    return false;
                }
            }

            TempData["ErrorMessage"] = "Debe ingresar la fecha de registro.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        private FondosRecaudadosActividadViewModel MapearFondosRecaudadosActividadViewModel(TbFondosRecaudadosActividad entidad)
        {
            return new FondosRecaudadosActividadViewModel
            {
                IdFondosRecaudadosActividad = entidad.IdFondosRecaudadosActividad,
                IdActividad = entidad.IdActividad,
                Detalle = entidad.Detalle,
                Monto = entidad.Monto,
                FechaRegistro = entidad.FechaRegistro,
                FechaTextoRegistro = entidad.FechaRegistro?.ToString("yyyy-MM-dd")
            };
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var fondo = await _context.TbFondosRecaudadosActividads
                .Include(f => f.IdActividadNavigation)
                .FirstOrDefaultAsync(f => f.IdFondosRecaudadosActividad == id);

            if (fondo == null)
                return NotFound();

            var model = MapearFondosRecaudadosActividadViewModel(fondo);

            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "Nombre", model.IdActividad);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FondosRecaudadosActividadViewModel model)
        {
            if (id != model.IdFondosRecaudadosActividad)
                return NotFound();

            // Validar y asignar la fecha
            if (!ValidarYAsignarFechaRegistro(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de registro válida.";
                ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "Nombre", model.IdActividad);
                return View(model);
            }

            // Validación con FluentValidation
            var validator = new FondosRecaudadosActividadViewModelValidator();
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "Nombre", model.IdActividad);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var fondoExistente = await _context.TbFondosRecaudadosActividads.FindAsync(id);
                if (fondoExistente == null)
                    return NotFound();

                // Actualizar entidad con datos del modelo
                fondoExistente.IdActividad = model.IdActividad;
                fondoExistente.Detalle = model.Detalle;
                fondoExistente.Monto = model.Monto;
                fondoExistente.FechaRegistro = model.FechaRegistro;

                _context.Update(fondoExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Fondo recaudado actualizado correctamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el fondo: " + ex.Message;
                ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "Nombre", model.IdActividad);
                return View(model);
            }
        }


        // GET: TbFondosRecaudadosActividads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbFondosRecaudadosActividads == null)
            {
                return NotFound();
            }

            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads
                .Include(t => t.IdActividadNavigation)
                .FirstOrDefaultAsync(m => m.IdFondosRecaudadosActividad == id);
            if (tbFondosRecaudadosActividad == null)
            {
                return NotFound();
            }

            return View(tbFondosRecaudadosActividad);
        }

        // POST: TbFondosRecaudadosActividads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbFondosRecaudadosActividads == null)
            {
                return Problem("Entity set 'UcgdbContext.TbFondosRecaudadosActividads'  is null.");
            }
            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads.FindAsync(id);
            if (tbFondosRecaudadosActividad != null)
            {
                _context.TbFondosRecaudadosActividads.Remove(tbFondosRecaudadosActividad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbFondosRecaudadosActividadExists(int id)
        {
          return (_context.TbFondosRecaudadosActividads?.Any(e => e.IdFondosRecaudadosActividad == id)).GetValueOrDefault();
        }
    }
}
