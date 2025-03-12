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
    public class TbActumsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbActumsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbActums
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbActa.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbActums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbActa == null)
            {
                return NotFound();
            }

            var tbActum = await _context.TbActa
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActa == id);
            if (tbActum == null)
            {
                return NotFound();
            }

            return View(tbActum);
        }

        // GET: TbActums/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            return View();
        }

        // POST: TbActums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdActa,IdAsociacion,IdAsociado,FechaSesion,NumeroActa,Descripcion,Estado,MontoTotalAcordado")] TbActum tbActum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbActum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActum.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActum.IdAsociado);
            return View(tbActum);
        }

        // GET: TbActums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbActa == null)
            {
                return NotFound();
            }

            var tbActum = await _context.TbActa.FindAsync(id);
            if (tbActum == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActum.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActum.IdAsociado);
            return View(tbActum);
        }

        // POST: TbActums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActa,IdAsociacion,IdAsociado,FechaSesion,NumeroActa,Descripcion,Estado,MontoTotalAcordado")] TbActum tbActum)
        {
            if (id != tbActum.IdActa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbActum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbActumExists(tbActum.IdActa))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActum.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActum.IdAsociado);
            return View(tbActum);
        }

        // GET: TbActums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbActa == null)
            {
                return NotFound();
            }

            var tbActum = await _context.TbActa
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActa == id);
            if (tbActum == null)
            {
                return NotFound();
            }

            return View(tbActum);
        }

        // POST: TbActums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbActa == null)
            {
                return Problem("Entity set 'UcgdbContext.TbActa'  is null.");
            }
            var tbActum = await _context.TbActa.FindAsync(id);
            if (tbActum != null)
            {
                _context.TbActa.Remove(tbActum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbActumExists(int id)
        {
          return (_context.TbActa?.Any(e => e.IdActa == id)).GetValueOrDefault();
        }
    }
}
