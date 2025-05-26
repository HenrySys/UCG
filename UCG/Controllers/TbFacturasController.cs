using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;

namespace UCG.Controllers
{
    public class TbFacturasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbFacturasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbFacturas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbFacturas.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation).Include(t => t.IdColaboradorNavigation).Include(t => t.IdProveedorNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbFacturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbFacturas == null)
            {
                return NotFound();
            }

            var tbFactura = await _context.TbFacturas
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdColaboradorNavigation)
                .Include(t => t.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (tbFactura == null)
            {
                return NotFound();
            }

            return View(tbFactura);
        }

        // GET: TbFacturas/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado");
            ViewData["IdColaborador"] = new SelectList(_context.TbColaboradors, "IdColaborador", "IdColaborador");
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor");
            return View();
        }

        // POST: TbFacturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFactura,NumeroFactura,FechaEmision,Descripcion,MontoTotal,IdColaborador,IdProveedor,IdAsociacion,IdAsociado,ArchivoUrl,NombreArchivo,FechaSubida")] TbFactura tbFactura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbFactura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbFactura.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbFactura.IdAsociado);
            ViewData["IdColaborador"] = new SelectList(_context.TbColaboradors, "IdColaborador", "IdColaborador", tbFactura.IdColaborador);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbFactura.IdProveedor);
            return View(tbFactura);
        }

        // GET: TbFacturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbFacturas == null)
            {
                return NotFound();
            }

            var tbFactura = await _context.TbFacturas.FindAsync(id);
            if (tbFactura == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbFactura.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbFactura.IdAsociado);
            ViewData["IdColaborador"] = new SelectList(_context.TbColaboradors, "IdColaborador", "IdColaborador", tbFactura.IdColaborador);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbFactura.IdProveedor);
            return View(tbFactura);
        }

        // POST: TbFacturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFactura,NumeroFactura,FechaEmision,Descripcion,MontoTotal,IdColaborador,IdProveedor,IdAsociacion,IdAsociado,ArchivoUrl,NombreArchivo,FechaSubida")] TbFactura tbFactura)
        {
            if (id != tbFactura.IdFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbFactura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbFacturaExists(tbFactura.IdFactura))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbFactura.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbFactura.IdAsociado);
            ViewData["IdColaborador"] = new SelectList(_context.TbColaboradors, "IdColaborador", "IdColaborador", tbFactura.IdColaborador);
            ViewData["IdProveedor"] = new SelectList(_context.TbProveedors, "IdProveedor", "IdProveedor", tbFactura.IdProveedor);
            return View(tbFactura);
        }

        // GET: TbFacturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbFacturas == null)
            {
                return NotFound();
            }

            var tbFactura = await _context.TbFacturas
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdColaboradorNavigation)
                .Include(t => t.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (tbFactura == null)
            {
                return NotFound();
            }

            return View(tbFactura);
        }

        // POST: TbFacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbFacturas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbFacturas'  is null.");
            }
            var tbFactura = await _context.TbFacturas.FindAsync(id);
            if (tbFactura != null)
            {
                _context.TbFacturas.Remove(tbFactura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbFacturaExists(int id)
        {
          return (_context.TbFacturas?.Any(e => e.IdFactura == id)).GetValueOrDefault();
        }
    }
}
