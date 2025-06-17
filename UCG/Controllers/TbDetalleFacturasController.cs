using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbDetalleFacturasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbDetalleFacturasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbDetalleFacturas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbDetalleFacturas.Include(t => t.IdFacturaNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbDetalleFacturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbDetalleFacturas == null)
            {
                return NotFound();
            }

            var tbDetalleFactura = await _context.TbDetalleFacturas
                .Include(t => t.IdFacturaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleFactura == id);
            if (tbDetalleFactura == null)
            {
                return NotFound();
            }

            return View(tbDetalleFactura);
        }

        // GET: TbDetalleFacturas/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                // Viene del menú general: mostrar combo de facturas
                ViewData["IdFactura"] = new SelectList(_context.TbFacturas.ToList(), "IdFactura", "NumeroFactura");
                return View(new DetalleFacturaViewModel());
            }

            // Viene desde DetalleFactura por factura: fijar factura
            var factura = _context.TbFacturas.FirstOrDefault(f => f.IdFactura == id);
            if (factura == null)
                return NotFound();

            var model = new DetalleFacturaViewModel
            {
                IdFactura = id.Value
            };

            ViewData["IdFactura"] = id; // útil para JS si se necesita
            return View(model);
        }

        // POST: Crear detalle factura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleFacturaViewModel model)
        {
            var validator = new DetalleFacturaViewModelValidator(_context);
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
                var detalle = MapearDetalleFactura(model);

                _context.TbDetalleFacturas.Add(detalle);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Detalle de factura agregado correctamente.";
                return RedirectToAction("Create", "TbDetalleFacturas", new { id = model.IdFactura });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el detalle.";
                await PrepararViewDataAsync(model);
                return View(model);
            }
        }

        private TbDetalleFactura MapearDetalleFactura(DetalleFacturaViewModel model)
        {
            return new TbDetalleFactura
            {
                IdFactura = model.IdFactura,
                Descripcion = model.Descripcion,
                Unidad = model.Unidad,
                Cantidad = model.Cantidad,
                PorcentajeIva = model.PorcentajeIva,
                PrecioUnitario = model.PrecioUnitario,
                PorcentajeDescuento = model.PorcentajeDescuento,
                Descuento = model.Descuento,
                MontoIva = model.MontoIva,
                BaseImponible = model.BaseImponible,
                TotalLinea = model.TotalLinea
            };
        }

        private async Task PrepararViewDataAsync(DetalleFacturaViewModel model)
        {
            var facturas = await _context.TbFacturas.ToListAsync();
            ViewData["IdFactura"] = new SelectList(facturas, "IdFactura", "NumeroFactura", model.IdFactura);
        }

        // GET: TbDetalleFacturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbDetalleFacturas == null)
            {
                return NotFound();
            }

            var tbDetalleFactura = await _context.TbDetalleFacturas.FindAsync(id);
            if (tbDetalleFactura == null)
            {
                return NotFound();
            }
            ViewData["IdFactura"] = new SelectList(_context.TbFacturas, "IdFactura", "IdFactura", tbDetalleFactura.IdFactura);
            return View(tbDetalleFactura);
        }

        // POST: TbDetalleFacturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalleFactura,IdFactura,Descripcion,Unidad,Cantidad,PorcentajeIva,PrecioUnitario,PorcentajeDescuento,Descuento,MontoIva,BaseImponible,TotalLinea")] TbDetalleFactura tbDetalleFactura)
        {
            if (id != tbDetalleFactura.IdDetalleFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbDetalleFactura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbDetalleFacturaExists(tbDetalleFactura.IdDetalleFactura))
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
            ViewData["IdFactura"] = new SelectList(_context.TbFacturas, "IdFactura", "IdFactura", tbDetalleFactura.IdFactura);
            return View(tbDetalleFactura);
        }

        // GET: TbDetalleFacturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbDetalleFacturas == null)
            {
                return NotFound();
            }

            var tbDetalleFactura = await _context.TbDetalleFacturas
                .Include(t => t.IdFacturaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleFactura == id);
            if (tbDetalleFactura == null)
            {
                return NotFound();
            }

            return View(tbDetalleFactura);
        }

        // POST: TbDetalleFacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbDetalleFacturas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbDetalleFacturas'  is null.");
            }
            var tbDetalleFactura = await _context.TbDetalleFacturas.FindAsync(id);
            if (tbDetalleFactura != null)
            {
                _context.TbDetalleFacturas.Remove(tbDetalleFactura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbDetalleFacturaExists(int id)
        {
          return (_context.TbDetalleFacturas?.Any(e => e.IdDetalleFactura == id)).GetValueOrDefault();
        }
    }
}
