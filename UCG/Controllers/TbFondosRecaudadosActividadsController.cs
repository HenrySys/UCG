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
    public class TbFondosRecaudadosActividadsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbFondosRecaudadosActividadsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbFondosRecaudadosActividads
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbFondosRecaudadosActividads.Include(t => t.IdActividadNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbFondosRecaudadosActividads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbFondosRecaudadosActividads == null)
            {
                return NotFound();
            }

            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads
                .Include(t => t.IdActividadNavigation)
                .FirstOrDefaultAsync(m => m.IdFondosRecaudadosActividad == id);
            if (tbFondosRecaudadosActividad == null)
            {
                return NotFound();
            }

            return View(tbFondosRecaudadosActividad);
        }

        // GET: TbFondosRecaudadosActividads/Create
        public IActionResult Create()
        {
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad");
            return View();
        }

        // POST: TbFondosRecaudadosActividads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFondosRecaudadosActividad,IdActividad,Detalle,Monto,FechaRegistro")] TbFondosRecaudadosActividad tbFondosRecaudadosActividad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbFondosRecaudadosActividad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad", tbFondosRecaudadosActividad.IdActividad);
            return View(tbFondosRecaudadosActividad);
        }

        // GET: TbFondosRecaudadosActividads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbFondosRecaudadosActividads == null)
            {
                return NotFound();
            }

            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads.FindAsync(id);
            if (tbFondosRecaudadosActividad == null)
            {
                return NotFound();
            }
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad", tbFondosRecaudadosActividad.IdActividad);
            return View(tbFondosRecaudadosActividad);
        }

        // POST: TbFondosRecaudadosActividads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFondosRecaudadosActividad,IdActividad,Detalle,Monto,FechaRegistro")] TbFondosRecaudadosActividad tbFondosRecaudadosActividad)
        {
            if (id != tbFondosRecaudadosActividad.IdFondosRecaudadosActividad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbFondosRecaudadosActividad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbFondosRecaudadosActividadExists(tbFondosRecaudadosActividad.IdFondosRecaudadosActividad))
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
            ViewData["IdActividad"] = new SelectList(_context.TbActividads, "IdActividad", "IdActividad", tbFondosRecaudadosActividad.IdActividad);
            return View(tbFondosRecaudadosActividad);
        }

        // GET: TbFondosRecaudadosActividads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbFondosRecaudadosActividads == null)
            {
                return NotFound();
            }

            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads
                .Include(t => t.IdActividadNavigation)
                .FirstOrDefaultAsync(m => m.IdFondosRecaudadosActividad == id);
            if (tbFondosRecaudadosActividad == null)
            {
                return NotFound();
            }

            return View(tbFondosRecaudadosActividad);
        }

        // POST: TbFondosRecaudadosActividads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbFondosRecaudadosActividads == null)
            {
                return Problem("Entity set 'UcgdbContext.TbFondosRecaudadosActividads'  is null.");
            }
            var tbFondosRecaudadosActividad = await _context.TbFondosRecaudadosActividads.FindAsync(id);
            if (tbFondosRecaudadosActividad != null)
            {
                _context.TbFondosRecaudadosActividads.Remove(tbFondosRecaudadosActividad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbFondosRecaudadosActividadExists(int id)
        {
          return (_context.TbFondosRecaudadosActividads?.Any(e => e.IdFondosRecaudadosActividad == id)).GetValueOrDefault();
        }
    }
}
