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
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdConceptoAsociacionNavigation)
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
            // 1. Validar fechas
            var fechaEmisionValida = await ParseFechaEmisionAsync(model);
            //var fechaSubidaValida = await ParseFechaSubidaAsync(model);

            if (!fechaEmisionValida)
            {
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // 2. Validar con FluentValidation
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

            // 3. Guardar con transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var factura = MapearFactura(model);
                _context.TbFacturas.Add(factura);
                await _context.SaveChangesAsync();

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
                //ArchivoUrl = model.ArchivoUrl,
                //NombreArchivo = model.NombreArchivo,
                //FechaSubida = model.FechaSubida                  
            };
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

        //private async Task<bool> ParseFechaSubidaAsync(FacturaViewModel model)
        //{
        //    if (!string.IsNullOrWhiteSpace(model.FechaTextoSubida))
        //    {
        //        if (DateTime.TryParseExact(model.FechaTextoSubida, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaSubida))
        //        {
        //            model.FechaSubida = fechaSubida;
        //            return true;
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Debe ingresar una fecha de subida válida (formato: yyyy-MM-dd).";
        //            TempData.Keep("ErrorMessage");
        //            return false;
        //        }
        //    }

        //    TempData["ErrorMessage"] = "Debe ingresar la fecha de subida.";
        //    TempData.Keep("ErrorMessage");
        //    return false;
        //}



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

        // GET: TbFacturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbFacturas == null)
            {
                return NotFound();
            }

            var tbFactura = await _context.TbFacturas
                .Include(f => f.IdAsociacionNavigation)
                .Include(f => f.IdAsociadoNavigation)
                .Include(f => f.IdColaboradorNavigation)
                .Include(f => f.IdProveedorNavigation)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (tbFactura == null)
            {
                return NotFound();
            }

            var model = new FacturaViewModel
            {
                IdFactura = tbFactura.IdFactura,
                NumeroFactura = tbFactura.NumeroFactura,
                FechaEmision = tbFactura.FechaEmision,
                Descripcion = tbFactura.Descripcion,
                IdConceptoAsociacion = tbFactura.IdConceptoAsociacion,
                IdColaborador = tbFactura.IdColaborador,
                IdProveedor = tbFactura.IdProveedor,
                IdAsociacion = tbFactura.IdAsociacion,
                IdAsociado = tbFactura.IdAsociado,
                MontoTotal = tbFactura.MontoTotal,
                FechaTextoEmision = tbFactura.FechaEmision.ToString("yyyy-MM-dd")
            };

            await ConfigurarAsociacionFacturaAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idFactura, FacturaViewModel model)
        {
            if (idFactura != model.IdFactura)
                return NotFound();

            // Parsear la fecha desde el texto
            if (!await ParseFechaEmisionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de emisión válida.";
                await ConfigurarAsociacionFacturaAsync(model);
                return View(model);
            }

            // Validar el modelo con FluentValidation
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
                    .FirstOrDefaultAsync(f => f.IdFactura == idFactura);

                if (facturaExistente == null)
                    return NotFound();

                // Actualizar campos
                facturaExistente.NumeroFactura = model.NumeroFactura;
                facturaExistente.FechaEmision = model.FechaEmision;
                facturaExistente.Descripcion = model.Descripcion;
                facturaExistente.MontoTotal = model.MontoTotal;
                facturaExistente.IdAsociacion = model.IdAsociacion;
                facturaExistente.IdAsociado = model.IdAsociado;
                facturaExistente.IdColaborador = model.IdColaborador;
                facturaExistente.IdProveedor = model.IdProveedor;
                facturaExistente.IdConceptoAsociacion = model.IdConceptoAsociacion;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "La factura fue actualizada exitosamente.";
                return RedirectToAction(nameof(Edit), new { idFactura });
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
