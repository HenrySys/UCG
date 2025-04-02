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
    public class TbPuestoesController : Controller
    {
        private readonly UcgdbContext _context;

        public TbPuestoesController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbPuestoes
        public async Task<IActionResult> Index()
        {
            try{
              return _context.TbPuestos != null ? 
                          View(await _context.TbPuestos.ToListAsync()) :
                          Problem("Entity set 'UcgdbContext.TbPuestos'  is null.");
            }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al mostrar Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
        }

        // GET: TbPuestoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbPuestos == null)
            {
                return NotFound();
            }

            try{
            var tbPuesto = await _context.TbPuestos
                .FirstOrDefaultAsync(m => m.IdPuesto == id);
            if (tbPuesto == null)
            {
                return NotFound();
            }

            return View(tbPuesto);
            }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al mostrar Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
        }

        // GET: TbPuestoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbPuestoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPuesto,Nombre,Decripcion")] TbPuesto tbPuesto)
        {
            if (!ModelState.IsValid)
            {
                return View(tbPuesto);
            }
            

            try{
                _context.Add(tbPuesto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al Crear Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
        }

        // GET: TbPuestoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbPuestos == null)
            {
                return NotFound();
            }

            try{
            var tbPuesto = await _context.TbPuestos.FindAsync(id);
            if (tbPuesto == null)
            {
                return NotFound();
            }
            return View(tbPuesto);
            }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al editar Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
        }

        // POST: TbPuestoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPuesto,Nombre,Decripcion")] TbPuesto tbPuesto)
        {
            if (id != tbPuesto.IdPuesto)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tbPuesto);
            }
            
            try
                {
                    _context.Update(tbPuesto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbPuestoExists(tbPuesto.IdPuesto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al editar Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
                
        }

        // GET: TbPuestoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbPuestos == null)
            {
                return NotFound();
            }
            try{
            var tbPuesto = await _context.TbPuestos
                .FirstOrDefaultAsync(m => m.IdPuesto == id);
            if (tbPuesto == null)
            {
                return NotFound();
            }

            return View(tbPuesto);
            }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al Eliminar Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
        }

        // POST: TbPuestoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbPuestos == null)
            {
                return Problem("Entity set 'UcgdbContext.TbPuestos'  is null.");
            }

            try{
            var tbPuesto = await _context.TbPuestos.FindAsync(id);
            if (tbPuesto != null)
            {
                _context.TbPuestos.Remove(tbPuesto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al editar Puestos. Error="+ex;
                return RedirectToAction("Error");
            }
        }

        private bool TbPuestoExists(int id)
        {
          return (_context.TbPuestos?.Any(e => e.IdPuesto == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
