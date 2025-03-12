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
    public class TbAcuerdoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbAcuerdoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbAcuerdoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbAcuerdos.Include(t => t.IdActaNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbAcuerdoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbAcuerdos == null)
            {
                return NotFound();
            }

            var tbAcuerdo = await _context.TbAcuerdos
                .Include(t => t.IdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdAcuerdo == id);
            if (tbAcuerdo == null)
            {
                return NotFound();
            }

            return View(tbAcuerdo);
        }

        // GET: TbAcuerdoes/Create
        public IActionResult Create()
        {
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");
            return View();
        }

        // POST: TbAcuerdoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAcuerdo,IdActa,NumeroAcuerdo,Nombre,Descripcion,MontoAcuerdo")] TbAcuerdo tbAcuerdo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbAcuerdo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbAcuerdo.IdActa);
            return View(tbAcuerdo);
        }

        // GET: TbAcuerdoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbAcuerdos == null)
            {
                return NotFound();
            }

            var tbAcuerdo = await _context.TbAcuerdos.FindAsync(id);
            if (tbAcuerdo == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbAcuerdo.IdActa);
            return View(tbAcuerdo);
        }

        // POST: TbAcuerdoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAcuerdo,IdActa,NumeroAcuerdo,Nombre,Descripcion,MontoAcuerdo")] TbAcuerdo tbAcuerdo)
        {
            if (id != tbAcuerdo.IdAcuerdo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbAcuerdo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbAcuerdoExists(tbAcuerdo.IdAcuerdo))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbAcuerdo.IdActa);
            return View(tbAcuerdo);
        }

        // GET: TbAcuerdoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbAcuerdos == null)
            {
                return NotFound();
            }

            var tbAcuerdo = await _context.TbAcuerdos
                .Include(t => t.IdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdAcuerdo == id);
            if (tbAcuerdo == null)
            {
                return NotFound();
            }

            return View(tbAcuerdo);
        }

        // POST: TbAcuerdoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbAcuerdos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbAcuerdos'  is null.");
            }
            var tbAcuerdo = await _context.TbAcuerdos.FindAsync(id);
            if (tbAcuerdo != null)
            {
                _context.TbAcuerdos.Remove(tbAcuerdo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbAcuerdoExists(int id)
        {
          return (_context.TbAcuerdos?.Any(e => e.IdAcuerdo == id)).GetValueOrDefault();
        }
    }
}
