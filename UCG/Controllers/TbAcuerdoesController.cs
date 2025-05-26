using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

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

        [HttpGet]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                // Viene del menú general: mostrar combo con todas las actas
                ViewData["IdActa"] = new SelectList(_context.TbActa.ToList(), "IdActa", "NumeroActa");
                return View(new AcuerdoViewModel());
            }

            // Viene desde Detalle de Acta: fijar el IdActa
            var acta = _context.TbActa.FirstOrDefault(a => a.IdActa == id);
            var model = new AcuerdoViewModel
            {
                IdActa = id.Value
            };

            ViewData["IdActa"] = id; // Para pasarlo al JS si hace falta
            return View(model);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcuerdoViewModel model)
        {
            var validator = new AcuerdoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var nuevoAcuerdo = MapearAcuerdo(model);

                _context.TbAcuerdos.Add(nuevoAcuerdo);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Acuerdo agregado correctamente.";
                return RedirectToAction("Create", "TbAcuerdoes", new { id = model.IdActa });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el acuerdo.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

        }

        private TbAcuerdo MapearAcuerdo(AcuerdoViewModel model)
        {
            return new TbAcuerdo
            {
             
                IdActa = model.IdActa!.Value,
                NumeroAcuerdo = model.NumeroAcuerdo,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Tipo = model.Tipo,
            };
        }


        private async Task PrepararViewDataAsync(AcuerdoViewModel model)
        {
            ViewData["IdActa"] = new SelectList(
                await _context.TbAsociacions.ToListAsync(),
                "IdActa",
                "NumeroActa",
                model.IdActa);
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
        
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
