using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbActumsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbActumsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbActums
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbActa.Include(t => t.IdAsociacionNavigation).Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbActums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbActa == null)
            {
                return NotFound();
            }

            var tbActum = await _context.TbActa
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActa == id);
            if (tbActum == null)
            {
                return NotFound();
            }

            return View(tbActum);
        }

        // GET: TbActums/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ActaViewModel
            {
                FechaSesion = DateOnly.FromDateTime(DateTime.Today),
                //ActaAsistencia = new List<ActaAsistenciaViewModel>()
            };

            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");

            return View(model);
        }

        // POST: TbActums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(ActaViewModel model)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(model.ActaAsistenciaJason))
        //        {
        //            model.ActaAsistencia = JsonConvert.DeserializeObject<List<ActaAsistenciaViewModel>>(model.ActaAsistenciaJason);
        //        }

        //        model.ActaAsistencia ??= new List<ActaAsistenciaViewModel>();

        //        var validator = new ActaViewModelValidator(_context);
        //        var validationResult = await validator.ValidateAsync(model);

        //        if (!validationResult.IsValid)
        //        {
        //            foreach (var error in validationResult.Errors)
        //            {
        //                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        //            }
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre", model.IdAsociacion);
        //            return View(model);
        //        }

        //        var acta = new TbActum
        //        {
        //            IdAsociacion = model.IdAsociacion,
        //            FechaSesion = model.FechaSesion,
        //            NumeroActa = model.NumeroActa,
        //            Descripcion = model.Descripcion,
        //            Estado = model.Estado,
        //            MontoTotalAcordado = model.MontoTotalAcordado,
        //        };

        //        _context.Add(acta);
        //        await _context.SaveChangesAsync();

        //        //Guardar asistencias si hay
        //        foreach (var asistencia in model.ActaAsistencia)
        //        {
        //            var nuevaAsistencia = new TbActaAsistencium
        //            {
        //                IdActa = acta.IdActa,
        //                IdAsociado = asistencia.IdAsociado,
        //                Fecha = asistencia.Fecha
        //            };

        //            _context.TbActaAsistencia.Add(nuevaAsistencia);
        //        }

        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el acta.");
        //        ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre", model.IdAsociacion);
        //        return View(model);
        //    }
        //}



        [HttpGet]
        public JsonResult ObtenerAsociadosPorAsociacion(int idAsociacion)
        {
            try
            {
                var asociados = _context.TbAsociados
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!asociados.Any())
                {
                    return Json(new { success = false, message = "No hay asociados disponibles." });
                }

                return Json(new { success = true, data = asociados });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los asociados: " + ex.Message });
            }
        }

        // GET: TbActums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbActa == null)
            {
                return NotFound();
            }

            var tbActum = await _context.TbActa.FindAsync(id);
            if (tbActum == null)
            {
                return NotFound();
            }
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActum.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActum.IdAsociado);
            return View(tbActum);
        }

        // POST: TbActums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActa,IdAsociacion,IdAsociado,FechaSesion,NumeroActa,Descripcion,Estado,MontoTotalAcordado")] TbActum tbActum)
        {
            if (id != tbActum.IdActa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbActum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbActumExists(tbActum.IdActa))
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
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbActum.IdAsociacion);
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActum.IdAsociado);
            return View(tbActum);
        }

        // GET: TbActums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbActa == null)
            {
                return NotFound();
            }

            var tbActum = await _context.TbActa
                .Include(t => t.IdAsociacionNavigation)
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActa == id);
            if (tbActum == null)
            {
                return NotFound();
            }

            return View(tbActum);
        }

        // POST: TbActums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbActa == null)
            {
                return Problem("Entity set 'UcgdbContext.TbActa'  is null.");
            }
            var tbActum = await _context.TbActa.FindAsync(id);
            if (tbActum != null)
            {
                _context.TbActa.Remove(tbActum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbActumExists(int id)
        {
          return (_context.TbActa?.Any(e => e.IdActa == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
