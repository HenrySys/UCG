using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UCG.Models;

namespace UCG.Controllers
{
    public class TbClientesController : Controller
    {
        private readonly UcgdbContext _context;

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

        // GET: TbClientes/Create
        public IActionResult Create()
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var model = new TbCliente();

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


                ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "NombreUsuario");

                return View(model);
            }
            else
            {
                ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
                ViewBag.EsAdmin = false;
                return View();
            }
            //ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            //return View();
        }

        // POST: TbClientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,IdAsociacion,Apellido1,Apellido2,Nombre,Cedula,Telefono,Correo,Direccion,Estado")] TbCliente tbCliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbCliente.IdAsociacion);
            return View(tbCliente);
        }

        // GET: TbClientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbClientes == null)
            {
                return NotFound();
            }

            var tbCliente = await _context.TbClientes.FindAsync(id);
            if (tbCliente == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbCliente.IdAsociacion);
            return View(tbCliente);
        }

        // POST: TbClientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,IdAsociacion,Apellido1,Apellido2,Nombre,Cedula,Telefono,Correo,Direccion,Estado")] TbCliente tbCliente)
        {
            if (id != tbCliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbCliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbClienteExists(tbCliente.IdCliente))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbCliente.IdAsociacion);
            return View(tbCliente);
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
