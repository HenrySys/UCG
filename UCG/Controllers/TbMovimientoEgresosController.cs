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
    public class TbMovimientoEgresosController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMovimientoEgresosController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMovimientoEgresos
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMovimientoEgresos.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMovimientoEgresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var tbMovimientoEgreso = await _context.TbMovimientoEgresos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoEgreso == id);
            if (tbMovimientoEgreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoEgreso);
        }

        public async Task<IActionResult> Create()
        {
            var model = new MovimientoEgresoViewModel();
            await ConfigurarAsociacionMovimientoEgresoAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoEgresoViewModel model)
        {
            // 1. Parsear y validar la fecha
            var fechaValida = await ParseFechaEmisionAsync(model);

            if (!fechaValida)
            {
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            // 2. Validar con FluentValidation si lo estás usando
            var validator = new MovimientoEgresoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores en el formulario.";
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            // 3. Guardar en transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var egreso = MapearMovimientoEgreso(model);
                _context.TbMovimientoEgresos.Add(egreso);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El movimiento de egreso fue registrado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar el movimiento: " + ex.Message;
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }
        }


        private async Task<bool> ParseFechaEmisionAsync(MovimientoEgresoViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoEgreso))
            {
                if (DateTime.TryParseExact(model.FechaTextoEgreso, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaEmision))
                {
                    model.Fecha = DateOnly.FromDateTime(fechaEmision);
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = "Debe ingresar una fecha de emisión válida (formato: yyyy-MM-dd).";
                    TempData.Keep("ErrorMessage");
                    return false;
                }
            }

            TempData["ErrorMessage"] = "Debe ingresar la fecha de emisión.";
            TempData.Keep("ErrorMessage");
            return false;
        }


        public TbMovimientoEgreso MapearMovimientoEgreso(MovimientoEgresoViewModel viewModel)
        {
            return new TbMovimientoEgreso
            {
                IdMovimientoEgreso = viewModel.IdMovimientoEgreso,
                IdAsociacion = viewModel.IdAsociacion,
                IdAsociado = viewModel.IdAsociado,
                IdActa = viewModel.IdActa,
                Monto = viewModel.Monto,
                Descripcion = viewModel.Descripcion,
                Fecha = viewModel.Fecha
            };
        }


        private async Task ConfigurarAsociacionMovimientoEgresoAsync(MovimientoEgresoViewModel model)
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            int? idAsociacion = model.IdAsociacion;

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                if (int.TryParse(idAsociacionClaim, out int parsedId))
                {
                    idAsociacion = parsedId;
                    model.IdAsociacion = parsedId;

                    var nombre = await _context.TbAsociacions
                        .Where(a => a.IdAsociacion == parsedId)
                        .Select(a => a.Nombre)
                        .FirstOrDefaultAsync();

                    ViewBag.IdAsociacion = parsedId;
                    ViewBag.Nombre = nombre;
                    ViewBag.EsAdmin = true;
                }
            }
            else
            {
                ViewBag.EsAdmin = false;
                var asociaciones = await _context.TbAsociacions.ToListAsync();
                ViewData["IdAsociacion"] = new SelectList(asociaciones, "IdAsociacion", "Nombre", idAsociacion);
            }

            if (idAsociacion.HasValue)
            {
                var asociados = await _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .ToListAsync();

                var actas = await _context.TbActa
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .ToListAsync();

                ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
                ViewData["IdActa"] = new SelectList(actas, "IdActa", "NumeroActa", model.IdActa);
 
            }
        }


        [HttpGet]
        public JsonResult ObtenerAsociadosPorAsociacion(int idAsociacion)
        {
            try
            {
                var asociados = _context.TbAsociados
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!asociados.Any())
                {
                    return Json(new { success = false, message = "No hay asociados disponibles." });
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
                var asociados = _context.TbActa
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!asociados.Any())
                {
                    return Json(new { success = false, message = "No hay actas disponibles." });
                }

                return Json(new { success = true, data = asociados });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los actas: " + ex.Message });
            }
        }


        [HttpGet]
        public JsonResult ObtenerAcuerdosPorActas(int idActa)
        {
            try
            {
                var asociados = _context.TbAcuerdos
                    .Where(c => c.IdActa == idActa)
                    .ToList();

                if (!asociados.Any())
                {
                    return Json(new { success = false, message = "No hay acuerdos disponibles." });
                }

                return Json(new { success = true, data = asociados });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los acuerdos: " + ex.Message });
            }
        }



        // GET: TbMovimientoEgresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var tbMovimientoEgreso = await _context.TbMovimientoEgresos.FindAsync(id);
            if (tbMovimientoEgreso == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimientoEgreso.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoEgreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoEgreso.IdAsociado);
            return View(tbMovimientoEgreso);
        }

        // POST: TbMovimientoEgresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimientoEgreso,IdAsociacion,IdAsociado,IdConceptoAsociacion,IdActa,Monto,Fecha,Descripcion")] TbMovimientoEgreso tbMovimientoEgreso)
        {
            if (id != tbMovimientoEgreso.IdMovimientoEgreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMovimientoEgreso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMovimientoEgresoExists(tbMovimientoEgreso.IdMovimientoEgreso))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimientoEgreso.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoEgreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoEgreso.IdAsociado);
            return View(tbMovimientoEgreso);
        }

        // GET: TbMovimientoEgresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var tbMovimientoEgreso = await _context.TbMovimientoEgresos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoEgreso == id);
            if (tbMovimientoEgreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoEgreso);
        }

        // POST: TbMovimientoEgresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMovimientoEgresos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbMovimientoEgresos'  is null.");
            }
            var tbMovimientoEgreso = await _context.TbMovimientoEgresos.FindAsync(id);
            if (tbMovimientoEgreso != null)
            {
                _context.TbMovimientoEgresos.Remove(tbMovimientoEgreso);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMovimientoEgresoExists(int id)
        {
          return (_context.TbMovimientoEgresos?.Any(e => e.IdMovimientoEgreso == id)).GetValueOrDefault();
        }
    }
}
