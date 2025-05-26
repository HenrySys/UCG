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
    public class TbChequesController : Controller
    {
        private readonly UcgdbContext _context;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        public TbChequesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbCheques
        public async Task<IActionResult> Index()
        {
            IQueryable<TbCheque> filtroAsociacion = _context.TbCheques.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoAutorizaNavigation).Include(t => t.IdCuentaNavigation);
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
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los cheques.";
                return RedirectToAction("Error");
            }
            // var ucgdbContext = _context.TbCheques.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoAutorizaNavigation).Include(t => t.IdCuentaNavigation);
            // return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbCheques/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbCheques == null)
            {
                return NotFound();
            }

            var tbCheque = await _context.TbCheques
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoAutorizaNavigation)
                .Include(t => t.IdCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdCheque == id);
            if (tbCheque == null)
            {
                return NotFound();
            }

            return View(tbCheque);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ChequeViewModel();
            await ConfigurarAsociacionChequeAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChequeViewModel model)
        {
            // Parsear fechas desde texto
            var emisionOk = await ParseFechaEmisionAsync(model);
            var pagoOk = await ParseFechaPagoAsync(model);
            var cobroOk = await ParseFechaCobroAsync(model); // opcional
            var anulacionOk = await ParseFechaAnulacionAsync(model); // opcional

            if (!emisionOk || !pagoOk || !cobroOk || !anulacionOk)
            {
                TempData["ErrorMessage"] = "Verifique las fechas ingresadas.";
                await ConfigurarAsociacionChequeAsync(model);
                return View(model);
            }

            // Crear la entidad desde el ViewModel
            var cheque = MapearCheque(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Add(cheque);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Cheque creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el cheque.";
                await ConfigurarAsociacionChequeAsync(model);
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult ObtenerAsociadosPorAsociacion(int idAsociacion)
        {
            try
            {
                var asociados = _context.TbAsociados
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(a => new {
                        a.IdAsociado,
                        a.Nombre
                    })
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
        public JsonResult ObtenerCuentasPorAsociacion(int idAsociacion)
        {
            try
            {
                var cuentas = _context.TbCuenta
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(c => new
                    {
                        c.IdCuenta,
                        c.TituloCuenta
                    })
                    .ToList();

                if (!cuentas.Any())
                {
                    return Json(new { success = false, message = "No hay cuentas disponibles para esta asociación." });
                }

                return Json(new { success = true, data = cuentas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener las cuentas: " + ex.Message });
            }
        }


        private TbCheque MapearCheque(ChequeViewModel model)
        {
            return new TbCheque
            {
                IdCheque = model.IdCheque,
                IdAsociacion = model.IdAsociacion,
                IdCuenta = model.IdCuenta,
                NumeroCheque = model.NumeroCheque,
                FechaEmision = model.FechaEmision,
                FechaPago = model.FechaPago,
                FechaCobro = model.FechaCobro,
                FechaAnulacion = model.FechaAnulacion,
                Beneficiario = model.Beneficiario,
                Monto = model.Monto,
                Estado = model.Estado!.Value,
                Observaciones = model.Observaciones,
                IdAsociadoAutoriza = model.IdAsociadoAutoriza,
                MontoRestante = model.MontoRestante
            };
        }

        private async Task<bool> ParseFechaEmisionAsync(ChequeViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoEmision))
            {
                if (DateOnly.TryParseExact(model.FechaTextoEmision, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaEmision = fecha;
                    return true;
                }
                TempData["ErrorMessage"] = "Debe ingresar una fecha de emisión válida.";
                TempData.Keep("ErrorMessage");
                return false;
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de emisión.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        private async Task<bool> ParseFechaPagoAsync(ChequeViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoPago))
            {
                if (DateOnly.TryParseExact(model.FechaTextoPago, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaPago = fecha;
                    return true;
                }
                TempData["ErrorMessage"] = "Debe ingresar una fecha de pago válida.";
                TempData.Keep("ErrorMessage");
                return false;
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de pago.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        private async Task<bool> ParseFechaCobroAsync(ChequeViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoCobro))
            {
                if (DateOnly.TryParseExact(model.FechaTextoCobro, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaCobro = fecha;
                    return true;
                }
                TempData["ErrorMessage"] = "Debe ingresar una fecha de cobro válida.";
                TempData.Keep("ErrorMessage");
                return false;
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de cobro.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        private async Task<bool> ParseFechaAnulacionAsync(ChequeViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoAnulacion))
            {
                if (DateOnly.TryParseExact(model.FechaTextoAnulacion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaAnulacion = fecha;
                    return true;
                }
                TempData["ErrorMessage"] = "Debe ingresar una fecha de anulación válida.";
                TempData.Keep("ErrorMessage");
                return false;
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de anulación.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        private async Task ConfigurarAsociacionChequeAsync(ChequeViewModel model)
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

                    var cuentas = await _context.TbCuenta
                        .Where(c => c.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    ViewData["IdCuenta"] = new SelectList(cuentas, "IdCuenta", "TituloCuenta", model.IdCuenta);
                    ViewData["IdAsociadoAutoriza"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociadoAutoriza);
                }
            }
            else
            {
                ViewBag.EsAdmin = false;

                ViewData["IdAsociacion"] = new SelectList(
                    await _context.TbAsociacions.ToListAsync(),
                    "IdAsociacion", "Nombre", model.IdAsociacion);

                if (model.IdAsociacion > 0)
                {
                    var cuentas = await _context.TbCuenta
                        .Where(c => c.IdAsociacion == model.IdAsociacion)
                        .ToListAsync();

                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == model.IdAsociacion)
                        .ToListAsync();

                    ViewData["IdCuenta"] = new SelectList(cuentas, "IdCuenta", "TituloCuenta", model.IdCuenta);
                    ViewData["IdAsociadoAutoriza"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociadoAutoriza);
                }
                else
                {
                    ViewData["IdCuenta"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["IdAsociadoAutoriza"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbCheques == null)
            {
                return NotFound();
            }

            var tbCheque = await _context.TbCheques.FindAsync(id);
            if (tbCheque == null)
            {
                return NotFound();
            }

            var model = new ChequeViewModel
            {
                IdCheque = tbCheque.IdCheque,
                IdAsociacion = tbCheque.IdAsociacion,
                IdCuenta = tbCheque.IdCuenta,
                IdAsociadoAutoriza = tbCheque.IdAsociadoAutoriza,
                NumeroCheque = tbCheque.NumeroCheque,
                Beneficiario = tbCheque.Beneficiario,
                Monto = tbCheque.Monto,
                Estado = tbCheque.Estado,
                Observaciones = tbCheque.Observaciones,
                MontoRestante = tbCheque.MontoRestante,
                FechaTextoEmision = tbCheque.FechaEmision.ToString("yyyy-MM-dd"),
                FechaTextoPago = tbCheque.FechaPago.ToString("yyyy-MM-dd"),
                FechaTextoCobro = tbCheque.FechaCobro?.ToString("yyyy-MM-dd"),
                FechaTextoAnulacion = tbCheque.FechaAnulacion?.ToString("yyyy-MM-dd")
            };

            await ConfigurarAsociacionChequeAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChequeViewModel model)
        {
            if (id != model.IdCheque)
            {
                return NotFound();
            }

            var emisionOk = await ParseFechaEmisionAsync(model);
            var pagoOk = await ParseFechaPagoAsync(model);
            var cobroOk = await ParseFechaCobroAsync(model);
            var anulacionOk = await ParseFechaAnulacionAsync(model);

            if (!emisionOk || !pagoOk /*|| !cobroOk || !anulacionOk*/)
            {
                TempData["ErrorMessage"] = "Verifique las fechas ingresadas.";
                await ConfigurarAsociacionChequeAsync(model);
                return View(model);
            }

            var cheque = MapearCheque(model);

            try
            {
                _context.Update(cheque);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cheque actualizado correctamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TbChequeExists(cheque.IdCheque))
                {
                    return NotFound();
                }
                throw;
            }
        }


        // GET: TbCheques/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbCheques == null)
            {
                return NotFound();
            }

            var tbCheque = await _context.TbCheques
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoAutorizaNavigation)
                .Include(t => t.IdCuentaNavigation)
                .FirstOrDefaultAsync(m => m.IdCheque == id);
            if (tbCheque == null)
            {
                return NotFound();
            }

            return View(tbCheque);
        }

        // POST: TbCheques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbCheques == null)
            {
                return Problem("Entity set 'UcgdbContext.TbCheques'  is null.");
            }
            var tbCheque = await _context.TbCheques.FindAsync(id);
            if (tbCheque != null)
            {
                _context.TbCheques.Remove(tbCheque);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbChequeExists(int id)
        {
          return (_context.TbCheques?.Any(e => e.IdCheque == id)).GetValueOrDefault();
        }
    }
}
