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
    public class TbJuntaDirectivasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbJuntaDirectivasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbJuntaDirectivas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbJuntaDirectivas.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbJuntaDirectivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbJuntaDirectiva = await _context.TbJuntaDirectivas
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdJuntaDirectiva == id);
            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbJuntaDirectiva);
        }

        // GET: TbJuntaDirectivas/Create
        public IActionResult Create()
        {
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            return View();
        }

        // POST: TbJuntaDirectivas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJuntaDirectiva,IdAsociacion,IdActa,PeriodoInicio,PeriodoFin,Nombre,Estado")] TbJuntaDirectiva tbJuntaDirectiva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbJuntaDirectiva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbJuntaDirectiva.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbJuntaDirectiva.IdAsociacion);
            return View(tbJuntaDirectiva);
        }

        // GET: TbJuntaDirectivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbJuntaDirectiva = await _context.TbJuntaDirectivas.FindAsync(id);
            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbJuntaDirectiva.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbJuntaDirectiva.IdAsociacion);
            return View(tbJuntaDirectiva);
        }

        // POST: TbJuntaDirectivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdJuntaDirectiva,IdAsociacion,IdActa,PeriodoInicio,PeriodoFin,Nombre,Estado")] TbJuntaDirectiva tbJuntaDirectiva)
        {
            if (id != tbJuntaDirectiva.IdJuntaDirectiva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbJuntaDirectiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbJuntaDirectivaExists(tbJuntaDirectiva.IdJuntaDirectiva))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbJuntaDirectiva.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbJuntaDirectiva.IdAsociacion);
            return View(tbJuntaDirectiva);
        }

        // GET: TbJuntaDirectivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbJuntaDirectiva = await _context.TbJuntaDirectivas
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdJuntaDirectiva == id);
            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbJuntaDirectiva);
        }

        // POST: TbJuntaDirectivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbJuntaDirectivas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbJuntaDirectivas'  is null.");
            }
            var tbJuntaDirectiva = await _context.TbJuntaDirectivas.FindAsync(id);
            if (tbJuntaDirectiva != null)
            {
                _context.TbJuntaDirectivas.Remove(tbJuntaDirectiva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbJuntaDirectivaExists(int id)
        {
          return (_context.TbJuntaDirectivas?.Any(e => e.IdJuntaDirectiva == id)).GetValueOrDefault();
        }
    }
}
