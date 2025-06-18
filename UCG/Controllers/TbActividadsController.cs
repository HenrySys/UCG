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
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbActividadsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbActividadsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbActividads
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbActividads.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbActividads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbActividads == null)
            {
                return NotFound();
            }

            var tbActividad = await _context.TbActividads
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.TbFondosRecaudadosActividads)
                .FirstOrDefaultAsync(m => m.IdActividad == id);
            if (tbActividad == null)
            {
                return NotFound();
            }

            return View(tbActividad);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ActividadViewModel();
            await ConfigurarAsociacionActividadAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActividadViewModel model)
        {
            // Validar la fecha de la actividad
            var fechaOk = await ParseFechaActividadAsync(model);

            if (!fechaOk)
            {
                TempData["ErrorMessage"] = "Verifique la fecha ingresada.";
                await ConfigurarAsociacionActividadAsync(model);
                return View(model);
            }

            var actividad = MapearActividad(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Add(actividad);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Actividad creada correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar la actividad.";
                await ConfigurarAsociacionActividadAsync(model);
                return View(model);
            }
        }


        private TbActividad MapearActividad(ActividadViewModel model)
        {
            return new TbActividad
            {
                IdActividad = model.IdActividad,
                Nombre = model.Nombre,
                Fecha = model.Fecha, // DateOnly
                Razon = model.Razon,
                IdAsociacion = model.IdAsociacion,
                IdAsociado = model.IdAsociado,
                IdActa = model.IdActa,
                Lugar = model.Lugar,
                Observaciones = model.Observaciones,
                MontoTotalRecuadado = model.MontoTotalRecuadado

            };
        }

        private ActividadViewModel MapearActividadViewModel(TbActividad actividad)
        {
            return new ActividadViewModel
            {
                IdActividad = actividad.IdActividad,
                Nombre = actividad.Nombre,
                Fecha = actividad.Fecha,
                FechaTextoActividad = actividad.Fecha?.ToString("yyyy-MM-dd"),
                Razon = actividad.Razon,
                IdAsociacion = actividad.IdAsociacion,
                IdAsociado = actividad.IdAsociado,
                IdActa = actividad.IdActa,
                Lugar = actividad.Lugar,
                Observaciones = actividad.Observaciones,
                MontoTotalRecuadado = actividad.MontoTotalRecuadado

            };
        }


        private async Task ConfigurarAsociacionActividadAsync(ActividadViewModel actividad)
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

                    actividad.IdAsociacion = idAsociacion;
                    ViewBag.IdAsociacion = idAsociacion;
                    ViewBag.Nombre = nombre;
                    ViewBag.EsAdmin = true;

                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    var actas = await _context.TbActa
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", actividad.IdAsociado);
                    ViewData["IdActa"] = new SelectList(actas, "IdActa", "NumeroActa", actividad.IdActa); // Cambiá "Titulo" si tu campo de texto es otro
                }
            }
            else
            {
                ViewBag.EsAdmin = false;

                ViewData["IdAsociacion"] = new SelectList(
                    await _context.TbAsociacions.ToListAsync(),
                    "IdAsociacion", "Nombre", actividad.IdAsociacion);

                if (actividad.IdAsociacion > 0)
                {
                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == actividad.IdAsociacion)
                        .ToListAsync();

                    var actas = await _context.TbActa
                        .Where(a => a.IdAsociacion == actividad.IdAsociacion)
                        .ToListAsync();

                    ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", actividad.IdAsociado);
                    ViewData["IdActa"] = new SelectList(actas, "IdActa", "NumeroActa", actividad.IdActa);
                }
                else
                {
                    ViewData["IdAsociado"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["IdActa"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
        }

        private async Task<bool> ParseFechaActividadAsync(ActividadViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoActividad))
            {
                if (DateOnly.TryParseExact(model.FechaTextoActividad, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.Fecha = fecha;
                    return true;
                }
                TempData["ErrorMessage"] = "Debe ingresar una fecha válida.";
                TempData.Keep("ErrorMessage");
                return false;
            }

            TempData["ErrorMessage"] = "Debe ingresar una fecha.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        [HttpGet]
        public JsonResult ObtenerAsociadosPorAsociacion(int idAsociacion)
        {
            try
            {
                var asociados = _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => new
                    {
                        a.IdAsociado,
                        a.Nombre
                    })
                    .ToList();

                if (!asociados.Any())
                {
                    return Json(new { success = false, message = "No hay asociados disponibles para esta asociación." });
                }

                return Json(new { success = true, data = asociados });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los asociados: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerActasPorAsociacion(int idAsociacion)
        {
            try
            {
                var actas = _context.TbActa
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => new
                    {
                        a.IdActa,
                        a.NumeroActa
                    })
                    .ToList();

                if (!actas.Any())
                {
                    return Json(new { success = false, message = "No hay actas disponibles para esta asociación." });
                }

                return Json(new { success = true, data = actas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener las actas: " + ex.Message });
            }
        }




        // GET: TbActividads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbActividads == null)
                return NotFound();

            var actividad = await _context.TbActividads
                .Include(a => a.IdAsociacionNavigation)
                .Include(a => a.IdAsociadoNavigation)
                .Include(a => a.IdActaNavigation)
                .FirstOrDefaultAsync(a => a.IdActividad == id);

            if (actividad == null)
                return NotFound();

            var model = MapearActividadViewModel(actividad);



            await ConfigurarAsociacionActividadAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActividadViewModel model)
        {
            if (id != model.IdActividad)
                return NotFound();

            // Validar fecha
            var fechaOk = await ParseFechaActividadAsync(model);
            if (!fechaOk)
            {
                TempData["ErrorMessage"] = "Verifique la fecha ingresada.";
                await ConfigurarAsociacionActividadAsync(model);
                return View(model);
            }

            // Validar con FluentValidation si lo tenés implementado
            var validator = new ActividadViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación.";
                await ConfigurarAsociacionActividadAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var actividadExistente = await _context.TbActividads.FindAsync(id);
                if (actividadExistente == null)
                    return NotFound();

                // Mapear valores nuevos
                actividadExistente.Nombre = model.Nombre;
                actividadExistente.Fecha = model.Fecha;
                actividadExistente.Razon = model.Razon;
                actividadExistente.IdAsociacion = model.IdAsociacion;
                actividadExistente.IdAsociado = model.IdAsociado;
                actividadExistente.IdActa = model.IdActa;
                actividadExistente.Lugar = model.Lugar;
                actividadExistente.Observaciones = model.Observaciones;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Actividad actualizada correctamente.";
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar la actividad: " + ex.Message;
                await ConfigurarAsociacionActividadAsync(model);
                return View(model);
            }
        }


        // GET: TbActividads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbActividads == null)
            {
                return NotFound();
            }

            var tbActividad = await _context.TbActividads
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActividad == id);
            if (tbActividad == null)
            {
                return NotFound();
            }

            return View(tbActividad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbActividads == null)
            {
                return Problem("El conjunto de entidades 'TbActividads' es null.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var actividad = await _context.TbActividads.FindAsync(id);
                if (actividad == null)
                {
                    TempData["ErrorMessage"] = "La actividad no fue encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                _context.TbActividads.Remove(actividad);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Actividad eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Ocurrió un error al eliminar la actividad: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        private bool TbActividadExists(int id)
        {
          return (_context.TbActividads?.Any(e => e.IdActividad == id)).GetValueOrDefault();
        }
    }
}
