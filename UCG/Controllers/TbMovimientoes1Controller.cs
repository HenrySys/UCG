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
    public class TbMovimientoes1Controller : Controller
    {
        private readonly UcgdbContext _context;

        public TbMovimientoes1Controller(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMovimientoes1
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMovimientos.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation).Include(t => t.IdCategoriaMovimientoNavigation).Include(t => t.IdConceptoNavigation).Include(t => t.IdCuentaNavigation).Include(t => t.IdProveedorNavigation).Include(t => t.IdProyectoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMovimientoes1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMovimientos == null)
            {
                return NotFound();
            }

            var tbMovimiento = await _context.TbMovimientos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdCategoriaMovimientoNavigation)
                .Include(t => t.IdConceptoNavigation)
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdProveedorNavigation)
                .Include(t => t.IdProyectoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);
            if (tbMovimiento == null)
            {
                return NotFound();
            }

            return View(tbMovimiento);
        }

        // GET: TbMovimientoes1/Create
        public IActionResult Create()
        {
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            ViewData["IdCategoriaMovimiento"] = new SelectList(_context.TbCategoriaMovimientos, "IdCategoriaMovimiento", "IdCategoriaMovimiento");
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento");
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta");
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor");
            ViewData["IdProyecto"] = new SelectList(_context.TbProyectos, "IdProyecto", "IdProyecto");
            return View();
        }

        // POST: TbMovimientoes1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMovimiento,IdAsociacion,IdAsociado,TipoMovimiento,IdCategoriaMovimiento,FuenteFondo,IdProyecto,IdCuenta,IdActa,IdProveedor,IdCliente,Descripcion,MetdodoPago,FechaMovimiento,SubtotalMovido,MontoTotalMovido,Estado,IdConcepto")] TbMovimiento tbMovimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbMovimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimiento.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimiento.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimiento.IdAsociado);
            ViewData["IdCategoriaMovimiento"] = new SelectList(_context.TbCategoriaMovimientos, "IdCategoriaMovimiento", "IdCategoriaMovimiento", tbMovimiento.IdCategoriaMovimiento);
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", tbMovimiento.IdConcepto);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbMovimiento.IdCuenta);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbMovimiento.IdProveedor);
            ViewData["IdProyecto"] = new SelectList(_context.TbProyectos, "IdProyecto", "IdProyecto", tbMovimiento.IdProyecto);
            return View(tbMovimiento);
        }

        // GET: TbMovimientoes1/Edit/5
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
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimiento.IdAsociado);
            ViewData["IdCategoriaMovimiento"] = new SelectList(_context.TbCategoriaMovimientos, "IdCategoriaMovimiento", "IdCategoriaMovimiento", tbMovimiento.IdCategoriaMovimiento);
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", tbMovimiento.IdConcepto);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbMovimiento.IdCuenta);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbMovimiento.IdProveedor);
            ViewData["IdProyecto"] = new SelectList(_context.TbProyectos, "IdProyecto", "IdProyecto", tbMovimiento.IdProyecto);
            return View(tbMovimiento);
        }

        // POST: TbMovimientoes1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimiento,IdAsociacion,IdAsociado,TipoMovimiento,IdCategoriaMovimiento,FuenteFondo,IdProyecto,IdCuenta,IdActa,IdProveedor,IdCliente,Descripcion,MetdodoPago,FechaMovimiento,SubtotalMovido,MontoTotalMovido,Estado,IdConcepto")] TbMovimiento tbMovimiento)
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
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", tbMovimiento.IdConcepto);
            ViewData["IdCuenta"] = new SelectList(_context.TbCuenta, "IdCuenta", "IdCuenta", tbMovimiento.IdCuenta);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbMovimiento.IdProveedor);
            ViewData["IdProyecto"] = new SelectList(_context.TbProyectos, "IdProyecto", "IdProyecto", tbMovimiento.IdProyecto);
            return View(tbMovimiento);
        }

        // GET: TbMovimientoes1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientos == null)
            {
                return NotFound();
            }

            var tbMovimiento = await _context.TbMovimientos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdCategoriaMovimientoNavigation)
                .Include(t => t.IdConceptoNavigation)
                .Include(t => t.IdCuentaNavigation)
                .Include(t => t.IdProveedorNavigation)
                .Include(t => t.IdProyectoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);
            if (tbMovimiento == null)
            {
                return NotFound();
            }

            return View(tbMovimiento);
        }

        // POST: TbMovimientoes1/Delete/5
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
