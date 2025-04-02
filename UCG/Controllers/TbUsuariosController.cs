using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
//    public class TbUsuariosController : Controller
//    {
//        private readonly UcgdbContext _context;

//        public TbUsuariosController(UcgdbContext context)
//        {
//            _context = context;
//        }

//        // GET: TbUsuarios
//        public async Task<IActionResult> Index()
//        {
//              return _context.TbUsuarios != null ? 
//                          View(await _context.TbUsuarios.ToListAsync()) :
//                          Problem("Entity set 'UcgdbContext.TbUsuarios'  is null.");
//        }

//        // GET: TbUsuarios/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null || _context.TbUsuarios == null)
//            {
//                return NotFound();
//            }

//            var tbUsuario = await _context.TbUsuarios
//                .FirstOrDefaultAsync(m => m.IdUsuario == id);
//            if (tbUsuario == null)
//            {
//                return NotFound();
//            }

//            return View(tbUsuario);
//        }

//        // GET: TbUsuarios/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: TbUsuarios/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create( UsuarioViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var usuario = new TbUsuario()
//                {
//                    NombreUsuario = model.NombreUsuario,
//                    Contraseña = model.Contraseña,
//                    Rol = model.Rol,
//                    Estado = model.Estado,
//                    Correo = model.Correo

//                };
//                _context.Add(usuario);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(model );
//        }

//        // GET: TbUsuarios/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null || _context.TbUsuarios == null)
//            {
//                return NotFound();
//            }

//            var tbUsuario = await _context.TbUsuarios.FindAsync(id);
//            if (tbUsuario == null)
//            {
//                return NotFound();
//            }
//            return View(tbUsuario);
//        }

//        // POST: TbUsuarios/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,NombreUsuario,Contraseña,Rol,Correo,Estado")] TbUsuario tbUsuario)
//        {
//            if (id != tbUsuario.IdUsuario)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(tbUsuario);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!TbUsuarioExists(tbUsuario.IdUsuario))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(tbUsuario);
//        }

//        // GET: TbUsuarios/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null || _context.TbUsuarios == null)
//            {
//                return NotFound();
//            }

//            var tbUsuario = await _context.TbUsuarios
//                .FirstOrDefaultAsync(m => m.IdUsuario == id);
//            if (tbUsuario == null)
//            {
//                return NotFound();
//            }

//            return View(tbUsuario);
//        }

//        // POST: TbUsuarios/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            if (_context.TbUsuarios == null)
//            {
//                return Problem("Entity set 'UcgdbContext.TbUsuarios'  is null.");
//            }
//            var tbUsuario = await _context.TbUsuarios.FindAsync(id);
//            if (tbUsuario != null)
//            {
//                _context.TbUsuarios.Remove(tbUsuario);
//            }
            
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool TbUsuarioExists(int id)
//        {
//          return (_context.TbUsuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
//        }
//    }
}
