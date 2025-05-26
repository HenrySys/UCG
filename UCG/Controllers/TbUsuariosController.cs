using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ViewModels;
using UCG.Services;

namespace UCG.Controllers
{
   public class TbUsuariosController : Controller
   {
       private readonly UcgdbContext _context;
       private readonly HashingService _hashingService;
       private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";


       public TbUsuariosController(UcgdbContext context, HashingService hashingService )
       {
           _context = context;
           _hashingService = hashingService;
       }

       // GET: TbUsuarios
       public async Task<IActionResult> Index()
       {
        IQueryable<TbUsuario> filtroAsociacion = _context.TbUsuarios.Include(t => t.IdAsociacionNavigation);
        try{
            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                    if (int.TryParse(idAsociacionClaim, out int idAsociacion))
                    {
                        filtroAsociacion = filtroAsociacion.Where(c => c.IdAsociacion == idAsociacion);
                        return View(await filtroAsociacion.ToListAsync());
                    }
            }
             return _context.TbUsuarios != null ? 
                        View(await _context.TbUsuarios.ToListAsync()) :
                        Problem("Entity set 'UcgdbContext.TbUsuarios'  is null.");
        }catch(Exception ex){
            TempData["ErrorMessage"] = "Ocurrió un error al cargar los usuarios. Error:" + ex;
            return RedirectToAction("Error");  
        }
       }

       // GET: TbUsuarios/Details/5
       public async Task<IActionResult> Details(int? id)
       {
           if (id == null || _context.TbUsuarios == null)
           {
               return NotFound();
           }

           try{

           var tbUsuario = await _context.TbUsuarios
               .FirstOrDefaultAsync(m => m.IdUsuario == id);
           if (tbUsuario == null)
           {
               return NotFound();
           }

           return View(tbUsuario);
           }catch(Exception ex){
            TempData["ErrorMessage"] = "Ocurrió un error al obtener los detalles del usuario. Error=" + ex;
            return RedirectToAction("Error");
           }
       }

       // GET: TbUsuarios/Create
       public IActionResult Create()
       {
        var model = new UsuarioViewModel();

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
       }

       // POST: TbUsuarios/Create
       // To protect from overposting attacks, enable the specific properties you want to bind to.
       // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Create( UsuarioViewModel model)
       {

            if (model.Contraseña != model.ConfirmarContraseña)
           {
               ModelState.AddModelError("Contraseña", "Las contraseñas no coinciden");
               return View(model);
           }
           
            if (!ModelState.IsValid)
           {
            return View(model);
           }

            
                var usuario = new TbUsuario
               {
                   NombreUsuario = model.NombreUsuario,
                   Contraseña = _hashingService.GenerateHash(model.Contraseña),
                   Rol = model.Rol,
                   Correo = model.Correo,
                   Estado = model.Estado
                   

               };
            try
            {
                _context.Add(usuario);
               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
            }catch(Exception ex){
                ModelState.AddModelError("", "Ocurrió un error al registrar el usuario.");
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return View(model);
            }
       }

       // GET: TbUsuarios/Edit/5
       public async Task<IActionResult> Edit(int? id)
       {
           if (id == null || _context.TbUsuarios == null)
           {
               return NotFound();
           }

           try{

           var tbUsuario = await _context.TbUsuarios.FindAsync(id);
           if (tbUsuario == null)
           {
               return NotFound();
           }
           return View(tbUsuario);
           }catch(Exception ex){
            TempData["ErrorMessage"] = "Ocurrió un error al cargar la edición del usuario. Error="+ ex;
            return RedirectToAction("Error");
           }
       }

       // POST: TbUsuarios/Edit/5
       // To protect from overposting attacks, enable the specific properties you want to bind to.
       // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,NombreUsuario,Contraseña,Rol,Correo,Estado")] TbUsuario tbUsuario)
       {
           if (id != tbUsuario.IdUsuario)
           {
               return NotFound();
           }

           if (!ModelState.IsValid){
            return View(tbUsuario);
           }
           
               try
               {
                   _context.Update(tbUsuario);
                   await _context.SaveChangesAsync();
                   return RedirectToAction(nameof(Index));
               }
               catch (DbUpdateConcurrencyException)
               {
                   if (!TbUsuarioExists(tbUsuario.IdUsuario))
                   {
                       return NotFound();
                   }
                   else
                   {
                       throw;
                   }
               }
               catch(Exception ex){
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el usuario. Error="+ex;
                return RedirectToAction("Error");
               }
               
           
           
       }

       // GET: TbUsuarios/Delete/5
       public async Task<IActionResult> Delete(int? id)
       {
           if (id == null || _context.TbUsuarios == null)
           {
               return NotFound();
           }

           try{

           var tbUsuario = await _context.TbUsuarios
               .FirstOrDefaultAsync(m => m.IdUsuario == id);
           if (tbUsuario == null)
           {
               return NotFound();
           }

           return View(tbUsuario);
           }catch(Exception ex){
            TempData["ErrorMessage"] = "Ocurrió un error al cargar la eliminación del usuario. Error="+ex;
                return RedirectToAction("Error");
           }
       }

       // POST: TbUsuarios/Delete/5
       [HttpPost, ActionName("Delete")]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> DeleteConfirmed(int id)
       {
           if (_context.TbUsuarios == null)
           {
               return Problem("Entity set 'UcgdbContext.TbUsuarios'  is null.");
           }

           try {

           var tbUsuario = await _context.TbUsuarios.FindAsync(id);
           if (tbUsuario != null)
           {
               _context.TbUsuarios.Remove(tbUsuario);
           }
            
           await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
           }catch(Exception ex){
            TempData["ErrorMessage"] = "Ocurrió un error al eliminar el usuario. Error="+ex;
                return RedirectToAction("Error");
           }
       }

       private bool TbUsuarioExists(int id)
       {
         return (_context.TbUsuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
       }

       public IActionResult Error()
        {
            return View("Error");
        }
   }
}
