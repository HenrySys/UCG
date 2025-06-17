using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;

namespace UCG.Controllers
{
    public class TbDocumentoIngresoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbDocumentoIngresoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbDocumentoIngresoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbDocumentoIngresos.Include(t => t.IdActividadNavigation).Include(t => t.IdClienteNavigation).Include(t => t.IdConceptoAsociacionNavigation).Include(t => t.IdCuentaNavigation).Include(t => t.IdFinancistaNavigation).Include(t => t.IdMovimientoIngresoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbDocumentoIngresoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbDocumentoIngresos == null)
            {
                return NotFound();
            }

            var tbDocumentoIngreso = await _context.TbDocumentoIngresos
                .Include(t => t.IdActividadNavigation)
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdConceptoAsociacionNavigation)
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdFinancistaNavigation)
                .Include(t => t.IdMovimientoIngresoNavigation)
                .FirstOrDefaultAsync(m => m.IdDocumentoIngreso == id);
            if (tbDocumentoIngreso == null)
            {
                return NotFound();
            }

            return View(tbDocumentoIngreso);
        }

        // GET: TbDocumentoIngresoes/Create
        public IActionResult Create()
        {
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad");
            ViewData["IdCliente"] = new SelectList(_context.TbClientes, "IdCliente", "IdCliente");
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion");
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta");
            ViewData["IdFinancista"] = new SelectList(_context.TbFinancista, "IdFinancista", "IdFinancista");
            ViewData["IdMovimientoIngreso"] = new SelectList(_context.TbMovimientoIngresos, "IdMovimientoIngreso", "IdMovimientoIngreso");
            return View();
        }

        // POST: TbDocumentoIngresoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDocumentoIngreso,IdMovimientoIngreso,IdConceptoAsociacion,IdCuenta,IdCliente,IdFinancista,IdActividad,NumComprobante,FechaComprobante,MetodoPago,Descripcion,Monto")] TbDocumentoIngreso tbDocumentoIngreso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbDocumentoIngreso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad", tbDocumentoIngreso.IdActividad);
            ViewData["IdCliente"] = new SelectList(_context.TbClientes, "IdCliente", "IdCliente", tbDocumentoIngreso.IdCliente);
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion", tbDocumentoIngreso.IdConceptoAsociacion);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbDocumentoIngreso.IdCuenta);
            ViewData["IdFinancista"] = new SelectList(_context.TbFinancista, "IdFinancista", "IdFinancista", tbDocumentoIngreso.IdFinancista);
            ViewData["IdMovimientoIngreso"] = new SelectList(_context.TbMovimientoIngresos, "IdMovimientoIngreso", "IdMovimientoIngreso", tbDocumentoIngreso.IdMovimientoIngreso);
            return View(tbDocumentoIngreso);
        }

        // GET: TbDocumentoIngresoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbDocumentoIngresos == null)
            {
                return NotFound();
            }

            var tbDocumentoIngreso = await _context.TbDocumentoIngresos.FindAsync(id);
            if (tbDocumentoIngreso == null)
            {
                return NotFound();
            }
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad", tbDocumentoIngreso.IdActividad);
            ViewData["IdCliente"] = new SelectList(_context.TbClientes, "IdCliente", "IdCliente", tbDocumentoIngreso.IdCliente);
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion", tbDocumentoIngreso.IdConceptoAsociacion);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbDocumentoIngreso.IdCuenta);
            ViewData["IdFinancista"] = new SelectList(_context.TbFinancista, "IdFinancista", "IdFinancista", tbDocumentoIngreso.IdFinancista);
            ViewData["IdMovimientoIngreso"] = new SelectList(_context.TbMovimientoIngresos, "IdMovimientoIngreso", "IdMovimientoIngreso", tbDocumentoIngreso.IdMovimientoIngreso);
            return View(tbDocumentoIngreso);
        }

        // POST: TbDocumentoIngresoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDocumentoIngreso,IdMovimientoIngreso,IdConceptoAsociacion,IdCuenta,IdCliente,IdFinancista,IdActividad,NumComprobante,FechaComprobante,MetodoPago,Descripcion,Monto")] TbDocumentoIngreso tbDocumentoIngreso)
        {
            if (id != tbDocumentoIngreso.IdDocumentoIngreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbDocumentoIngreso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbDocumentoIngresoExists(tbDocumentoIngreso.IdDocumentoIngreso))
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
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad", tbDocumentoIngreso.IdActividad);
            ViewData["IdCliente"] = new SelectList(_context.TbClientes, "IdCliente", "IdCliente", tbDocumentoIngreso.IdCliente);
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion", tbDocumentoIngreso.IdConceptoAsociacion);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbDocumentoIngreso.IdCuenta);
            ViewData["IdFinancista"] = new SelectList(_context.TbFinancista, "IdFinancista", "IdFinancista", tbDocumentoIngreso.IdFinancista);
            ViewData["IdMovimientoIngreso"] = new SelectList(_context.TbMovimientoIngresos, "IdMovimientoIngreso", "IdMovimientoIngreso", tbDocumentoIngreso.IdMovimientoIngreso);
            return View(tbDocumentoIngreso);
        }

        // GET: TbDocumentoIngresoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbDocumentoIngresos == null)
            {
                return NotFound();
            }

            var tbDocumentoIngreso = await _context.TbDocumentoIngresos
                .Include(t => t.IdActividadNavigation)
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdConceptoAsociacionNavigation)
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdFinancistaNavigation)
                .Include(t => t.IdMovimientoIngresoNavigation)
                .FirstOrDefaultAsync(m => m.IdDocumentoIngreso == id);
            if (tbDocumentoIngreso == null)
            {
                return NotFound();
            }

            return View(tbDocumentoIngreso);
        }

        // POST: TbDocumentoIngresoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbDocumentoIngresos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbDocumentoIngresos'  is null.");
            }
            var tbDocumentoIngreso = await _context.TbDocumentoIngresos.FindAsync(id);
            if (tbDocumentoIngreso != null)
            {
                _context.TbDocumentoIngresos.Remove(tbDocumentoIngreso);
                TempData["SuccessMessage"] = "El Documento Ingreso fue eliminado correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbDocumentoIngresoExists(int id)
        {
          return (_context.TbDocumentoIngresos?.Any(e => e.IdDocumentoIngreso == id)).GetValueOrDefault();
        }
    }
}
