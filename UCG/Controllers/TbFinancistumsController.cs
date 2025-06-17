using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbFinancistumsController : Controller
    {
        private readonly UcgdbContext _context;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        public TbFinancistumsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbFinancistums
        public async Task<IActionResult> Index()
        {
            IQueryable<TbFinancistum> filtroAsociacion = _context.TbFinancista.Include(t => t.IdAsociacionNavigation);
            try
            {
                if (rol == "Admin"){
                    var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                    if (int.TryParse(idAsociacionClaim, out int idAsociacion))
                    {
                        filtroAsociacion = filtroAsociacion.Where(t => t.IdAsociacion == idAsociacion);
                    }
                }
                return View(await filtroAsociacion.ToListAsync());

            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los Financistas.";
                return RedirectToAction("Error");
            }
            // var ucgdbContext = _context.TbFinancista.Include(t => t.IdAsociacionNavigation);
            // return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbFinancistums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbFinancista == null)
            {
                return NotFound();
            }

            var tbFinancistum = await _context.TbFinancista
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdFinancista == id);
            if (tbFinancistum == null)
            {
                return NotFound();
            }

            return View(tbFinancistum);
        }

        // GET: TbFinancistums/Create
        public async Task<IActionResult> Create()
        {
            var model = new FinancistaViewModel();
            await ConfigurarAsociacionFinancistaAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinancistaViewModel model)
        {
            var validator = new FinancistaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionFinancistaAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entidad = MapearFinancista(model);
                _context.TbFinancista.Add(entidad);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Financista creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el financista.";
                await ConfigurarAsociacionFinancistaAsync(model);
                return View(model);
            }
        }


        private TbFinancistum MapearFinancista(FinancistaViewModel model)
        {
            return new TbFinancistum
            {
                IdFinancista = model.IdFinancista,
                IdAsociacion = model.IdAsociacion,
                Nombre = model.Nombre,
                TipoEntidad = model.TipoEntidad!.Value,
                Descripcion = model.Descripcion,
                Telefono = model.Telefono,
                Correo = model.Correo,
                SitioWeb = model.SitioWeb
            };
        }


        private async Task ConfigurarAsociacionFinancistaAsync(FinancistaViewModel model)
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

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
                ViewBag.IdAsociacion = new SelectList(
                    await _context.TbAsociacions.ToListAsync(),
                    "IdAsociacion", "Nombre", model.IdAsociacion);
            }
        }

        // GET: TbFinancistums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var entidad = await _context.TbFinancista.FindAsync(id);
            if (entidad == null)
                return NotFound();

            var model = new FinancistaViewModel
            {
                IdFinancista = entidad.IdFinancista,
                IdAsociacion = entidad.IdAsociacion,
                Nombre = entidad.Nombre,
                TipoEntidad = entidad.TipoEntidad,
                Descripcion = entidad.Descripcion,
                Telefono = entidad.Telefono,
                Correo = entidad.Correo,
                SitioWeb = entidad.SitioWeb
            };

            await ConfigurarAsociacionFinancistaAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FinancistaViewModel model)
        {
            var validator = new FinancistaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionFinancistaAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existente = await _context.TbFinancista
                    .FirstOrDefaultAsync(f => f.IdFinancista == model.IdFinancista);

                if (existente == null)
                    return NotFound();

                existente.IdAsociacion = model.IdAsociacion;
                existente.Nombre = model.Nombre;
                existente.TipoEntidad = model.TipoEntidad!.Value;
                existente.Descripcion = model.Descripcion;
                existente.Telefono = model.Telefono;
                existente.Correo = model.Correo;
                existente.SitioWeb = model.SitioWeb;

                _context.Update(existente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Financista actualizado correctamente.";
                return RedirectToAction(nameof(Edit), new { id = model.IdFinancista });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el financista.";
                await ConfigurarAsociacionFinancistaAsync(model);
                return View(model);
            }
        }


        // GET: TbFinancistums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbFinancista == null)
            {
                return NotFound();
            }

            var tbFinancistum = await _context.TbFinancista
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdFinancista == id);
            if (tbFinancistum == null)
            {
                return NotFound();
            }

            return View(tbFinancistum);
        }

        // POST: TbFinancistums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbFinancista == null)
            {
                return Problem("Entity set 'UcgdbContext.TbFinancista'  is null.");
            }
            var tbFinancistum = await _context.TbFinancista.FindAsync(id);
            if (tbFinancistum != null)
            {
                _context.TbFinancista.Remove(tbFinancistum);
                TempData["SuccessMessage"] = "El financista fue eliminado correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbFinancistumExists(int id)
        {
          return (_context.TbFinancista?.Any(e => e.IdFinancista == id)).GetValueOrDefault();
        }
    }
}
