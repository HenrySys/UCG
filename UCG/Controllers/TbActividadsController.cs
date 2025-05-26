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
                Observaciones = model.Observaciones
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
                    ViewData["IdActa"] = new SelectList(actas, "IdActa", "Titulo", actividad.IdActa); // Cambiá "Titulo" si tu campo de texto es otro
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
                    ViewData["IdActa"] = new SelectList(actas, "IdActa", "Titulo", actividad.IdActa);
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
            {
                return NotFound();
            }

            var tbActividad = await _context.TbActividads.FindAsync(id);
            if (tbActividad == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbActividad.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActividad.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActividad.IdAsociado);
            return View(tbActividad);
        }

        // POST: TbActividads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActividad,Nombre,Fecha,Razon,IdAsociacion,IdAsociado,IdActa,Lugar,Observaciones")] TbActividad tbActividad)
        {
            if (id != tbActividad.IdActividad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbActividad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbActividadExists(tbActividad.IdActividad))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbActividad.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActividad.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActividad.IdAsociado);
            return View(tbActividad);
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

        // POST: TbActividads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbActividads == null)
            {
                return Problem("Entity set 'UcgdbContext.TbActividads'  is null.");
            }
            var tbActividad = await _context.TbActividads.FindAsync(id);
            if (tbActividad != null)
            {
                _context.TbActividads.Remove(tbActividad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbActividadExists(int id)
        {
          return (_context.TbActividads?.Any(e => e.IdActividad == id)).GetValueOrDefault();
        }
    }
}
