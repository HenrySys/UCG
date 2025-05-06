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
using UCG.Models.ValidationModels;
using Newtonsoft.Json;

namespace UCG.Controllers
{
    public class TbJuntaDirectivasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbJuntaDirectivasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbJuntaDirectivas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbJuntaDirectivas.Include(t => t.IdActaNavigation).Include(t => t.IdAsociacionNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbJuntaDirectivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbJuntaDirectiva = await _context.TbJuntaDirectivas
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdJuntaDirectiva == id);
            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbJuntaDirectiva);
        }

        // GET: TbJuntaDirectivas/Create
        public IActionResult Create()
        {

            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var model = new JuntaDirectivaViewModel();

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

                ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");

                return View(model);
            }
            else
            {
                ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa");
                ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
                ViewBag.EsAdmin = false;
                return View();
            }
    
        }

        // POST: TbJuntaDirectivas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JuntaDirectivaViewModel model)
        {
            var validator = new JuntaDirectivaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var nuevaJunta = MapearJuntaDirectiva(model);
               

                _context.TbJuntaDirectivas.Add(nuevaJunta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Junta Directiva creada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar la junta directiva.";
                await PrepararViewDataAsync(model);
                return View(model);
            }
        }
        private TbJuntaDirectiva MapearJuntaDirectiva(JuntaDirectivaViewModel model)
        {
            return new TbJuntaDirectiva
            {

                IdAsociacion = model.IdAsociacion,
                IdActa = model.IdActa,
                PeriodoInicio = model.PeriodoInicio,
                PeriodoFin = model.PeriodoFin,
                Nombre = model.Nombre,
                Estado = model.Estado
            };
        }

        private async Task<bool> DeserializarJsonAsync(JuntaDirectivaViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.MiembrosJuntaJson))
                    model.MiembroJunta = JsonConvert.DeserializeObject<List<ActaAsistenciaViewModel>>(model.MiembrosJuntaJson);

                    model.MiembroJunta ??= new();
              
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al deserializar los datos: " + ex.Message;
                return false;
            }
        }
       
        private async Task PrepararViewDataAsync(JuntaDirectivaViewModel model)
        {
            ViewData["IdActa"] = new SelectList(
                await _context.TbAsociacions.ToListAsync(),
                "IdActa",
                "NumeroActa",
                model.IdActa);

            ViewData["IdAsociacion"] = new SelectList(
               await _context.TbAsociacions.ToListAsync(),
               "IdAsociacion", "Nombre", model.IdAsociacion);
        }

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

        public JsonResult ObtenerActaPorAsociacion(int idAsociacion)
        {
            try
            {
                var actas = _context.TbActa
                    .Where(c => c.IdAsociacion == idAsociacion)
                    .ToList();

                if (!actas.Any())
                {
                    return Json(new { success = false, message = "No hay actas disponibles." });
                }

                return Json(new { success = true, data = actas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los actas: " + ex.Message });
            }
        }


        // GET: TbJuntaDirectivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbJuntaDirectiva = await _context.TbJuntaDirectivas.FindAsync(id);
            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbJuntaDirectiva.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbJuntaDirectiva.IdAsociacion);
            return View(tbJuntaDirectiva);
        }

        // POST: TbJuntaDirectivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdJuntaDirectiva,IdAsociacion,IdActa,PeriodoInicio,PeriodoFin,Nombre,Estado")] TbJuntaDirectiva tbJuntaDirectiva)
        {
            if (id != tbJuntaDirectiva.IdJuntaDirectiva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbJuntaDirectiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbJuntaDirectivaExists(tbJuntaDirectiva.IdJuntaDirectiva))
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
            ViewData["IdActa"] = new SelectList(_context.TbActa, "IdActa", "IdActa", tbJuntaDirectiva.IdActa);
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "IdAsociacion", tbJuntaDirectiva.IdAsociacion);
            return View(tbJuntaDirectiva);
        }

        // GET: TbJuntaDirectivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbJuntaDirectiva = await _context.TbJuntaDirectivas
                .Include(t => t.IdActaNavigation)
                .Include(t => t.IdAsociacionNavigation)
                .FirstOrDefaultAsync(m => m.IdJuntaDirectiva == id);
            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbJuntaDirectiva);
        }

        // POST: TbJuntaDirectivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbJuntaDirectivas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbJuntaDirectivas'  is null.");
            }
            var tbJuntaDirectiva = await _context.TbJuntaDirectivas.FindAsync(id);
            if (tbJuntaDirectiva != null)
            {
                _context.TbJuntaDirectivas.Remove(tbJuntaDirectiva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbJuntaDirectivaExists(int id)
        {
          return (_context.TbJuntaDirectivas?.Any(e => e.IdJuntaDirectiva == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
