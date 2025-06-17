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
    public class TbMovimientoIngresosController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMovimientoIngresosController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMovimientoIngresos
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMovimientoIngresos.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMovimientoIngresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMovimientoIngresos == null)
            {
                return NotFound();
            }

            var tbMovimientoIngreso = await _context.TbMovimientoIngresos
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoIngreso == id);
            if (tbMovimientoIngreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoIngreso);
        }

        // GET: TbMovimientoIngresos/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            return View();
        }

        // POST: TbMovimientoIngresos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMovimientoIngreso,IdAsociacion,IdAsociado,FechaIngreso,Descripcion,MontoTotalIngresado")] TbMovimientoIngreso tbMovimientoIngreso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbMovimientoIngreso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoIngreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoIngreso.IdAsociado);
            return View(tbMovimientoIngreso);
        }

        // GET: TbMovimientoIngresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMovimientoIngresos == null)
            {
                return NotFound();
            }

            var tbMovimientoIngreso = await _context.TbMovimientoIngresos.FindAsync(id);
            if (tbMovimientoIngreso == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoIngreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoIngreso.IdAsociado);
            return View(tbMovimientoIngreso);
        }

        // POST: TbMovimientoIngresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimientoIngreso,IdAsociacion,IdAsociado,FechaIngreso,Descripcion,MontoTotalIngresado")] TbMovimientoIngreso tbMovimientoIngreso)
        {
            if (id != tbMovimientoIngreso.IdMovimientoIngreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMovimientoIngreso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMovimientoIngresoExists(tbMovimientoIngreso.IdMovimientoIngreso))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbMovimientoIngreso.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMovimientoIngreso.IdAsociado);
            return View(tbMovimientoIngreso);
        }

        // GET: TbMovimientoIngresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMovimientoIngresos == null)
            {
                return NotFound();
            }

            var tbMovimientoIngreso = await _context.TbMovimientoIngresos
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimientoIngreso == id);
            if (tbMovimientoIngreso == null)
            {
                return NotFound();
            }

            return View(tbMovimientoIngreso);
        }

        // POST: TbMovimientoIngresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMovimientoIngresos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbMovimientoIngresos'  is null.");
            }
            var tbMovimientoIngreso = await _context.TbMovimientoIngresos.FindAsync(id);
            if (tbMovimientoIngreso != null)
            {
                _context.TbMovimientoIngresos.Remove(tbMovimientoIngreso);
                TempData["SuccessMessage"] = "El Movimiento Ingreso fue eliminado correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMovimientoIngresoExists(int id)
        {
          return (_context.TbMovimientoIngresos?.Any(e => e.IdMovimientoIngreso == id)).GetValueOrDefault();
        }
    }
}
