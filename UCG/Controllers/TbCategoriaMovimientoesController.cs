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
    public class TbCategoriaMovimientoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbCategoriaMovimientoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbCategoriaMovimientoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbCategoriaMovimientos.Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbCategoriaMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbCategoriaMovimientos == null)
            {
                return NotFound();
            }

            var tbCategoriaMovimiento = await _context.TbCategoriaMovimientos
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCategoriaMovimiento == id);
            if (tbCategoriaMovimiento == null)
            {
                return NotFound();
            }

            return View(tbCategoriaMovimiento);
        }

        // GET: TbCategoriaMovimientoes/Create
        public IActionResult Create()
        {
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            return View();
        }

        // POST: TbCategoriaMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoriaMovimiento,IdAsociado,TipoMovimiento,NombreCategoria,DescripcionCategoria,IdConceptoAsociacion")] TbCategoriaMovimiento tbCategoriaMovimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbCategoriaMovimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbCategoriaMovimiento.IdAsociado);
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion", tbCategoriaMovimiento.IdConceptoAsociacion);

            return View(tbCategoriaMovimiento);
        }

        // GET: TbCategoriaMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbCategoriaMovimientos == null)
            {
                return NotFound();
            }

            var tbCategoriaMovimiento = await _context.TbCategoriaMovimientos.FindAsync(id);
            if (tbCategoriaMovimiento == null)
            {
                return NotFound();
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbCategoriaMovimiento.IdAsociado);
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion", tbCategoriaMovimiento.IdConceptoAsociacion);

            return View(tbCategoriaMovimiento);
        }

        // POST: TbCategoriaMovimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoriaMovimiento,IdAsociado,TipoMovimiento,NombreCategoria,DescripcionCategoria,IdConceptoAsociacion")] TbCategoriaMovimiento tbCategoriaMovimiento)
        {
            if (id != tbCategoriaMovimiento.IdCategoriaMovimiento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbCategoriaMovimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbCategoriaMovimientoExists(tbCategoriaMovimiento.IdCategoriaMovimiento))
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
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbCategoriaMovimiento.IdAsociado);
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion", tbCategoriaMovimiento.IdConceptoAsociacion);

            return View(tbCategoriaMovimiento);
        }

        // GET: TbCategoriaMovimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbCategoriaMovimientos == null)
            {
                return NotFound();
            }

            var tbCategoriaMovimiento = await _context.TbCategoriaMovimientos
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCategoriaMovimiento == id);
            if (tbCategoriaMovimiento == null)
            {
                return NotFound();
            }

            return View(tbCategoriaMovimiento);
        }

        // POST: TbCategoriaMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbCategoriaMovimientos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbCategoriaMovimientos'  is null.");
            }
            var tbCategoriaMovimiento = await _context.TbCategoriaMovimientos.FindAsync(id);
            if (tbCategoriaMovimiento != null)
            {
                _context.TbCategoriaMovimientos.Remove(tbCategoriaMovimiento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbCategoriaMovimientoExists(int id)
        {
          return (_context.TbCategoriaMovimientos?.Any(e => e.IdCategoriaMovimiento == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
