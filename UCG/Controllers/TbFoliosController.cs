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
using System.Globalization;
using UCG.Models.ValidationModels;

namespace UCG.Controllers
{
    public class TbFoliosController : Controller
    {
        private readonly UcgdbContext _context;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        public TbFoliosController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbFolios
        public async Task<IActionResult> Index()
        {
            IQueryable<TbFolio> filtroAsociacion = _context.TbFolios.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
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
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los folios.";
                return RedirectToAction("Error");
            }
        }

        // GET: TbFolios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbFolios == null)
            {
                return NotFound();
            }

            var tbFolio = await _context.TbFolios
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.TbActa)
                .FirstOrDefaultAsync(m => m.IdFolio == id);
            if (tbFolio == null)
            {
                return NotFound();
            }

            return View(tbFolio);
        }

        public async Task<IActionResult> Create()
        {
            var model = new FolioViewModel();
            await ConfigurarAsociacionFolioAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FolioViewModel model)
        {
            // Validación de fechas
            var fechaEmisionValida = await ParseFechaEmisionAsync(model);
            var fechaCierreValida = await ParseFechaCierreAsync(model);

            if (!fechaEmisionValida /*|| !fechaCierreValida*/)
            {
                await ConfigurarAsociacionFolioAsync(model);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionFolioAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var nuevoFolio = MapearFolio(model);
                _context.TbFolios.Add(nuevoFolio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Folio creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el folio.";
                await ConfigurarAsociacionFolioAsync(model);
                return View(model);
            }
        }


        [HttpGet]
        public JsonResult ObtenerAsociadosPorAsociacion(int idAsociacion)
        {
            try
            {
                var asociados = _context.TbAsociados
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .Select(a => new {
                        a.IdAsociado,
                        a.Nombre
                    })
                    .ToList();

                if (!asociados.Any())
                {
                    return Json(new { success = false, message = "No hay asociados disponibles." });
                }

                return Json(new { success = true, data = asociados });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los asociados: " + ex.Message });
            }
        }


        private TbFolio MapearFolio(FolioViewModel model)
        {
            return new TbFolio
            {
                IdFolio = model.IdFolio,
                IdAsociacion = model.IdAsociacion!.Value,
                IdAsociado = model.IdAsociado!.Value,
                FechaEmision = model.FechaEmision,
                FechaCierre = model.FechaCierre,
                NumeroFolio = model.NumeroFolio!,
                Descripcion = model.Descripcion!,
                Estado = model.Estado!.Value
            };
        }


        private async Task<bool> ParseFechaEmisionAsync(FolioViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoEmision))
            {
                if (DateOnly.TryParseExact(model.FechaTextoEmision, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaEmision = fecha;
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = "Debe ingresar una fecha de emision válida.";
                    TempData.Keep("ErrorMessage");

                    return false;
                }
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de emision válida.";
            TempData.Keep("ErrorMessage");

            return false;
        }

        private async Task<bool> ParseFechaCierreAsync(FolioViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaTextoCierre))
            {
                if (DateOnly.TryParseExact(model.FechaTextoCierre, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaCierre = fecha;
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = "Debe ingresar una fecha de cierre válida.";
                    TempData.Keep("ErrorMessage");

                    return false;
                }
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de cierre válida.";
            TempData.Keep("ErrorMessage");

            return false;
        }


        private async Task ConfigurarAsociacionFolioAsync(FolioViewModel model)
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

                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
                }
            }
            else
            {
                ViewBag.EsAdmin = false;

                var asociaciones = await _context.TbAsociacions.ToListAsync();
                ViewData["IdAsociacion"] = new SelectList(asociaciones, "IdAsociacion", "Nombre", model.IdAsociacion);

                if (model.IdAsociacion.HasValue && model.IdAsociacion.Value > 0)
                {
                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == model.IdAsociacion.Value)
                        .ToListAsync();

                    ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
                }
                else
                {
                    ViewData["IdAsociado"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
        }





        // GET: TbFolios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbFolios == null)
                return NotFound();

            var folio = await _context.TbFolios.FindAsync(id);
            if (folio == null)
                return NotFound();

            var model = new FolioViewModel
            {
                IdFolio = folio.IdFolio,
                IdAsociacion = folio.IdAsociacion,
                IdAsociado = folio.IdAsociado,
                FechaEmision = folio.FechaEmision,
                FechaTextoEmision = folio.FechaEmision.ToString("yyyy-MM-dd"),
                FechaCierre = folio.FechaCierre,
                FechaTextoCierre = folio.FechaCierre.ToString("yyyy-MM-dd"),
                NumeroFolio = folio.NumeroFolio,
                Descripcion = folio.Descripcion,
                Estado = folio.Estado
            };

            await ConfigurarAsociacionFolioAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FolioViewModel model)
        {
            var validator = new FolioViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            var fechaEmisionValida = await ParseFechaEmisionAsync(model);
            var fechaCierreValida = await ParseFechaCierreAsync(model);

            if (!fechaEmisionValida || !fechaCierreValida)
            {
                TempData["ErrorMessage"] = "Las fechas ingresadas no son válidas.";
                await ConfigurarAsociacionFolioAsync(model);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionFolioAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var folio = await _context.TbFolios.FirstOrDefaultAsync(f => f.IdFolio == model.IdFolio);
                if (folio == null)
                    return NotFound();

                folio.IdAsociacion = model.IdAsociacion!.Value;
                folio.IdAsociado = model.IdAsociado!.Value;
                folio.FechaEmision = model.FechaEmision;
                folio.FechaCierre = model.FechaCierre;
                folio.NumeroFolio = model.NumeroFolio!;
                folio.Descripcion = model.Descripcion!;
                folio.Estado = model.Estado!.Value;

                _context.Update(folio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Folio actualizado correctamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el folio.";
                await ConfigurarAsociacionFolioAsync(model);
                return View(model);
            }
        }

        // GET: TbFolios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbFolios == null)
            {
                return NotFound();
            }

            var tbFolio = await _context.TbFolios
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdFolio == id);
            if (tbFolio == null)
            {
                return NotFound();
            }

            return View(tbFolio);
        }

        // POST: TbFolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbFolios == null)
            {
                return Problem("Entity set 'UcgdbContext.TbFolios'  is null.");
            }
            var tbFolio = await _context.TbFolios.FindAsync(id);
            if (tbFolio != null)
            {
                _context.TbFolios.Remove(tbFolio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbFolioExists(int id)
        {
          return (_context.TbFolios?.Any(e => e.IdFolio == id)).GetValueOrDefault();
        }
    }
}
