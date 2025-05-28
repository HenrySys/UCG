using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;
using UCG.Models.ValidationModels;

namespace UCG.Controllers
{
    public class TbConceptoMovimientoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbConceptoMovimientoesController(UcgdbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var conceptos = await _context.TbConceptoMovimientos.ToListAsync();
            return View(conceptos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var concepto = await _context.TbConceptoMovimientos
                .FirstOrDefaultAsync(c => c.IdConceptoMovimiento == id);

            if (concepto == null)
                return NotFound();

            return View(concepto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConceptoMovimientoViewModel model)
        {
            var validator = new ConceptoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entidad = new TbConceptoMovimiento
                {
                    TipoMovimiento = model.TipoMovimiento,
                    Concepto = model.Concepto,
                    TipoOrigenIngreso = model.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Ingreso ? model.TipoOrigenIngreso : null,
                    TipoEmisorEgreso = model.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Egreso ? model.TipoEmisorEgreso : null
                };

                _context.Add(entidad);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Concepto creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el concepto.";
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var entidad = await _context.TbConceptoMovimientos.FindAsync(id);
            if (entidad == null)
                return NotFound();

            var model = new ConceptoMovimientoViewModel
            {
                IdConceptoMovimiento = entidad.IdConceptoMovimiento,
                TipoMovimiento = entidad.TipoMovimiento,
                Concepto = entidad.Concepto,
                TipoOrigenIngreso = entidad.TipoOrigenIngreso,
                TipoEmisorEgreso = entidad.TipoEmisorEgreso
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConceptoMovimientoViewModel model)
        {
            if (id != model.IdConceptoMovimiento)
                return NotFound();

            var validator = new ConceptoViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entidad = await _context.TbConceptoMovimientos.FindAsync(id);
                if (entidad == null)
                    return NotFound();

                entidad.TipoMovimiento = model.TipoMovimiento;
                entidad.Concepto = model.Concepto;
                entidad.TipoOrigenIngreso = model.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Ingreso ? model.TipoOrigenIngreso : null;
                entidad.TipoEmisorEgreso = model.TipoMovimiento == TbConceptoMovimiento.TiposDeConceptoMovimientos.Egreso ? model.TipoEmisorEgreso : null;

                _context.Update(entidad);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Concepto actualizado correctamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el concepto.";
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var entidad = await _context.TbConceptoMovimientos
                .FirstOrDefaultAsync(c => c.IdConceptoMovimiento == id);

            if (entidad == null)
                return NotFound();

            return View(entidad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entidad = await _context.TbConceptoMovimientos.FindAsync(id);
            if (entidad != null)
            {
                _context.TbConceptoMovimientos.Remove(entidad);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Concepto eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TbConceptoMovimientoExists(int id)
        {
            return _context.TbConceptoMovimientos.Any(e => e.IdConceptoMovimiento == id);
        }
    }
}
