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
    public class TbMovimientoEgresosController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMovimientoEgresosController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMovimientoEgresos
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMovimientoEgresos.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMovimientoEgresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var tbMovimientoEgreso = await _context.TbMovimientoEgresos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoEgreso == id);
            if (tbMovimientoEgreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoEgreso);
        }

        // GET: TbMovimientoEgresos/Create
        public IActionResult Create()
        {
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            ViewData["IdConceptoAsociacion"] = new SelectList(_context.TbConceptoAsociacions, "IdConceptoAsociacion", "IdConceptoAsociacion");
            return View();
        }

        // POST: TbMovimientoEgresos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMovimientoEgreso,IdAsociacion,IdAsociado,IdConceptoAsociacion,IdActa,Monto,Fecha,Descripcion")] TbMovimientoEgreso tbMovimientoEgreso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbMovimientoEgreso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimientoEgreso.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoEgreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoEgreso.IdAsociado);
            return View(tbMovimientoEgreso);
        }

        // GET: TbMovimientoEgresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var tbMovimientoEgreso = await _context.TbMovimientoEgresos.FindAsync(id);
            if (tbMovimientoEgreso == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimientoEgreso.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoEgreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoEgreso.IdAsociado);
            return View(tbMovimientoEgreso);
        }

        // POST: TbMovimientoEgresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimientoEgreso,IdAsociacion,IdAsociado,IdConceptoAsociacion,IdActa,Monto,Fecha,Descripcion")] TbMovimientoEgreso tbMovimientoEgreso)
        {
            if (id != tbMovimientoEgreso.IdMovimientoEgreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMovimientoEgreso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMovimientoEgresoExists(tbMovimientoEgreso.IdMovimientoEgreso))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbMovimientoEgreso.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoEgreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoEgreso.IdAsociado);
            return View(tbMovimientoEgreso);
        }

        // GET: TbMovimientoEgresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientoEgresos == null)
            {
                return NotFound();
            }

            var tbMovimientoEgreso = await _context.TbMovimientoEgresos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoEgreso == id);
            if (tbMovimientoEgreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoEgreso);
        }

        // POST: TbMovimientoEgresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMovimientoEgresos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbMovimientoEgresos'  is null.");
            }
            var tbMovimientoEgreso = await _context.TbMovimientoEgresos.FindAsync(id);
            if (tbMovimientoEgreso != null)
            {
                _context.TbMovimientoEgresos.Remove(tbMovimientoEgreso);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMovimientoEgresoExists(int id)
        {
          return (_context.TbMovimientoEgresos?.Any(e => e.IdMovimientoEgreso == id)).GetValueOrDefault();
        }
    }
}
