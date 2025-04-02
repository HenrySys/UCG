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
    public class TbAsociadoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbAsociadoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbAsociadoes
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbAsociados.Include(t => t.IdAsociacionNavigation).Include(t => t.IdUsuarioNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbAsociadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbAsociados == null)
            {
                return NotFound();
            }

            var tbAsociado = await _context.TbAsociados
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsociado == id);
            if (tbAsociado == null)
            {
                return NotFound();
            }

            return View(tbAsociado);
        }

        // GET: TbAsociadoes/Create
        public IActionResult Create()
        {
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion");
            ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: TbAsociadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsociado,IdAsociacion,IdUsuario,Nacionalidad,Cedula,Apellido1,Apellido2,Nombre,FechaNacimiento,Sexo,EstadoCivil,Telefono,Correo,Direccion,Estado")] TbAsociado tbAsociado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbAsociado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbAsociado.IdAsociacion);
            ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "IdUsuario", tbAsociado.IdUsuario);
            return View(tbAsociado);
        }

        // GET: TbAsociadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbAsociados == null)
            {
                return NotFound();
            }

            var tbAsociado = await _context.TbAsociados.FindAsync(id);
            if (tbAsociado == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbAsociado.IdAsociacion);
            ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "IdUsuario", tbAsociado.IdUsuario);
            return View(tbAsociado);
        }

        // POST: TbAsociadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsociado,IdAsociacion,IdUsuario,Nacionalidad,Cedula,Apellido1,Apellido2,Nombre,FechaNacimiento,Sexo,EstadoCivil,Telefono,Correo,Direccion,Estado")] TbAsociado tbAsociado)
        {
            if (id != tbAsociado.IdAsociado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbAsociado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbAsociadoExists(tbAsociado.IdAsociado))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbAsociado.IdAsociacion);
            ViewData["IdUsuario"] = new SelectList(_context.TbUsuarios, "IdUsuario", "IdUsuario", tbAsociado.IdUsuario);
            return View(tbAsociado);
        }

        // GET: TbAsociadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbAsociados == null)
            {
                return NotFound();
            }

            var tbAsociado = await _context.TbAsociados
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAsociado == id);
            if (tbAsociado == null)
            {
                return NotFound();
            }

            return View(tbAsociado);
        }

        // POST: TbAsociadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbAsociados == null)
            {
                return Problem("Entity set 'UcgdbContext.TbAsociados'  is null.");
            }
            var tbAsociado = await _context.TbAsociados.FindAsync(id);
            if (tbAsociado != null)
            {
                _context.TbAsociados.Remove(tbAsociado);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbAsociadoExists(int id)
        {
          return (_context.TbAsociados?.Any(e => e.IdAsociado == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
