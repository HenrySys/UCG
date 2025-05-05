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

        // GET: TbCuentums/Create
        public IActionResult Create()
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var model = new CuentumViewModel();

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

        // POST: TbCuentums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCuenta,IdAsociacion,TipoCuenta,TituloCuenta,NumeroCuenta,Telefono,Estado")] TbCuentum tbCuentum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbCuentum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbCuentum.IdAsociacion);
            return View(tbCuentum);
        }

        // GET: TbCuentums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbCuenta == null)
            {
                return NotFound();
            }

            var tbCuentum = await _context.TbCuenta.FindAsync(id);
            if (tbCuentum == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbCuentum.IdAsociacion);
            return View(tbCuentum);
        }

        // POST: TbCuentums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCuenta,IdAsociacion,TipoCuenta,TituloCuenta,NumeroCuenta,Telefono,Estado")] TbCuentum tbCuentum)
        {
            if (id != tbCuentum.IdCuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbCuentum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbCuentumExists(tbCuentum.IdCuenta))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbCuentum.IdAsociacion);
            return View(tbCuentum);
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
