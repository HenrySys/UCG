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
    public class TbAsociacionsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbAsociacionsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbAsociacions
        public async Task<IActionResult> Index()
        {
              return _context.TbAsociacions != null ? 
                          View(await _context.TbAsociacions.ToListAsync()) :
                          Problem("Entity set 'UcgdbContext.TbAsociacions'  is null.");
        }

        // GET: TbAsociacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbAsociacions == null)
            {
                return NotFound();
            }

            var tbAsociacion = await _context.TbAsociacions
                .FirstOrDefaultAsync(m => m.IdAsociacion == id);
            if (tbAsociacion == null)
            {
                return NotFound();
            }

            return View(tbAsociacion);
        }

        // GET: TbAsociacions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbAsociacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsociacion,CedulaJuridica,CodigoRegistro,Nombre,FechaConstitucion,Telefono,Fax,Correo,Provincia,Canton,Distrito,Pueblo,Direccion,Descripcion,Estado")] TbAsociacion tbAsociacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbAsociacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbAsociacion);
        }

        // GET: TbAsociacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbAsociacions == null)
            {
                return NotFound();
            }

            var tbAsociacion = await _context.TbAsociacions.FindAsync(id);
            if (tbAsociacion == null)
            {
                return NotFound();
            }
            return View(tbAsociacion);
        }

        // POST: TbAsociacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsociacion,CedulaJuridica,CodigoRegistro,Nombre,FechaConstitucion,Telefono,Fax,Correo,Provincia,Canton,Distrito,Pueblo,Direccion,Descripcion,Estado")] TbAsociacion tbAsociacion)
        {
            if (id != tbAsociacion.IdAsociacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbAsociacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbAsociacionExists(tbAsociacion.IdAsociacion))
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
            return View(tbAsociacion);
        }

        // GET: TbAsociacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbAsociacions == null)
            {
                return NotFound();
            }

            var tbAsociacion = await _context.TbAsociacions
                .FirstOrDefaultAsync(m => m.IdAsociacion == id);
            if (tbAsociacion == null)
            {
                return NotFound();
            }

            return View(tbAsociacion);
        }

        // POST: TbAsociacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbAsociacions == null)
            {
                return Problem("Entity set 'UcgdbContext.TbAsociacions'  is null.");
            }
            var tbAsociacion = await _context.TbAsociacions.FindAsync(id);
            if (tbAsociacion != null)
            {
                _context.TbAsociacions.Remove(tbAsociacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbAsociacionExists(int id)
        {
          return (_context.TbAsociacions?.Any(e => e.IdAsociacion == id)).GetValueOrDefault();
        }
    }
}
