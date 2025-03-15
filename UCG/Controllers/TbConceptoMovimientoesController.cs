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
    public class TbConceptoMovimientoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbConceptoMovimientoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbConceptoMovimientoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbConceptoMovimientos.Include(t => t.IdAsociacionNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbConceptoMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbConceptoMovimientos == null)
            {
                return NotFound();
            }

            var tbConceptoMovimiento = await _context.TbConceptoMovimientos
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdConceptoMovimiento == id);
            if (tbConceptoMovimiento == null)
            {
                return NotFound();
            }

            return View(tbConceptoMovimiento);
        }

        // GET: TbConceptoMovimientoes/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            return View();
        }

        // POST: TbConceptoMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConceptoMovimiento,IdAsociacion,TipoMovimiento,Concepto")] TbConceptoMovimiento tbConceptoMovimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbConceptoMovimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbConceptoMovimiento.IdAsociacion);
            return View(tbConceptoMovimiento);
        }

        // GET: TbConceptoMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbConceptoMovimientos == null)
            {
                return NotFound();
            }

            var tbConceptoMovimiento = await _context.TbConceptoMovimientos.FindAsync(id);
            if (tbConceptoMovimiento == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbConceptoMovimiento.IdAsociacion);
            return View(tbConceptoMovimiento);
        }

        // POST: TbConceptoMovimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConceptoMovimiento,IdAsociacion,TipoMovimiento,Concepto")] TbConceptoMovimiento tbConceptoMovimiento)
        {
            if (id != tbConceptoMovimiento.IdConceptoMovimiento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbConceptoMovimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbConceptoMovimientoExists(tbConceptoMovimiento.IdConceptoMovimiento))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbConceptoMovimiento.IdAsociacion);
            return View(tbConceptoMovimiento);
        }

        // GET: TbConceptoMovimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbConceptoMovimientos == null)
            {
                return NotFound();
            }

            var tbConceptoMovimiento = await _context.TbConceptoMovimientos
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdConceptoMovimiento == id);
            if (tbConceptoMovimiento == null)
            {
                return NotFound();
            }

            return View(tbConceptoMovimiento);
        }

        // POST: TbConceptoMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbConceptoMovimientos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbConceptoMovimientos'  is null.");
            }
            var tbConceptoMovimiento = await _context.TbConceptoMovimientos.FindAsync(id);
            if (tbConceptoMovimiento != null)
            {
                _context.TbConceptoMovimientos.Remove(tbConceptoMovimiento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbConceptoMovimientoExists(int id)
        {
          return (_context.TbConceptoMovimientos?.Any(e => e.IdConceptoMovimiento == id)).GetValueOrDefault();
        }
    }
}
