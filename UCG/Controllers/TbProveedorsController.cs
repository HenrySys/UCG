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
    public class TbProveedorsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbProveedorsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbProveedors
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbProveedors.Include(t => t.IdAsociacionNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbProveedors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbProveedors == null)
            {
                return NotFound();
            }

            var tbProveedor = await _context.TbProveedors
                .Include(t => t.IdAsociacionNavigation)
                .Include(p => p.TbFacturas)
                .FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (tbProveedor == null)
            {
                return NotFound();
            }

            return View(tbProveedor);
        }

        // GET: TbProveedors/Create
        public async Task<IActionResult> Create()
        {
            var model = new ProveedorViewModel();
            await ConfigurarAsociacionProveedorAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorViewModel model)
        {
            var validator = new ProveedorViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionProveedorAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoProveedor = MapearProveedor(model);
                _context.TbProveedors.Add(nuevoProveedor);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Proveedor creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el proveedor.";
                await ConfigurarAsociacionProveedorAsync(model);
                return View(model);
            }
        }

        private TbProveedor MapearProveedor(ProveedorViewModel model)
        {
            return new TbProveedor
            {
                IdProveedor = model.IdProveedor,
                IdAsociacion = model.IdAsociacion!.Value,
                TipoProveedor = model.TipoProveedor!.Value,
                NombreEmpresa = model.NombreEmpresa!,
                CedulaJuridica = model.CedulaJuridica!,
                NombreContacto = model.NombreContacto!,
                CedulaContacto = model.CedulaContacto!,
                Telefono = model.Telefono!,
                Correo = model.Correo!,
                Direccion = model.Direccion!,
                Fax = model.Fax!,
                Descripcion = model.Descripcion!,
                Estado = model.Estado
            };
        }

        private async Task ConfigurarAsociacionProveedorAsync(ProveedorViewModel model)
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
            if (id == null || _context.TbProveedors == null)
                return NotFound();

            var entidad = await _context.TbProveedors.FindAsync(id);
            if (entidad == null)
                return NotFound();

            var model = new ProveedorViewModel
            {
                IdProveedor = entidad.IdProveedor,
                IdAsociacion = entidad.IdAsociacion,
                TipoProveedor = entidad.TipoProveedor,
                NombreEmpresa = entidad.NombreEmpresa,
                CedulaJuridica = entidad.CedulaJuridica,
                NombreContacto = entidad.NombreContacto,
                CedulaContacto = entidad.CedulaContacto,
                Telefono = entidad.Telefono,
                Correo = entidad.Correo,
                Direccion = entidad.Direccion,
                Fax = entidad.Fax,
                Descripcion = entidad.Descripcion,
                Estado = entidad.Estado!.Value
            };

            await ConfigurarAsociacionProveedorAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProveedorViewModel model)
        {
            var validator = new ProveedorViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionProveedorAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var proveedor = await _context.TbProveedors
                    .FirstOrDefaultAsync(p => p.IdProveedor == model.IdProveedor);

                if (proveedor == null)
                    return NotFound();

                proveedor.IdAsociacion = model.IdAsociacion!.Value;
                proveedor.TipoProveedor = model.TipoProveedor!.Value;
                proveedor.NombreEmpresa = model.NombreEmpresa!;
                proveedor.CedulaJuridica = model.CedulaJuridica!;
                proveedor.NombreContacto = model.NombreContacto!;
                proveedor.CedulaContacto = model.CedulaContacto!;
                proveedor.Telefono = model.Telefono!;
                proveedor.Correo = model.Correo!;
                proveedor.Direccion = model.Direccion!;
                proveedor.Fax = model.Fax!;
                proveedor.Descripcion = model.Descripcion!;
                proveedor.Estado = model.Estado;

                _context.Update(proveedor);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Proveedor actualizado correctamente.";
                return RedirectToAction(nameof(Edit), new { id = model.IdProveedor });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el proveedor.";
                await ConfigurarAsociacionProveedorAsync(model);
                return View(model);
            }
        }


        // GET: TbProveedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbProveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.TbProveedors
                .Include(p => p.IdAsociacionNavigation)
                .FirstOrDefaultAsync(p => p.IdProveedor == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: TbProveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var proveedor = await _context.TbProveedors.FindAsync(id);

                if (proveedor == null)
                {
                    TempData["ErrorMessage"] = "El proveedor no fue encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.TbProveedors.Remove(proveedor);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El proveedor fue eliminado correctamente.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al eliminar el proveedor: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        private bool TbProveedorExists(int id)
        {
          return (_context.TbProveedors?.Any(e => e.IdProveedor == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
