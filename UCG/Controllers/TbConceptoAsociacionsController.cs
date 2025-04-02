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
    public class TbConceptoAsociacionsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbConceptoAsociacionsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbConceptoAsociacions
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbConceptoAsociacions.Include(t => t.IdAsociacionNavigation).Include(t => t.IdConceptoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbConceptoAsociacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbConceptoAsociacions == null)
            {
                return NotFound();
            }

            var tbConceptoAsociacion = await _context.TbConceptoAsociacions
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdConceptoNavigation)
                .FirstOrDefaultAsync(m => m.IdConceptoAsociacion == id);
            if (tbConceptoAsociacion == null)
            {
                return NotFound();
            }

            return View(tbConceptoAsociacion);
        }

        // GET: TbConceptoAsociacions/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento");
            return View();
        }

        // POST: TbConceptoAsociacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConceptoAsociacion,IdAsociacion,IdConcepto")] TbConceptoAsociacion tbConceptoAsociacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbConceptoAsociacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbConceptoAsociacion.IdAsociacion);
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", tbConceptoAsociacion.IdConcepto);
            return View(tbConceptoAsociacion);
        }

        // GET: TbConceptoAsociacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbConceptoAsociacions == null)
            {
                return NotFound();
            }

            var tbConceptoAsociacion = await _context.TbConceptoAsociacions.FindAsync(id);
            if (tbConceptoAsociacion == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbConceptoAsociacion.IdAsociacion);
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", tbConceptoAsociacion.IdConcepto);
            return View(tbConceptoAsociacion);
        }

        // POST: TbConceptoAsociacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConceptoAsociacion,IdAsociacion,IdConcepto")] TbConceptoAsociacion tbConceptoAsociacion)
        {
            if (id != tbConceptoAsociacion.IdConceptoAsociacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbConceptoAsociacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbConceptoAsociacionExists(tbConceptoAsociacion.IdConceptoAsociacion))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbConceptoAsociacion.IdAsociacion);
            ViewData["IdConcepto"] = new SelectList(_context.TbConceptoMovimientos, "IdConceptoMovimiento", "IdConceptoMovimiento", tbConceptoAsociacion.IdConcepto);
            return View(tbConceptoAsociacion);
        }

        // GET: TbConceptoAsociacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbConceptoAsociacions == null)
            {
                return NotFound();
            }

            var tbConceptoAsociacion = await _context.TbConceptoAsociacions
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdConceptoNavigation)
                .FirstOrDefaultAsync(m => m.IdConceptoAsociacion == id);
            if (tbConceptoAsociacion == null)
            {
                return NotFound();
            }

            return View(tbConceptoAsociacion);
        }

        // POST: TbConceptoAsociacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbConceptoAsociacions == null)
            {
                return Problem("Entity set 'UcgdbContext.TbConceptoAsociacions'  is null.");
            }
            var tbConceptoAsociacion = await _context.TbConceptoAsociacions.FindAsync(id);
            if (tbConceptoAsociacion != null)
            {
                _context.TbConceptoAsociacions.Remove(tbConceptoAsociacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbConceptoAsociacionExists(int id)
        {
          return (_context.TbConceptoAsociacions?.Any(e => e.IdConceptoAsociacion == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
