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
    public class TbCuentumsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbCuentumsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbCuentums
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbCuenta.Include(t => t.IdAsociacionNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbCuentums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbCuenta == null)
            {
                return NotFound();
            }

            var tbCuentum = await _context.TbCuenta
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (tbCuentum == null)
            {
                return NotFound();
            }

            return View(tbCuentum);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CuentumViewModel();
            await ConfigurarAsociacionCuentaAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuentumViewModel model)
        {
            var validator = new CuentumViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionCuentaAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevaCuenta = MapearCuenta(model);
                _context.TbCuenta.Add(nuevaCuenta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Cuenta creada correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar la cuenta.";
                await ConfigurarAsociacionCuentaAsync(model);
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


        private TbCuentum MapearCuenta(CuentumViewModel model)
        {
            return new TbCuentum
            {
                IdCuenta = model.IdCuenta,
                IdAsociacion = model.IdAsociacion,
                IdAsociado = model.IdAsociado,
                TipoCuenta = model.TipoCuenta,
                TituloCuenta = model.TituloCuenta!,
                NumeroCuenta = model.NumeroCuenta!,
                Telefono = model.Telefono!,
                Estado = model.Estado!.Value,
                Banco = model.Banco,
                Saldo = model.Saldo
            };
        }

        private async Task ConfigurarAsociacionCuentaAsync(CuentumViewModel model)
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbCuenta == null)
                return NotFound();

            var cuenta = await _context.TbCuenta.FindAsync(id);
            if (cuenta == null)
                return NotFound();

            var model = new CuentumViewModel
            {
                IdCuenta = cuenta.IdCuenta,
                IdAsociacion = cuenta.IdAsociacion,
                IdAsociado = cuenta.IdAsociado,
                TipoCuenta = cuenta.TipoCuenta,
                TituloCuenta = cuenta.TituloCuenta,
                NumeroCuenta = cuenta.NumeroCuenta,
                Telefono = cuenta.Telefono,
                Estado = cuenta.Estado,
                Banco = cuenta.Banco!.Value 
            };

            await ConfigurarAsociacionCuentaAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CuentumViewModel model)
        {
            if (id != model.IdCuenta)
                return NotFound();

            var validator = new CuentumViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionCuentaAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cuentaExistente = await _context.TbCuenta.FindAsync(id);
                if (cuentaExistente == null)
                    return NotFound();

                // Actualizar campos manualmente
                cuentaExistente.IdAsociacion = model.IdAsociacion;
                cuentaExistente.IdAsociado = model.IdAsociado;
                cuentaExistente.TipoCuenta = model.TipoCuenta;
                cuentaExistente.TituloCuenta = model.TituloCuenta;
                cuentaExistente.NumeroCuenta = model.NumeroCuenta;
                cuentaExistente.Telefono = model.Telefono;
                cuentaExistente.Estado = model.Estado;
                cuentaExistente.Banco = model.Banco;

                _context.Update(cuentaExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Cuenta actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar la cuenta.";
                await ConfigurarAsociacionCuentaAsync(model);
                return View(model);
            }
        }


        // GET: TbCuentums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbCuenta == null)
            {
                return NotFound();
            }

            var tbCuentum = await _context.TbCuenta
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdCuenta == id);
            if (tbCuentum == null)
            {
                return NotFound();
            }

            return View(tbCuentum);
        }

        // POST: TbCuentums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbCuenta == null)
            {
                return Problem("Entity set 'UcgdbContext.TbCuenta'  is null.");
            }
            var tbCuentum = await _context.TbCuenta.FindAsync(id);
            if (tbCuentum != null)
            {
                _context.TbCuenta.Remove(tbCuentum);
                TempData["SuccessMessage"] = "La Cuenta fue eliminada correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbCuentumExists(int id)
        {
          return (_context.TbCuenta?.Any(e => e.IdCuenta == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
