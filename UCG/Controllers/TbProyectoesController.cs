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
    public class TbProyectoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbProyectoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbProyectoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbProyectos.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbProyectoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbProyectos == null)
            {
                return NotFound();
            }

            var tbProyecto = await _context.TbProyectos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdProyecto == id);
            if (tbProyecto == null)
            {
                return NotFound();
            }

            return View(tbProyecto);
        }

        // GET: TbProyectoes/Create
        public IActionResult Create()
        {
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            return View();
        }

        // POST: TbProyectoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProyecto,IdAsociacion,IdActa,IdAsociado,Nombre,Descripcion,MontoTotalDestinado")] TbProyecto tbProyecto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbProyecto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbProyecto.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbProyecto.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbProyecto.IdAsociado);
            return View(tbProyecto);
        }

        // GET: TbProyectoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbProyectos == null)
            {
                return NotFound();
            }

            var tbProyecto = await _context.TbProyectos.FindAsync(id);
            if (tbProyecto == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbProyecto.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbProyecto.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbProyecto.IdAsociado);
            return View(tbProyecto);
        }

        // POST: TbProyectoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProyecto,IdAsociacion,IdActa,IdAsociado,Nombre,Descripcion,MontoTotalDestinado")] TbProyecto tbProyecto)
        {
            if (id != tbProyecto.IdProyecto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbProyecto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbProyectoExists(tbProyecto.IdProyecto))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbProyecto.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbProyecto.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbProyecto.IdAsociado);
            return View(tbProyecto);
        }

        // GET: TbProyectoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbProyectos == null)
            {
                return NotFound();
            }

            var tbProyecto = await _context.TbProyectos
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdProyecto == id);
            if (tbProyecto == null)
            {
                return NotFound();
            }

            return View(tbProyecto);
        }

        // POST: TbProyectoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbProyectos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbProyectos'  is null.");
            }
            var tbProyecto = await _context.TbProyectos.FindAsync(id);
            if (tbProyecto != null)
            {
                _context.TbProyectos.Remove(tbProyecto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbProyectoExists(int id)
        {
          return (_context.TbProyectos?.Any(e => e.IdProyecto == id)).GetValueOrDefault();
        }
    }
}
