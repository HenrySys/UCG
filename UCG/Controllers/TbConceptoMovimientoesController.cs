using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;


namespace UCG.Controllers
{
    public class TbConceptoMovimientoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbConceptoMovimientoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbConceptoMovimientoes

        public async Task<IActionResult> Index()
        {
            return _context.TbConceptoMovimientos != null ?
                        View(await _context.TbConceptoMovimientos.ToListAsync()) :
                        Problem("Entity set 'UcgdbContext.TbConceptoMovimientos'  is null.");
        }

        // GET: TbConceptoMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbConceptoMovimientos == null)
            {
                return NotFound();
            }

            var tbConceptoMovimiento = await _context.TbConceptoMovimientos
                .FirstOrDefaultAsync(m => m.IdConceptoMovimiento == id);
            if (tbConceptoMovimiento == null)
            {
                return NotFound();
            }

            return View(tbConceptoMovimiento);
        }

        // GET: TbConceptoMovimientoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbConceptoMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConceptoMovimiento,TipoMovimiento,Concepto")] ConceptoMovimientoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var concepto = new TbConceptoMovimiento()
                {
                    TipoMovimiento = model.TipoMovimiento,
                    Concepto = model.Concepto

                };

                _context.Add(concepto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: TbConceptoMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbConceptoMovimientos == null)
            {
                return NotFound();
            }

            var tbConceptoMovimiento = await _context.TbConceptoMovimientos.FindAsync(id);
            if (tbConceptoMovimiento == null)
            {
                return NotFound();
            }
            return View(tbConceptoMovimiento);
        }

        // POST: TbConceptoMovimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConceptoMovimiento,TipoMovimiento,Concepto")] TbConceptoMovimiento tbConceptoMovimiento)
        {
            if (id != tbConceptoMovimiento.IdConceptoMovimiento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbConceptoMovimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbConceptoMovimientoExists(tbConceptoMovimiento.IdConceptoMovimiento))
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
            return View(tbConceptoMovimiento);
        }

        // GET: TbConceptoMovimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbConceptoMovimientos == null)
            {
                return NotFound();
            }

            var tbConceptoMovimiento = await _context.TbConceptoMovimientos
                .FirstOrDefaultAsync(m => m.IdConceptoMovimiento == id);
            if (tbConceptoMovimiento == null)
            {
                return NotFound();
            }

            return View(tbConceptoMovimiento);
        }

        // POST: TbConceptoMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbConceptoMovimientos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbConceptoMovimientos'  is null.");
            }
            var tbConceptoMovimiento = await _context.TbConceptoMovimientos.FindAsync(id);
            if (tbConceptoMovimiento != null)
            {
                _context.TbConceptoMovimientos.Remove(tbConceptoMovimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbConceptoMovimientoExists(int id)
        {
            return (_context.TbConceptoMovimientos?.Any(e => e.IdConceptoMovimiento == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
