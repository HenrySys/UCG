using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbFacturasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbFacturasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbFacturas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbFacturas.Include(t => t.IdAsociacionNavigation).Include(t => t.IdConceptoAsociacionNavigation).Include(t => t.IdAsociadoNavigation).Include(t => t.IdColaboradorNavigation).Include(t => t.IdProveedorNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbFacturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbFacturas == null)
            {
                return NotFound();
            }

            var tbFactura = await _context.TbFacturas
                .Include(f => f.IdAsociacionNavigation)
                .Include(f => f.IdConceptoAsociacionNavigation)
                .Include(f => f.IdAsociadoNavigation)
                .Include(f => f.IdColaboradorNavigation)
                .Include(f => f.IdProveedorNavigation)
                .Include(f => f.TbDetalleFacturas) // INCLUIR LOS DETALLES DE LA FACTURA
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (tbFactura == null)
            {
                return NotFound();
            }

            return View(tbFactura);
        }


        public async Task<IActionResult> Create()
        {
            var model = new FacturaViewModel();
            await ConfigurarAsociacionFacturaAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacturaViewModel model)
        {
            // 1. Parsear fecha
            var fechaEmisionValida = await ParseFechaEmisionAsync(model);
            if (!fechaEmisionValida)
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de emisión válida.";
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // 2. Deserializar detalles de factura
            if (!await DeserializarDetalleFacturaJsonAsync(model))
            {
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // 3. Validar modelo con FluentValidation
            var validator = new FacturaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // 4. Guardar en transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Guardar factura principal
                var factura = MapearFactura(model);
                _context.TbFacturas.Add(factura);
                await _context.SaveChangesAsync();

                // Guardar detalles relacionados
                await GuardarDetallesFacturaAsync(model.DetalleFactura, factura.IdFactura);

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "La factura fue creada exitosamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar la factura: " + ex.Message;
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }
        }


        private async Task ConfigurarAsociacionFacturaAsync(FacturaViewModel model)
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

                var conceptos = await _context.TbConceptoAsociacions
                     .Include(ca => ca.IdConceptoNavigation)
                     .Where(ca =>
                         ca.IdAsociacion == idAsociacion &&
                         ca.IdConceptoNavigation != null &&
                         ca.IdConceptoNavigation.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Egreso &&
                         ca.IdConceptoNavigation.TipoEmisorEgreso != null
                     ).ToListAsync();

                var colaboradores = await _context.TbColaboradors.Where(a => a.IdAsociacion == idAsociacion)
                    .ToListAsync();

                var proveedores = await _context.TbProveedors.Where(a => a.IdAsociacion == idAsociacion)
                    .ToListAsync();


                ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
                ViewData["IdConceptoAsociacion"] = new SelectList(conceptos, "IdConceptoAsociacion", "DescripcionPersonalizada", model.IdConceptoAsociacion);
                ViewData["IdColaborador"] = new SelectList(colaboradores, "IdColaborador", "Nombre", model.IdColaborador);
                ViewData["IdProveedor"] = new SelectList(proveedores, "IdProveedor", "NombreEmpresa", model.IdProveedor);

                if (model.IdConceptoAsociacion.HasValue)
                {
                    var tipoEmisor = await _context.TbConceptoAsociacions
                        .Where(c => c.IdConceptoAsociacion == model.IdConceptoAsociacion.Value)
                        .Select(c => c.IdConceptoNavigation.TipoEmisorEgreso)
                        .FirstOrDefaultAsync();

                    model.TipoEmisor = tipoEmisor?.ToString();
                }
            }
        }
        private async Task<bool> DeserializarDetalleFacturaJsonAsync(FacturaViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.DetalleFacturaJason))
                {
                    model.DetalleFactura = JsonConvert.DeserializeObject<List<DetalleFacturaViewModel>>(model.DetalleFacturaJason);
                }

                model.DetalleFactura ??= new();

                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al deserializar los detalles de factura: " + ex.Message;
                return false;
            }
        }




        private TbFactura MapearFactura(FacturaViewModel model)
        {
            return new TbFactura
            {
                IdFactura = model.IdFactura,
                NumeroFactura = model.NumeroFactura,
                FechaEmision = model.FechaEmision,               
                Descripcion = model.Descripcion,
                IdConceptoAsociacion = model.IdConceptoAsociacion,
                IdColaborador = model.IdColaborador,
                IdProveedor = model.IdProveedor,
                IdAsociacion = model.IdAsociacion,
                IdAsociado = model.IdAsociado,
                MontoTotal = model.MontoTotal,
                FechaSubida = model.FechaSubida,
                Subtotal = model.Subtotal,
                TotalIva = model.TotalIva           
            };
        
        }
        private async Task GuardarDetallesFacturaAsync(List<DetalleFacturaViewModel> detalles, int idFactura)
        {
            foreach (var detalle in detalles)
            {
                var nuevoDetalle = new TbDetalleFactura
                {
                    IdFactura = idFactura,
                    Descripcion = detalle.Descripcion,
                    Unidad = detalle.Unidad,
                    Cantidad = detalle.Cantidad,
                    PorcentajeIva = detalle.PorcentajeIva,
                    PrecioUnitario = detalle.PrecioUnitario,
                    PorcentajeDescuento = detalle.PorcentajeDescuento,
                    Descuento = detalle.Descuento,
                    MontoIva = detalle.MontoIva,
                    BaseImponible = detalle.BaseImponible,
                    TotalLinea = detalle.TotalLinea
                };

                _context.TbDetalleFacturas.Add(nuevoDetalle);
            }

            await _context.SaveChangesAsync();
        }



        private async Task<bool> ParseFechaEmisionAsync(FacturaViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoEmision))
            {
                if (DateTime.TryParseExact(model.FechaTextoEmision, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaEmision))
                {
                    model.FechaEmision = DateOnly.FromDateTime(fechaEmision);
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
        public JsonResult ObtenerConceptosPorAsociacion(int idAsociacion)
        {
            try
            {
                var conceptos = _context.TbConceptoAsociacions
                    .Include(ca => ca.IdConceptoNavigation)
                    .Where(ca =>
                        ca.IdAsociacion == idAsociacion &&
                        ca.IdConceptoNavigation != null &&
                        ca.IdConceptoNavigation.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Egreso &&
                        ca.IdConceptoNavigation.TipoEmisorEgreso != null
                    )
                    .Select(ca => new
                    {
                        idConceptoAsociacion = ca.IdConceptoAsociacion,
                        descripcionPersonalizada = ca.DescripcionPersonalizada ?? "(Sin descripción)",
                        tipoEmisor = ca.IdConceptoNavigation!.TipoEmisorEgreso!.ToString()
                    })
                    .ToList();

                if (!conceptos.Any())
                {
                    return Json(new { success = false, message = "No hay conceptos disponibles para esta asociación." });
                }

                return Json(new { success = true, data = conceptos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los conceptos: " + ex.Message });
            }
        }


        [HttpGet]
        public JsonResult ObtenerProveedoresPorAsociacion(int idAsociacion)
        {
            try
            {
                var conceptos = _context.TbProveedors
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(c => new
                    {
                        c.IdProveedor,
                        c.NombreEmpresa
                    })
                    .ToList();

                if (!conceptos.Any())
                {
                    return Json(new { success = false, message = "No hay proveedores disponibles para esta asociación." });
                }

                return Json(new { success = true, data = conceptos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los proveedores: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerColaboradoresPorAsociacion(int idAsociacion)
        {
            try
            {
                var conceptos = _context.TbColaboradors
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(c => new
                    {
                        c.IdColaborador,
                        c.Nombre
                    })
                    .ToList();

                if (!conceptos.Any())
                {
                    return Json(new { success = false, message = "No hay colaboradores disponibles para esta asociación." });
                }

                return Json(new { success = true, data = conceptos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los colaboradores: " + ex.Message });
            }
        }


        private FacturaViewModel MapearFacturaViewModel(TbFactura factura)
        {
            return new FacturaViewModel
            {
                IdFactura = factura.IdFactura,
                NumeroFactura = factura.NumeroFactura,
                FechaEmision = factura.FechaEmision,
                FechaTextoEmision = factura.FechaEmision.ToString("yyyy-MM-dd"),
                Descripcion = factura.Descripcion,
                IdConceptoAsociacion = factura.IdConceptoAsociacion,
                IdColaborador = factura.IdColaborador,
                IdProveedor = factura.IdProveedor,
                IdAsociacion = factura.IdAsociacion,
                IdAsociado = factura.IdAsociado,
                MontoTotal = factura.MontoTotal,

                DetalleFactura = factura.TbDetalleFacturas?.Select(d => new DetalleFacturaViewModel
                {
                    IdDetalleFactura = d.IdDetalleFactura,
                    Descripcion = d.Descripcion,
                    Unidad = d.Unidad,
                    Cantidad = d.Cantidad,
                    PorcentajeIva = d.PorcentajeIva,
                    PrecioUnitario = d.PrecioUnitario,
                    PorcentajeDescuento = d.PorcentajeDescuento,
                    Descuento = d.Descuento,
                    MontoIva = d.MontoIva,
                    BaseImponible = d.BaseImponible,
                    TotalLinea = d.TotalLinea
                }).ToList() ?? new List<DetalleFacturaViewModel>()
            };
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbFacturas == null)
                return NotFound();

            var tbFactura = await _context.TbFacturas
                .Include(f => f.IdAsociacionNavigation)
                .Include(f => f.IdAsociadoNavigation)
                .Include(f => f.IdColaboradorNavigation)
                .Include(f => f.IdProveedorNavigation)
                .Include(f => f.TbDetalleFacturas)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (tbFactura == null)
                return NotFound();

            var model = MapearFacturaViewModel(tbFactura);

            // ESTA LÍNEA FALTABA
            model.DetalleFacturaJason = JsonConvert.SerializeObject(model.DetalleFactura);

            await ConfigurarAsociacionFacturaAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idFactura, FacturaViewModel model)
        {
            if (idFactura != model.IdFactura)
                return NotFound();

            // Parsear fecha desde texto
            if (!await ParseFechaEmisionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de emisión válida.";
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // Deserializar detalles de factura desde JSON
            if (!await DeserializarDetalleFacturaJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de los detalles de factura.";
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // Validación con FluentValidation
            var validator = new FacturaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var facturaExistente = await _context.TbFacturas
                    .Include(f => f.TbDetalleFacturas)
                    .FirstOrDefaultAsync(f => f.IdFactura == idFactura);

                if (facturaExistente == null)
                    return NotFound();

                // Actualizar campos principales
                facturaExistente.NumeroFactura = model.NumeroFactura;
                facturaExistente.FechaEmision = model.FechaEmision;
                facturaExistente.Descripcion = model.Descripcion;
                facturaExistente.IdConceptoAsociacion = model.IdConceptoAsociacion;
                facturaExistente.IdColaborador = model.IdColaborador;
                facturaExistente.IdProveedor = model.IdProveedor;
                facturaExistente.IdAsociacion = model.IdAsociacion;
                facturaExistente.IdAsociado = model.IdAsociado;
                facturaExistente.MontoTotal = model.MontoTotal;
                facturaExistente.Estado = model.Estado;
                facturaExistente.ArchivoUrl = model.ArchivoUrl;
                facturaExistente.NombreArchivo = model.NombreArchivo;
                facturaExistente.FechaSubida = model.FechaSubida;
                facturaExistente.Subtotal = model.Subtotal;
                facturaExistente.TotalIva = model.TotalIva;

                // Reemplazar detalles
                _context.TbDetalleFacturas.RemoveRange(facturaExistente.TbDetalleFacturas);
                await GuardarDetallesFacturaAsync(model.DetalleFactura, idFactura);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "La factura fue actualizada exitosamente.";

                // Si se quiere redirigir a agregar detalles (opcional)
                if (Request.Form["RedirigirDetalle"] == "true")
                {
                    return RedirectToAction("Create", "TbDetalleFacturas", new { id = model.IdFactura });
                }

                return RedirectToAction(nameof(Edit));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar la factura: " + ex.Message;
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }
        }



        // GET: TbFacturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbFacturas == null)
            {
                return NotFound();
            }

            var tbFactura = await _context.TbFacturas
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdColaboradorNavigation)
                .Include(t => t.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (tbFactura == null)
            {
                return NotFound();
            }

            return View(tbFactura);
        }

        // POST: TbFacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbFacturas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbFacturas'  is null.");
            }
            var tbFactura = await _context.TbFacturas.FindAsync(id);
            if (tbFactura != null)
            {
                _context.TbFacturas.Remove(tbFactura);
                TempData["SuccessMessage"] = "La Factura fue eliminada correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbFacturaExists(int id)
        {
          return (_context.TbFacturas?.Any(e => e.IdFactura == id)).GetValueOrDefault();
        }
    }
}
