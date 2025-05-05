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
                .FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (tbProveedor == null)
            {
                return NotFound();
            }

            return View(tbProveedor);
        }

        // GET: TbProveedors/Create
        public IActionResult Create()
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var model = new ProveedorViewModel();

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                bool tieneAsociacion = int.TryParse(idAsociacionClaim, out int idAsociacion);

                // Obtener el nombre de la asociación desde la base de datos
                var Nombre = _context.TbAsociacions
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => a.Nombre)
                .FirstOrDefault();

                // Se mantiene seleccionable el usuario
                ViewBag.IdAsociacion = idAsociacion;
                ViewBag.Nombre = Nombre;
                ViewBag.EsAdmin = true;
                model.IdAsociacion = idAsociacion;

                return View(model);
            }
            else
            {
                ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
                ViewBag.EsAdmin = false;
                return View();
            }
            // ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            // return View();
        }

        // POST: TbProveedors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProveedor,IdAsociacion,TipoProveedor,NombreEmpresa,CedulaJuridica,NombreContacto,CedulaContacto,Direccion,Telefono,Correo,Fax,Estado")] TbProveedor tbProveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbProveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbProveedor.IdAsociacion);
            return View(tbProveedor);
        }

        // GET: TbProveedors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbProveedors == null)
            {
                return NotFound();
            }

            var tbProveedor = await _context.TbProveedors.FindAsync(id);
            if (tbProveedor == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbProveedor.IdAsociacion);
            return View(tbProveedor);
        }

        // POST: TbProveedors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProveedor,IdAsociacion,TipoProveedor,NombreEmpresa,CedulaJuridica,NombreContacto,CedulaContacto,Direccion,Telefono,Correo,Fax,Estado")] TbProveedor tbProveedor)
        {
            if (id != tbProveedor.IdProveedor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbProveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbProveedorExists(tbProveedor.IdProveedor))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbProveedor.IdAsociacion);
            return View(tbProveedor);
        }

        // GET: TbProveedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbProveedors == null)
            {
                return NotFound();
            }

            var tbProveedor = await _context.TbProveedors
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (tbProveedor == null)
            {
                return NotFound();
            }

            return View(tbProveedor);
        }

        // POST: TbProveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbProveedors == null)
            {
                return Problem("Entity set 'UcgdbContext.TbProveedors'  is null.");
            }
            var tbProveedor = await _context.TbProveedors.FindAsync(id);
            if (tbProveedor != null)
            {
                _context.TbProveedors.Remove(tbProveedor);
            }
            
            await _context.SaveChangesAsync();
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
