using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;
using UCG.Models.ValidationModels;

namespace UCG.Controllers
{
    public class TbConceptoAsociacionsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbConceptoAsociacionsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbConceptoAsociacions
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbConceptoAsociacions.Include(t => t.IdAsociacionNavigation).Include(t => t.IdConceptoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbConceptoAsociacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbConceptoAsociacions == null)
            {
                return NotFound();
            }

            var tbConceptoAsociacion = await _context.TbConceptoAsociacions
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdConceptoNavigation)
                .FirstOrDefaultAsync(m => m.IdConceptoAsociacion == id);
            if (tbConceptoAsociacion == null)
            {
                return NotFound();
            }

            return View(tbConceptoAsociacion);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ConceptoAsociacionViewModel();
            await ConfigurarAsociacionAsync(model);

            ViewData["IdConcepto"] = new SelectList(
                await _context.TbConceptoMovimientos.ToListAsync(),
                "IdConceptoMovimiento", "Concepto");

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConceptoAsociacionViewModel model)
        {
            var validator = new ConceptoAsociacionViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevo = MapearConceptoAsociacion(model);

                _context.TbConceptoAsociacions.Add(nuevo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Concepto asociado creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el concepto.";
                await ConfigurarAsociacionAsync(model);
                return View(model);
            }
        }
        private TbConceptoAsociacion MapearConceptoAsociacion(ConceptoAsociacionViewModel model)
        {
            return new TbConceptoAsociacion
            {
                IdConceptoAsociacion = model.IdConceptoAsociacion,
                IdAsociacion = model.IdAsociacion!.Value,
                IdConcepto = model.IdConcepto!.Value,
                DescripcionPersonalizada = model.DescripcionPersonalizada
            };
        }
       

        private async Task ConfigurarAsociacionAsync(ConceptoAsociacionViewModel model)
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                if (int.TryParse(idAsociacionClaim, out int idAsociacion))
                {
                    var nombre = await _context.TbAsociacions
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .Select(a => a.Nombre)
                        .FirstOrDefaultAsync();

                    model.IdAsociacion = idAsociacion;
                    ViewBag.IdAsociacion = idAsociacion;
                    ViewBag.Nombre = nombre;
                    ViewBag.EsAdmin = true;
                }
            }
            else
            {
                ViewBag.EsAdmin = false;
                ViewData["IdAsociacion"] = new SelectList(
                    await _context.TbAsociacions.ToListAsync(),
                    "IdAsociacion", "Nombre",
                    model.IdAsociacion);
            }

            // Cargar conceptos disponibles para todos los usuarios
            ViewData["IdConcepto"] = new SelectList(
                await _context.TbConceptoMovimientos.ToListAsync(),
                "IdConceptoMovimiento", "Concepto");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entidad = await _context.TbConceptoAsociacions.FindAsync(id);
            if (entidad == null) return NotFound();

            var model = new ConceptoAsociacionViewModel
            {
                IdConceptoAsociacion = entidad.IdConceptoAsociacion,
                IdAsociacion = entidad.IdAsociacion,
                IdConcepto = entidad.IdConcepto,
                DescripcionPersonalizada = entidad.DescripcionPersonalizada
            };

            await ConfigurarAsociacionAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConceptoAsociacionViewModel model)
        {
            var validator = new ConceptoAsociacionViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existente = await _context.TbConceptoAsociacions
                    .FirstOrDefaultAsync(c => c.IdConceptoAsociacion == model.IdConceptoAsociacion);

                if (existente == null) return NotFound();

                existente.IdAsociacion = model.IdAsociacion!.Value;
                existente.IdConcepto = model.IdConcepto!.Value;
                existente.DescripcionPersonalizada = model.DescripcionPersonalizada;

                _context.Update(existente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Concepto asociado actualizado correctamente.";
                return RedirectToAction(nameof(Edit), new { id = model.IdConceptoAsociacion });
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el concepto.";
                await ConfigurarAsociacionAsync(model);
                return View(model);
            }
        }


        // GET: TbConceptoAsociacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbConceptoAsociacions == null)
            {
                return NotFound();
            }

            var tbConceptoAsociacion = await _context.TbConceptoAsociacions
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdConceptoNavigation)
                .FirstOrDefaultAsync(m => m.IdConceptoAsociacion == id);
            if (tbConceptoAsociacion == null)
            {
                return NotFound();
            }

            return View(tbConceptoAsociacion);
        }

        // POST: TbConceptoAsociacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbConceptoAsociacions == null)
            {
                return Problem("Entity set 'UcgdbContext.TbConceptoAsociacions'  is null.");
            }
            var tbConceptoAsociacion = await _context.TbConceptoAsociacions.FindAsync(id);
            if (tbConceptoAsociacion != null)
            {
                _context.TbConceptoAsociacions.Remove(tbConceptoAsociacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbConceptoAsociacionExists(int id)
        {
          return (_context.TbConceptoAsociacions?.Any(e => e.IdConceptoAsociacion == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
