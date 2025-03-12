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
    public class TbMiembroJuntaDirectivasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMiembroJuntaDirectivasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMiembroJuntaDirectivas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMiembroJuntaDirectivas.Include(t => t.IdAsociadoNavigation).Include(t => t.IdJuntaDirectivaNavigation).Include(t => t.IdPuestoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMiembroJuntaDirectivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMiembroJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdJuntaDirectivaNavigation)
                .Include(t => t.IdPuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdMiembrosJuntaDirectiva == id);
            if (tbMiembroJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbMiembroJuntaDirectiva);
        }

        // GET: TbMiembroJuntaDirectivas/Create
        public IActionResult Create()
        {
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.TbJuntaDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva");
            ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "IdPuesto");
            return View();
        }

        // POST: TbMiembroJuntaDirectivas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMiembrosJuntaDirectiva,IdJuntaDirectiva,IdAsociado,IdPuesto,Estado")] TbMiembroJuntaDirectiva tbMiembroJuntaDirectiva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbMiembroJuntaDirectiva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMiembroJuntaDirectiva.IdAsociado);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.TbJuntaDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", tbMiembroJuntaDirectiva.IdJuntaDirectiva);
            ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "IdPuesto", tbMiembroJuntaDirectiva.IdPuesto);
            return View(tbMiembroJuntaDirectiva);
        }

        // GET: TbMiembroJuntaDirectivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMiembroJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas.FindAsync(id);
            if (tbMiembroJuntaDirectiva == null)
            {
                return NotFound();
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMiembroJuntaDirectiva.IdAsociado);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.TbJuntaDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", tbMiembroJuntaDirectiva.IdJuntaDirectiva);
            ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "IdPuesto", tbMiembroJuntaDirectiva.IdPuesto);
            return View(tbMiembroJuntaDirectiva);
        }

        // POST: TbMiembroJuntaDirectivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMiembrosJuntaDirectiva,IdJuntaDirectiva,IdAsociado,IdPuesto,Estado")] TbMiembroJuntaDirectiva tbMiembroJuntaDirectiva)
        {
            if (id != tbMiembroJuntaDirectiva.IdMiembrosJuntaDirectiva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMiembroJuntaDirectiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMiembroJuntaDirectivaExists(tbMiembroJuntaDirectiva.IdMiembrosJuntaDirectiva))
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
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMiembroJuntaDirectiva.IdAsociado);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.TbJuntaDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", tbMiembroJuntaDirectiva.IdJuntaDirectiva);
            ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "IdPuesto", tbMiembroJuntaDirectiva.IdPuesto);
            return View(tbMiembroJuntaDirectiva);
        }

        // GET: TbMiembroJuntaDirectivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMiembroJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdJuntaDirectivaNavigation)
                .Include(t => t.IdPuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdMiembrosJuntaDirectiva == id);
            if (tbMiembroJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbMiembroJuntaDirectiva);
        }

        // POST: TbMiembroJuntaDirectivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMiembroJuntaDirectivas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbMiembroJuntaDirectivas'  is null.");
            }
            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas.FindAsync(id);
            if (tbMiembroJuntaDirectiva != null)
            {
                _context.TbMiembroJuntaDirectivas.Remove(tbMiembroJuntaDirectiva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMiembroJuntaDirectivaExists(int id)
        {
          return (_context.TbMiembroJuntaDirectivas?.Any(e => e.IdMiembrosJuntaDirectiva == id)).GetValueOrDefault();
        }
    }
}
