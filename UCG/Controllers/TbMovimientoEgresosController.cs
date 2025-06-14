using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                .Include(m => m.TbDetalleChequeFacturas)
                .ThenInclude(d => d.IdChequeNavigation)
                .Include(m => m.TbDetalleChequeFacturas)
                .ThenInclude(d => d.IdFacturaNavigation)
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
            if (!await ParseFechaEmisionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha válida de egreso.";
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            // 2. Deserializar JSON de detalles
            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los detalles de cheques y facturas.";
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            // 3. Validar con FluentValidation
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

            // 4. Crear la transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 5. Mapear y guardar el movimiento
                var egreso = MapearMovimientoEgreso(model);
                _context.TbMovimientoEgresos.Add(egreso);
                await _context.SaveChangesAsync();

                // 6. Guardar los detalles de cheque/factura
                await GuardarDetallesChequeFacturasAsync(model.DetalleChequeFacturaEgreso, egreso.IdMovimientoEgreso);
                await _context.SaveChangesAsync();

                // 7. Confirmar
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "El movimiento de egreso fue creado exitosamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar el movimiento de egreso: " + ex.Message;
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

        private async Task<bool> DeserializarJsonAsync(MovimientoEgresoViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.DetalleChequeFacturaEgresoJason))
                    model.DetalleChequeFacturaEgreso = JsonConvert.DeserializeObject<List<DetalleChequeFacturaViewModel>>(model.DetalleChequeFacturaEgresoJason);

               
                model.DetalleChequeFacturaEgreso ??= new();
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al deserializar los datos: " + ex.Message;
                return false;
            }
        }

        private async Task GuardarDetallesChequeFacturasAsync(List<DetalleChequeFacturaViewModel> chequeFacturas, int IdMovimientoEgreso)
        {
            foreach (var chequeFactura in chequeFacturas)
            {
                _context.TbDetalleChequeFacturas.Add(new TbDetalleChequeFactura
                {
                    IdMovimientoEgreso = IdMovimientoEgreso,
                    IdAcuerdo = chequeFactura.IdAcuerdo,
                    IdCheque = chequeFactura.IdCheque,
                    IdFactura = chequeFactura.IdFactura,
                    Monto = chequeFactura.Monto,
                    Observacion = chequeFactura.Descripcion

                });
                
            }
        }
        private MovimientoEgresoViewModel MapearMovimientoEgresoViewModel(TbMovimientoEgreso egreso)
        {
            return new MovimientoEgresoViewModel
            {
                IdMovimientoEgreso = egreso.IdMovimientoEgreso,
                IdAsociacion = egreso.IdAsociacion,
                IdAsociado = egreso.IdAsociado,
                IdActa = egreso.IdActa,
                Monto = egreso.Monto,
                Descripcion = egreso.Descripcion,
                Fecha = egreso.Fecha, // Asumiendo que en BD es DateTime
                FechaTextoEgreso = egreso.Fecha.ToString("yyyy-MM-dd"),
                DetalleChequeFacturaEgreso = egreso.TbDetalleChequeFacturas.Select(d => new DetalleChequeFacturaViewModel
                {
                    IdDetalleChequeFactura = d.IdDetalleChequeFactura,
                    IdMovimientoEgreso = d.IdMovimientoEgreso,
                    IdAcuerdo = d.IdAcuerdo,
                    IdCheque = d.IdCheque,
                    IdFactura = d.IdFactura,
                    NumeroCheque = d.IdChequeNavigation?.NumeroCheque,
                    NumeroFactura = d.IdFacturaNavigation?.NumeroFactura,
                    NumeroAcuerdo = d.IdAcuerdoNavigation?.NumeroAcuerdo,
                    Monto = d.Monto,
                    Descripcion = d.Observacion // <== CAMBIAR AQUI
                }).ToList(),
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
        public JsonResult ObtenerFacturasPorAsociacion(int idAsociacion)
        {
            try
            {
                var facturas = _context.TbFacturas
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!facturas.Any())
                {
                    return Json(new { success = false, message = "No hay facturas disponibles." });
                }

                return Json(new { success = true, data = facturas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los facturas: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerChequesPorAsociacion(int idAsociacion)
        {
            try
            {
                var cheques = _context.TbCheques
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!cheques.Any())
                {
                    return Json(new { success = false, message = "No hay cheques disponibles." });
                }

                return Json(new { success = true, data = cheques });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los cheques: " + ex.Message });
            }
        }


        [HttpGet]
        public JsonResult ObtenerActasPorAsociacion(int idAsociacion)
        {
            try
            {
                var actas = _context.TbActa
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!actas.Any())
                {
                    return Json(new { success = false, message = "No hay actas disponibles." });
                }

                return Json(new { success = true, data = actas });
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
                var acuerdos = _context.TbAcuerdos
                    .Where(c => c.IdActa == idActa)
                    .ToList();

                if (!acuerdos.Any())
                {
                    return Json(new { success = false, message = "No hay acuerdos disponibles." });
                }

                return Json(new { success = true, data = acuerdos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los acuerdos: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetMontoCheque(int id)
        {
            var cheque = _context.TbCheques
                .Where(c => c.IdCheque == id)
                .Select(c => new {
                    c.Monto,
                    c.MontoRestante
                })
                .FirstOrDefault();

            if (cheque == null)
                return NotFound();

            return Json(cheque);
        }

        [HttpGet]
        public IActionResult GetMontoFactura(int id)
        {
            var factura = _context.TbFacturas
                .Where(c => c.IdFactura == id)
                .Select(c => new {
                    c.MontoTotal,
                })
                .FirstOrDefault();

            if (factura == null)
                return NotFound();

            return Json(factura);
        }

        [HttpGet]
        public JsonResult VerificarDatosAsociacion(int idAsociacion)
        {
            try
            {
                var tieneCheques = _context.TbCheques.Any(c => c.IdAsociacion == idAsociacion);
                var tieneFacturas = _context.TbFacturas.Any(f => f.IdAsociacion == idAsociacion);

                return Json(new
                {
                    success = true,
                    tieneCheques,
                    tieneFacturas
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = true,
                    message = "Error al verificar los datos de la asociación: " + ex.Message
                });
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var egreso = await _context.TbMovimientoEgresos
                .Include(e => e.TbDetalleChequeFacturas)
                    .ThenInclude(d => d.IdChequeNavigation)
                .Include(e => e.TbDetalleChequeFacturas)
                    .ThenInclude(d => d.IdFacturaNavigation)
                .FirstOrDefaultAsync(e => e.IdMovimientoEgreso == id);

            if (egreso == null)
                return NotFound();

            var model = MapearMovimientoEgresoViewModel(egreso);
            model.DetalleChequeFacturaEgresoJason = JsonConvert.SerializeObject(model.DetalleChequeFacturaEgreso);

            await ConfigurarAsociacionMovimientoEgresoAsync(model);

            // 🔽 Cargá los combos del modal de forma global
            ViewBag.IdAcuerdo = new SelectList(
                await _context.TbAcuerdos
                    .Where(a => a.IdActa == model.IdActa)
                    .ToListAsync(),
                "IdAcuerdo", "NumeroAcuerdo");

            ViewBag.IdCheque = new SelectList(
                await _context.TbCheques
                    .Where(c => c.IdAsociacion == model.IdAsociacion)
                    .ToListAsync(),
                "IdCheque", "NumeroCheque");

            ViewBag.IdFactura = new SelectList(
                await _context.TbFacturas
                    .Where(f => f.IdAsociacion == model.IdAsociacion)
                    .ToListAsync(),
                "IdFactura", "NumeroFactura");

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovimientoEgresoViewModel model)
        {
            if (id != model.IdMovimientoEgreso)
                return NotFound();

            if (!await ParseFechaEmisionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha válida de egreso.";
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los detalles de cheques y facturas.";
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            var validator = new MovimientoEgresoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var egresoExistente = await _context.TbMovimientoEgresos
                    .Include(e => e.TbDetalleChequeFacturas)
                    .FirstOrDefaultAsync(e => e.IdMovimientoEgreso == id);

                if (egresoExistente == null)
                    return NotFound();

                // Actualizar campos principales
                egresoExistente.IdAsociacion = model.IdAsociacion;
                egresoExistente.IdAsociado = model.IdAsociado;
                egresoExistente.IdActa = model.IdActa;
                egresoExistente.Fecha = model.Fecha;
                egresoExistente.Monto = model.Monto;
                egresoExistente.Descripcion = model.Descripcion;

                // Reemplazar detalles anteriores
                _context.TbDetalleChequeFacturas.RemoveRange(egresoExistente.TbDetalleChequeFacturas);
                await GuardarDetallesChequeFacturasAsync(model.DetalleChequeFacturaEgreso, id);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El movimiento de egreso fue actualizado exitosamente.";
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el movimiento: " + ex.Message;
                await ConfigurarAsociacionMovimientoEgresoAsync(model);
                return View(model);
            }
        }


        // GET: TbMovimientoEgresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.TbMovimientoEgresos
                .Include(m => m.IdAsociacionNavigation)
                .Include(m => m.IdAsociadoNavigation)
                .Include(m => m.IdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoEgreso == id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // POST: TbMovimientoEgresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var movimiento = await _context.TbMovimientoEgresos
                    .Include(m => m.TbDetalleChequeFacturas)
                    .FirstOrDefaultAsync(m => m.IdMovimientoEgreso == id);

                if (movimiento == null)
                {
                    TempData["ErrorMessage"] = "El movimiento no fue encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Primero eliminá los detalles
                if (movimiento.TbDetalleChequeFacturas.Any())
                {
                    _context.TbDetalleChequeFacturas.RemoveRange(movimiento.TbDetalleChequeFacturas);
                }

                // Luego el movimiento
                _context.TbMovimientoEgresos.Remove(movimiento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El movimiento fue eliminado correctamente.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al eliminar el movimiento: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        private bool TbMovimientoEgresoExists(int id)
        {
          return (_context.TbMovimientoEgresos?.Any(e => e.IdMovimientoEgreso == id)).GetValueOrDefault();
        }
    }
}
