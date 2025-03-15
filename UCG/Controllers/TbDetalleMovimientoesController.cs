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
    public class TbDetalleMovimientoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbDetalleMovimientoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbDetalleMovimientoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbDetalleMovimientos.Include(t => t.IdAcuerdoNavigation).Include(t => t.IdMovimientoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbDetalleMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbDetalleMovimientos == null)
            {
                return NotFound();
            }

            var tbDetalleMovimiento = await _context.TbDetalleMovimientos
                .Include(t => t.IdAcuerdoNavigation)
                .Include(t => t.IdMovimientoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleMovimiento == id);
            if (tbDetalleMovimiento == null)
            {
                return NotFound();
            }

            return View(tbDetalleMovimiento);
        }

        // GET: TbDetalleMovimientoes/Create
        public IActionResult Create()
        {
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo");
            ViewData["IdMovimiento"] = new SelectList(_context.TbMovimientos, "IdMovimiento", "IdMovimiento");
            return View();
        }

        // POST: TbDetalleMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalleMovimiento,IdMovimiento,IdAcuerdo,TipoMovimiento,Decripcion,Subtotal,IdInformeEconomico")] TbDetalleMovimiento tbDetalleMovimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbDetalleMovimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo", tbDetalleMovimiento.IdAcuerdo);
            ViewData["IdMovimiento"] = new SelectList(_context.TbMovimientos, "IdMovimiento", "IdMovimiento", tbDetalleMovimiento.IdMovimiento);
            return View(tbDetalleMovimiento);
        }

        // GET: TbDetalleMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbDetalleMovimientos == null)
            {
                return NotFound();
            }

            var tbDetalleMovimiento = await _context.TbDetalleMovimientos.FindAsync(id);
            if (tbDetalleMovimiento == null)
            {
                return NotFound();
            }
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo", tbDetalleMovimiento.IdAcuerdo);
            ViewData["IdMovimiento"] = new SelectList(_context.TbMovimientos, "IdMovimiento", "IdMovimiento", tbDetalleMovimiento.IdMovimiento);
            return View(tbDetalleMovimiento);
        }

        // POST: TbDetalleMovimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalleMovimiento,IdMovimiento,IdAcuerdo,TipoMovimiento,Decripcion,Subtotal,IdInformeEconomico")] TbDetalleMovimiento tbDetalleMovimiento)
        {
            if (id != tbDetalleMovimiento.IdDetalleMovimiento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbDetalleMovimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbDetalleMovimientoExists(tbDetalleMovimiento.IdDetalleMovimiento))
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
            ViewData["IdAcuerdo"] = new SelectList(_context.TbAcuerdos, "IdAcuerdo", "IdAcuerdo", tbDetalleMovimiento.IdAcuerdo);
            ViewData["IdMovimiento"] = new SelectList(_context.TbMovimientos, "IdMovimiento", "IdMovimiento", tbDetalleMovimiento.IdMovimiento);
            return View(tbDetalleMovimiento);
        }

        // GET: TbDetalleMovimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbDetalleMovimientos == null)
            {
                return NotFound();
            }

            var tbDetalleMovimiento = await _context.TbDetalleMovimientos
                .Include(t => t.IdAcuerdoNavigation)
                .Include(t => t.IdMovimientoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleMovimiento == id);
            if (tbDetalleMovimiento == null)
            {
                return NotFound();
            }

            return View(tbDetalleMovimiento);
        }

        // POST: TbDetalleMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbDetalleMovimientos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbDetalleMovimientos'  is null.");
            }
            var tbDetalleMovimiento = await _context.TbDetalleMovimientos.FindAsync(id);
            if (tbDetalleMovimiento != null)
            {
                _context.TbDetalleMovimientos.Remove(tbDetalleMovimiento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbDetalleMovimientoExists(int id)
        {
          return (_context.TbDetalleMovimientos?.Any(e => e.IdDetalleMovimiento == id)).GetValueOrDefault();
        }
    }
}
