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
    public class TbDetalleChequeFacturasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbDetalleChequeFacturasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbDetalleChequeFacturas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbDetalleChequeFacturas.Include(t => t.IdAcuerdoNavigation).Include(t => t.IdChequeNavigation).Include(t => t.IdFacturaNavigation).Include(t => t.IdMovimientoEgresoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbDetalleChequeFacturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbDetalleChequeFacturas == null)
            {
                return NotFound();
            }

            var tbDetalleChequeFactura = await _context.TbDetalleChequeFacturas
                .Include(t => t.IdAcuerdoNavigation)
                .Include(t => t.IdChequeNavigation)
                .Include(t => t.IdFacturaNavigation)
                .Include(t => t.IdMovimientoEgresoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleChequeFactura == id);
            if (tbDetalleChequeFactura == null)
            {
                return NotFound();
            }

            return View(tbDetalleChequeFactura);
        }

        // GET: TbDetalleChequeFacturas/Create
        public IActionResult Create()
        {
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo");
            ViewData["IdCheque"] = new SelectList(_context.TbCheques, "IdCheque", "IdCheque");
            ViewData["IdFactura"] = new SelectList(_context.TbFacturas, "IdFactura", "IdFactura");
            ViewData["IdMovimientoEgreso"] = new SelectList(_context.TbMovimientoEgresos, "IdMovimientoEgreso", "IdMovimientoEgreso");
            return View();
        }

        // POST: TbDetalleChequeFacturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalleChequeFactura,IdMovimientoEgreso,IdAcuerdo,IdCheque,IdFactura,Monto,Observacion")] TbDetalleChequeFactura tbDetalleChequeFactura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbDetalleChequeFactura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo", tbDetalleChequeFactura.IdAcuerdo);
            ViewData["IdCheque"] = new SelectList(_context.TbCheques, "IdCheque", "IdCheque", tbDetalleChequeFactura.IdCheque);
            ViewData["IdFactura"] = new SelectList(_context.TbFacturas, "IdFactura", "IdFactura", tbDetalleChequeFactura.IdFactura);
            ViewData["IdMovimientoEgreso"] = new SelectList(_context.TbMovimientoEgresos, "IdMovimientoEgreso", "IdMovimientoEgreso", tbDetalleChequeFactura.IdMovimientoEgreso);
            return View(tbDetalleChequeFactura);
        }

        // GET: TbDetalleChequeFacturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbDetalleChequeFacturas == null)
            {
                return NotFound();
            }

            var tbDetalleChequeFactura = await _context.TbDetalleChequeFacturas.FindAsync(id);
            if (tbDetalleChequeFactura == null)
            {
                return NotFound();
            }
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo", tbDetalleChequeFactura.IdAcuerdo);
            ViewData["IdCheque"] = new SelectList(_context.TbCheques, "IdCheque", "IdCheque", tbDetalleChequeFactura.IdCheque);
            ViewData["IdFactura"] = new SelectList(_context.TbFacturas, "IdFactura", "IdFactura", tbDetalleChequeFactura.IdFactura);
            ViewData["IdMovimientoEgreso"] = new SelectList(_context.TbMovimientoEgresos, "IdMovimientoEgreso", "IdMovimientoEgreso", tbDetalleChequeFactura.IdMovimientoEgreso);
            return View(tbDetalleChequeFactura);
        }

        // POST: TbDetalleChequeFacturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalleChequeFactura,IdMovimientoEgreso,IdAcuerdo,IdCheque,IdFactura,Monto,Observacion")] TbDetalleChequeFactura tbDetalleChequeFactura)
        {
            if (id != tbDetalleChequeFactura.IdDetalleChequeFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbDetalleChequeFactura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbDetalleChequeFacturaExists(tbDetalleChequeFactura.IdDetalleChequeFactura))
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
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo", tbDetalleChequeFactura.IdAcuerdo);
            ViewData["IdCheque"] = new SelectList(_context.TbCheques, "IdCheque", "IdCheque", tbDetalleChequeFactura.IdCheque);
            ViewData["IdFactura"] = new SelectList(_context.TbFacturas, "IdFactura", "IdFactura", tbDetalleChequeFactura.IdFactura);
            ViewData["IdMovimientoEgreso"] = new SelectList(_context.TbMovimientoEgresos, "IdMovimientoEgreso", "IdMovimientoEgreso", tbDetalleChequeFactura.IdMovimientoEgreso);
            return View(tbDetalleChequeFactura);
        }

        // GET: TbDetalleChequeFacturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbDetalleChequeFacturas == null)
            {
                return NotFound();
            }

            var tbDetalleChequeFactura = await _context.TbDetalleChequeFacturas
                .Include(t => t.IdAcuerdoNavigation)
                .Include(t => t.IdChequeNavigation)
                .Include(t => t.IdFacturaNavigation)
                .Include(t => t.IdMovimientoEgresoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleChequeFactura == id);
            if (tbDetalleChequeFactura == null)
            {
                return NotFound();
            }

            return View(tbDetalleChequeFactura);
        }

        // POST: TbDetalleChequeFacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbDetalleChequeFacturas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbDetalleChequeFacturas'  is null.");
            }
            var tbDetalleChequeFactura = await _context.TbDetalleChequeFacturas.FindAsync(id);
            if (tbDetalleChequeFactura != null)
            {
                _context.TbDetalleChequeFacturas.Remove(tbDetalleChequeFactura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbDetalleChequeFacturaExists(int id)
        {
          return (_context.TbDetalleChequeFacturas?.Any(e => e.IdDetalleChequeFactura == id)).GetValueOrDefault();
        }
    }
}
