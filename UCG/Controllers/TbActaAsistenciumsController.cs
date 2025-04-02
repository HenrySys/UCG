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
    public class TbActaAsistenciumsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbActaAsistenciumsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbActaAsistenciums
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbActaAsistencia.Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbActaAsistenciums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbActaAsistencia == null)
            {
                return NotFound();
            }

            var tbActaAsistencium = await _context.TbActaAsistencia
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActaAsistencia == id);
            if (tbActaAsistencium == null)
            {
                return NotFound();
            }

            return View(tbActaAsistencium);
        }

        // GET: TbActaAsistenciums/Create
        public IActionResult Create()
        {
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            return View();
        }

        // POST: TbActaAsistenciums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdActaAsistencia,IdAsociado")] TbActaAsistencium tbActaAsistencium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbActaAsistencium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActaAsistencium.IdAsociado);
            return View(tbActaAsistencium);
        }

        // GET: TbActaAsistenciums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbActaAsistencia == null)
            {
                return NotFound();
            }

            var tbActaAsistencium = await _context.TbActaAsistencia.FindAsync(id);
            if (tbActaAsistencium == null)
            {
                return NotFound();
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActaAsistencium.IdAsociado);
            return View(tbActaAsistencium);
        }

        // POST: TbActaAsistenciums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActaAsistencia,IdAsociado")] TbActaAsistencium tbActaAsistencium)
        {
            if (id != tbActaAsistencium.IdActaAsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbActaAsistencium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbActaAsistenciumExists(tbActaAsistencium.IdActaAsistencia))
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
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActaAsistencium.IdAsociado);
            return View(tbActaAsistencium);
        }

        // GET: TbActaAsistenciums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbActaAsistencia == null)
            {
                return NotFound();
            }

            var tbActaAsistencium = await _context.TbActaAsistencia
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActaAsistencia == id);
            if (tbActaAsistencium == null)
            {
                return NotFound();
            }

            return View(tbActaAsistencium);
        }

        // POST: TbActaAsistenciums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbActaAsistencia == null)
            {
                return Problem("Entity set 'UcgdbContext.TbActaAsistencia'  is null.");
            }
            var tbActaAsistencium = await _context.TbActaAsistencia.FindAsync(id);
            if (tbActaAsistencium != null)
            {
                _context.TbActaAsistencia.Remove(tbActaAsistencium);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbActaAsistenciumExists(int id)
        {
          return (_context.TbActaAsistencia?.Any(e => e.IdActaAsistencia == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
