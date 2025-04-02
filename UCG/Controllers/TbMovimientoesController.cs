
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;
using static UCG.Models.TbConceptoMovimiento;

namespace UCG.Controllers
{
    public class TbMovimientoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMovimientoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMovimientoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMovimientos.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation).Include(t => t.IdCategoriaMovimientoNavigation).Include(t => t.IdCuentaNavigation).Include(t => t.IdProveedorNavigation).Include(t => t.IdAsociadoNavigation);
            return ucgdbContext != null ? View(await ucgdbContext.ToListAsync()) : Problem("Entity set 'UcgdbContext.TbMovimientos'  is null.");
        }

        // GET: TbMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMovimientos == null)
            {
                return NotFound();
            }

            var tbMovimiento = await _context.TbMovimientos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdCategoriaMovimientoNavigation)
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);
            if (tbMovimiento == null)
            {
                return NotFound();
            }

            return View(tbMovimiento);
        }

        // GET: TbMovimientoes/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(
           _context.TbAsociacions
               .Select(a => new
               {
                   IdAsociacion = a.IdAsociacion,
                   cod = a.CodigoRegistro + " " + a.CedulaJuridica
               }),
           "IdAsociacion", "cod");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movimiento = new TbMovimiento()
                {
                    FechaMovimiento = model.FechaMovimiento,
                    IdAsociacion = model.IdAsociacion,
                    IdAsociado = model.IdAsociado,
                    TipoMovimiento = model.TipoMovimiento,
                    IdConcepto = model.IdConceptoMovimiento,
                    IdCategoriaMovimiento = model.IdCategoriaMovimiento,
                    Estado = model.Estado,
                    FuenteFondo = model.FuenteFondo,
                    IdCuenta = model.IdCuenta,
                    MetdodoPago = model.MetodoPago,
                    SubtotalMovido = model.SubtotalMovido,
                    MontoTotalMovido = model.MontoTotalMovido,
                    IdActa = model?.IdActa,
                    IdProyecto = model?.IdProyecto,
                    IdCliente = model?.IdCliente,
                };

                // Verificar si el modelo contiene detalles
                // Agregar detalles si existen en el modelo
                if (model?.DetallesMovimiento != null && model.DetallesMovimiento.Any())
                {
                    foreach (var detalle in model.DetallesMovimiento)
                    {
                        movimiento.TbDetalleMovimientos.Add(new TbDetalleMovimiento
                        {
                            IdMovimiento = movimiento.IdMovimiento, // Asignamos el IdMovimiento al detalle
                            IdAcuerdo = detalle.IdAcuerdo,
                            Decripcion = detalle.Decripcion,
                            Subtotal = detalle.Subtotal
                        });
                    }
                }

                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", model.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", model.IdAsociacion);
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", model.IdConceptoMovimiento);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", model.IdProveedor);
            ViewData["IdProyecto"] = new SelectList(_context.TbProyectos, "IdProyecto", "IdProyecto", model.IdProyecto);

            return View(model);
        }


        [HttpGet]
        public JsonResult ObtenerProveedoresPorAsociacion(int idAsociacion)
        {
            try
            {
                var proveedores = _context.TbProveedors
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!proveedores.Any())
                {
                    return Json(new { success = false, message = "No hay proveedores disponibles." });
                }

                return Json(new { success = true, data = proveedores });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los proveedores: " + ex.Message });
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
                    return Json(new { success = false, message = "No hay proyectos disponibles." });
                }

                return Json(new { success = true, data = actas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los proyectos: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerProyectosPorAsociacion(int idAsociacion)
        {
            try
            {
                var proyectos = _context.TbProyectos
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!proyectos.Any())
                {
                    return Json(new { success = false, message = "No hay proyectos disponibles." });
                }

                return Json(new { success = true, data = proyectos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los proyectos: " + ex.Message });
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
        public JsonResult ObtenerCuentasPorAsociacion(int idAsociacion)
        {
            try
            {
                var cuentas = _context.TbCuenta
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!cuentas.Any())
                {
                    return Json(new { success = false, message = "No hay cuentas disponibles." });
                }

                return Json(new { success = true, data = cuentas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener las cuentas: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerConceptosPorTipo(int tipoMovimiento, int idAsociacion)
        {
            try
            {
                // Convertimos el int a enum de manera segura
                if (!Enum.IsDefined(typeof(TiposDeConceptoMovimientos), tipoMovimiento))
                {
                    return Json(new { success = false, message = "Tipo de movimiento no válido." });
                }

                var tipoMovimientoEnum = (TiposDeConceptoMovimientos)tipoMovimiento;

                // Obtener todos los conceptos que coincidan con el tipo
                var conceptos = _context.TbConceptoMovimientos
                    .Where(c => c.TipoMovimiento == tipoMovimientoEnum)
                    .ToList();

                // Filtrar solo los conceptos que pertenecen a la asociación
                var conceptosAsociacion = _context.TbConceptoAsociacions
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(c => c.IdConcepto)
                    .ToList();

                // Solo los conceptos que coincidan con ambos
                var conceptosFiltrados = conceptos
                    .Where(c => conceptosAsociacion.Contains(c.IdConceptoMovimiento))
                    .ToList();

                if (!conceptosFiltrados.Any())
                {
                    return Json(new { success = false, message = "No hay conceptos disponibles." });
                }

                return Json(new { success = true, data = conceptosFiltrados });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Ocurrió un error al obtener los conceptos.",
                    error = ex.Message
                });
            }
        }

        // Acción para obtener categorías por IdConcepto
        [HttpGet]
        public JsonResult ObtenerCategoriaPorTipoConcepto(int idConcepto)
        {
            try
            {
                var categorias = _context.TbCategoriaMovimientos
                    .Where(c => c.IdConceptoAsociacion == idConcepto)
                    .ToList();
                if (!categorias.Any())
                {
                    return Json(new { success = false, message = "No hay categorias disponibles." });
                }
                return Json(new { success = true, data = categorias });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "No hay categorias para el concepto de movimientos disponibles.", ex.Message });
            }
        }

        // GET: TbMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMovimientos == null)
            {
                return NotFound();
            }

            var tbMovimiento = await _context.TbMovimientos.FindAsync(id);
            if (tbMovimiento == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimiento.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimiento.IdAsociacion);
            ViewData["IdCategoriaMovimiento"] = new SelectList(_context.TbCategoriaMovimientos, "IdCategoriaMovimiento", "IdCategoriaMovimiento", tbMovimiento.IdCategoriaMovimiento);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbMovimiento.IdCuenta);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbMovimiento.IdProveedor);
            return View(tbMovimiento);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimiento,IdAsociacion,IdAsociado,TipoMovimiento,IdCategoriaMovimiento,FuenteFondo,IdProyecto,IdCuenta,IdActa,IdProveedor,IdCliente,Descripcion,MetdodoPago,FechaMovimiento,SubtotalMovido,MontoTotalMovido,Estado")] TbMovimiento tbMovimiento)
        {
            if (id != tbMovimiento.IdMovimiento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMovimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMovimientoExists(tbMovimiento.IdMovimiento))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimiento.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimiento.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimiento.IdAsociado);
            ViewData["IdCategoriaMovimiento"] = new SelectList(_context.TbCategoriaMovimientos, "IdCategoriaMovimiento", "IdCategoriaMovimiento", tbMovimiento.IdCategoriaMovimiento);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbMovimiento.IdCuenta);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbMovimiento.IdProveedor);
            return View(tbMovimiento);
        }

        // GET: TbMovimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientos == null)
            {
                return NotFound();
            }

            var tbMovimiento = await _context.TbMovimientos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdCategoriaMovimientoNavigation)
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);
            if (tbMovimiento == null)
            {
                return NotFound();
            }

            return View(tbMovimiento);
        }

        // POST: TbMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMovimientos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbMovimientos'  is null.");
            }
            var tbMovimiento = await _context.TbMovimientos.FindAsync(id);
            if (tbMovimiento != null)
            {
                _context.TbMovimientos.Remove(tbMovimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMovimientoExists(int id)
        {
            return (_context.TbMovimientos?.Any(e => e.IdMovimiento == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
