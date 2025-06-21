using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public class TbMovimientoIngresosController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMovimientoIngresosController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMovimientoIngresos
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMovimientoIngresos.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMovimientoIngresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMovimientoIngresos == null)
            {
                return NotFound();
            }

            var tbMovimientoIngreso = await _context.TbMovimientoIngresos
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(m => m.TbDocumentoIngresos)
                .FirstOrDefaultAsync(m => m.IdMovimientoIngreso == id);
            if (tbMovimientoIngreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoIngreso);
        }

        // GET: TbMovimientoIngresos/Create
        public async Task<IActionResult> Create()
        {
            var model = new MovimientoIngresoViewModel();
            await ConfigurarAsociacionMovimientoIngresoAsync(model);
            return View(model);
        }


        // POST: TbMovimientoIngresos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoIngresoViewModel model)
        {
            // 1. Validar y convertir fecha
            if (!await ParseFechaIngresoAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha válida de ingreso.";
                await ConfigurarAsociacionMovimientoIngresoAsync(model);
                return View(model);
            }

            // 2. Deserializar detalles JSON
            if (!await DeserializarJsonDocumentosAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los detalles de documentos de ingreso.";
                await ConfigurarAsociacionMovimientoIngresoAsync(model);
                return View(model);
            }

            // 3. Validar con FluentValidation (opcional)
            var validator = new MovimientoIngresoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores en el formulario.";
                await ConfigurarAsociacionMovimientoIngresoAsync(model);
                return View(model);
            }

            // 4. Transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 5. Guardar MovimientoIngreso
                var ingreso = MapearMovimientoIngreso(model);
                _context.TbMovimientoIngresos.Add(ingreso);
                await _context.SaveChangesAsync();

                // 6. Guardar documentos
                await GuardarDocumentosIngresoAsync(model.DetalleDocumentoIngreso, ingreso.IdMovimientoIngreso);
                await _context.SaveChangesAsync();

                // 7. Confirmar
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "El movimiento de ingreso fue creado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar el movimiento de ingreso: " + ex.Message;
                await ConfigurarAsociacionMovimientoIngresoAsync(model);
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


        // GET: TbMovimientoIngresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMovimientoIngresos == null)
            {
                return NotFound();
            }

            var tbMovimientoIngreso = await _context.TbMovimientoIngresos.FindAsync(id);
            if (tbMovimientoIngreso == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoIngreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoIngreso.IdAsociado);
            return View(tbMovimientoIngreso);
        }

        // POST: TbMovimientoIngresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimientoIngreso,IdAsociacion,IdAsociado,FechaIngreso,Descripcion,MontoTotalIngresado")] TbMovimientoIngreso tbMovimientoIngreso)
        {
            if (id != tbMovimientoIngreso.IdMovimientoIngreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMovimientoIngreso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMovimientoIngresoExists(tbMovimientoIngreso.IdMovimientoIngreso))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoIngreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoIngreso.IdAsociado);
            return View(tbMovimientoIngreso);
        }

        // GET: TbMovimientoIngresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientoIngresos == null)
            {
                return NotFound();
            }

            var tbMovimientoIngreso = await _context.TbMovimientoIngresos
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoIngreso == id);
            if (tbMovimientoIngreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoIngreso);
        }

        // POST: TbMovimientoIngresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMovimientoIngresos == null)
            {
                TempData["ErrorMessage"] = "El movimiento Ingreso es Nulo.";
                return Problem("Entity set 'UcgdbContext.TbMovimientoIngresos'  is null.");
            }
            var tbMovimientoIngreso = await _context.TbMovimientoIngresos.FindAsync(id);
            if (tbMovimientoIngreso != null)
            {
                _context.TbMovimientoIngresos.Remove(tbMovimientoIngreso);
                TempData["SuccessMessage"] = "El movimiento de ingreso fue eliminado exitosamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMovimientoIngresoExists(int id)
        {
          return (_context.TbMovimientoIngresos?.Any(e => e.IdMovimientoIngreso == id)).GetValueOrDefault();
        }

        private async Task ConfigurarAsociacionMovimientoIngresoAsync(MovimientoIngresoViewModel model)
        {
            var asociaciones = await _context.TbAsociacions.ToListAsync();
            var asociados = await _context.TbAsociados.ToListAsync();
            var conceptos = await _context.TbConceptoAsociacions
                .Include(c => c.IdConceptoNavigation)
                .Where(c => c.IdConceptoNavigation!.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Ingreso)
                .ToListAsync();
            var donantes = await _context.TbClientes.ToListAsync();
            var actividades = await _context.TbActividads.ToListAsync();
            var financistas = await _context.TbFinancista.ToListAsync();

            ViewData["IdAsociacion"] = new SelectList(asociaciones, "IdAsociacion", "Nombre", null);
            ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
            ViewData["IdConceptoAsociacion"] = new SelectList(conceptos, "IdConceptoAsociacion", "DescripcionPersonalizada",null);
            ViewData["IdDonante"] = new SelectList(donantes, "IdCliente", "Nombre", null);
            ViewData["IdActividad"] = new SelectList(actividades, "IdActividad", "Nombre", null);
            ViewData["IdFinancista"] = new SelectList(financistas, "IdFinancista", "Nombre", null);

        }


        private async Task<bool> ParseFechaIngresoAsync(MovimientoIngresoViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoIngreso))
            {
                if (DateTime.TryParseExact(model.FechaTextoIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaIngreso))
                {
                    model.FechaIngreso = DateOnly.FromDateTime(fechaIngreso);
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = "Debe ingresar una fecha válida (formato: yyyy-MM-dd).";
                    TempData.Keep("ErrorMessage");
                    return false;
                }
            }

            TempData["ErrorMessage"] = "Debe ingresar la fecha.";
            TempData.Keep("ErrorMessage");
            return false;
        }

        private async Task<bool> DeserializarJsonDocumentosAsync(MovimientoIngresoViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.DetalleDocumentoIngresoJson))
                    model.DetalleDocumentoIngreso = JsonConvert.DeserializeObject<List<DocumentoIngresoViewModel>>(model.DetalleDocumentoIngresoJson);

                model.DetalleDocumentoIngreso ??= new();
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al deserializar los datos de documentos: " + ex.Message;
                return false;
            }
        }

        private TbMovimientoIngreso MapearMovimientoIngreso(MovimientoIngresoViewModel model)
        {
            return new TbMovimientoIngreso
            {
                IdAsociacion = model.IdAsociacion,
                IdAsociado = model.IdAsociado,
                FechaIngreso = model.FechaIngreso,
                Descripcion = model.Descripcion,
                MontoTotalIngresado = model.MontoTotalIngresado
            };
        }

        private async Task GuardarDocumentosIngresoAsync(List<DocumentoIngresoViewModel> documentos, int idMovimientoIngreso)
        {
            foreach (var doc in documentos)
            {
                _context.TbDocumentoIngresos.Add(new TbDocumentoIngreso
                {
                    IdMovimientoIngreso = idMovimientoIngreso,
                    IdConceptoAsociacion = doc.IdConceptoAsociacion,
                    IdCuenta = doc.IdCuenta,
                    IdCliente = doc.IdCliente,
                    IdFinancista = doc.IdFinancista,
                    IdActividad = doc.IdActividad,
                    NumComprobante = doc.NumComprobante,
                    FechaComprobante = DateOnly.ParseExact(doc.FechaComprobante, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Descripcion = doc.Descripcion,
                    Monto = doc.Monto,
                    MetodoPago = (TbDocumentoIngreso.MetodoDePago?)doc.MetodoPago
                });
            }
        }

        [HttpGet]
        public JsonResult ObtenerConceptosPorAsociacion(int idAsociacion)
        {
            try
            {
                var conceptos = _context.TbConceptoAsociacions
                    .Include(ca => ca.IdConceptoNavigation)
                    .Where(ca =>
                        ca.IdAsociacion == idAsociacion &&
                        ca.IdConceptoNavigation != null &&
                        ca.IdConceptoNavigation.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Ingreso
                    )
                    .Select(ca => new
                    {
                        idConceptoAsociacion = ca.IdConceptoAsociacion,
                        descripcionPersonalizada = ca.DescripcionPersonalizada ?? "(Sin descripción)",
                        tipoOrigen = ca.IdConceptoNavigation!.TipoOrigenIngreso!.ToString()

                    })
                    .ToList();

                if (!conceptos.Any())
                    return Json(new { success = false, message = "No hay conceptos de ingreso disponibles." });

                return Json(new { success = true, data = conceptos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener conceptos: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerClientesPorAsociacion(int idAsociacion)
        {
            try
            {
                var clientes = _context.TbClientes
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(c => new
                    {
                        c.IdCliente,
                        c.Nombre
                    })
                    .ToList();

                if (!clientes.Any())
                    return Json(new { success = false, message = "No hay clientes disponibles." });

                return Json(new { success = true, data = clientes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los clientes: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerActividadesPorAsociacion(int idAsociacion)
        {
            try
            {
                var actividades = _context.TbActividads
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => new
                    {
                        a.IdActividad,
                        a.Nombre
                    })
                    .ToList();

                if (!actividades.Any())
                    return Json(new { success = false, message = "No hay actividades disponibles." });

                return Json(new { success = true, data = actividades });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener actividades: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerFinancistasPorAsociacion(int idAsociacion)
        {
            try
            {
                var financistas = _context.TbFinancista
                    .Where(f => f.IdAsociacion == idAsociacion)
                    .Select(f => new
                    {
                        f.IdFinancista,
                        f.Nombre
                    })
                    .ToList();

                if (!financistas.Any())
                    return Json(new { success = false, message = "No hay financistas disponibles." });

                return Json(new { success = true, data = financistas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener financistas: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerMontoActividad(int idActividad)
        {
            var monto = _context.TbActividads
                .Where(a => a.IdActividad == idActividad)
                .Select(a => a.MontoTotalRecuadado) // o el campo correspondiente
                .FirstOrDefault();

            return Json(new { success = true, monto = monto });
        }

    }

}
