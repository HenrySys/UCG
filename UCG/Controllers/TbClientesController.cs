using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UCG.Models;
using UCG.Models.ViewModels;
using UCG.Models.ValidationModels;

namespace UCG.Controllers
{
    public class TbClientesController : Controller
    {
        private readonly UcgdbContext _context;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        public TbClientesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbClientes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbClientes.Include(t => t.IdAsociacionNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbClientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbClientes == null)
            {
                return NotFound();
            }

            var tbCliente = await _context.TbClientes
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (tbCliente == null)
            {
                return NotFound();
            }

            return View(tbCliente);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ClienteViewModel();
            await ConfigurarAsociacionClienteAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            var validator = new ClienteViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionClienteAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoCliente = MapearCliente(model);
                _context.TbClientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Cliente creado correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el cliente.";
                await ConfigurarAsociacionClienteAsync(model);
                return View(model);
            }
        }


        private TbCliente MapearCliente(ClienteViewModel model)
        {
            return new TbCliente
            {
                IdCliente = model.IdCliente,
                IdAsociacion = model.IdAsociacion.Value,
                Apellido1 = model.Apellido1,
                Apellido2 = model.Apellido2,
                Nombre = model.Nombre,
                Cedula = model.Cedula,
                Telefono = model.Telefono,
                Correo = model.Correo,
                Direccion = model.Direccion,
                Estado = model.Estado.Value
            };
        }



        private async Task ConfigurarAsociacionClienteAsync(ClienteViewModel model)
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
            if (id == null || _context.TbClientes == null)
                return NotFound();

            var entidad = await _context.TbClientes.FindAsync(id);
            if (entidad == null)
                return NotFound();

            var model = new ClienteViewModel
            {
                IdCliente = entidad.IdCliente,
                IdAsociacion = entidad.IdAsociacion,
                Apellido1 = entidad.Apellido1,
                Apellido2 = entidad.Apellido2,
                Nombre = entidad.Nombre,
                Cedula = entidad.Cedula,
                Telefono = entidad.Telefono,
                Correo = entidad.Correo,
                Direccion = entidad.Direccion,
                Estado = entidad.Estado
            };

            await ConfigurarAsociacionClienteAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteViewModel model)
        {
            var validator = new ClienteViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionClienteAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cliente = await _context.TbClientes.FirstOrDefaultAsync(c => c.IdCliente == model.IdCliente);
                if (cliente == null)
                    return NotFound();

                cliente.IdAsociacion = model.IdAsociacion!.Value;
                cliente.Apellido1 = model.Apellido1!;
                cliente.Apellido2 = model.Apellido2;
                cliente.Nombre = model.Nombre!;
                cliente.Cedula = model.Cedula!;
                cliente.Telefono = model.Telefono!;
                cliente.Correo = model.Correo;
                cliente.Direccion = model.Direccion!;
                cliente.Estado = model.Estado!.Value;

                _context.Update(cliente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el cliente.";
                await ConfigurarAsociacionClienteAsync(model);
                return View(model);
            }
        }


        // GET: TbClientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbClientes == null)
            {
                return NotFound();
            }

            var tbCliente = await _context.TbClientes
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (tbCliente == null)
            {
                return NotFound();
            }

            return View(tbCliente);
        }

        // POST: TbClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbClientes == null)
            {
                return Problem("Entity set 'UcgdbContext.TbClientes'  is null.");
            }
            var tbCliente = await _context.TbClientes.FindAsync(id);
            if (tbCliente != null)
            {
                _context.TbClientes.Remove(tbCliente);
                TempData["SuccessMessage"] = "El Cliente fue eliminado correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbClienteExists(int id)
        {
          return (_context.TbClientes?.Any(e => e.IdCliente == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
