using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Models.ValidationModels;
using UCG.Models.ViewModels;

namespace UCG.Controllers
{
    public class TbActaAsistenciumsController : Controller
    {
        private readonly UcgdbContext _context;

        public TbActaAsistenciumsController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbActaAsistenciums
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbActaAsistencia.Include(t => t.IdAsociadoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbActaAsistenciums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbActaAsistencia == null)
            {
                return NotFound();
            }

            var tbActaAsistencium = await _context.TbActaAsistencia
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActaAsistencia == id);
            if (tbActaAsistencium == null)
            {
                return NotFound();
            }

            return View(tbActaAsistencium);
        }

        [HttpGet]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                ViewData["IdActa"] = new SelectList(_context.TbActa.ToList(), "IdActa", "NumeroActa");
                return View(new ActaAsistenciaViewModel());
            }

            var acta = _context.TbActa.FirstOrDefault(a => a.IdActa == id);
            var model = new ActaAsistenciaViewModel { IdActa = id.Value };
            ViewData["IdActa"] = id;
            ViewData["IdAsociacion"] = acta?.IdAsociacion;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActaAsistenciaViewModel model)
        {
            var validator = new ActaAsistenciaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

            model.Fecha = DateOnly.ParseExact(model.FechaTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var asistencia = MapearActa(model);


                _context.TbActaAsistencia.Add(asistencia);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var idAsociacion = await _context.TbActa
                    .Where(a => a.IdActa == model.IdActa)
                    .Select(a => a.IdAsociacion)
                    .FirstOrDefaultAsync();

                var asociadosAsociacion = await _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => a.IdAsociado)
                    .ToListAsync();

                var asociadosRegistrados = await _context.TbActaAsistencia
                    .Where(a => a.IdActa == model.IdActa)
                    .Select(a => a.IdAsociado)
                    .ToListAsync();

                var asociadosRestantes = asociadosAsociacion.Except(asociadosRegistrados).ToList();

                TempData["SuccessMessage"] = asociadosRestantes.Any()
                    ? "La asistencia fue creada exitosamente."
                    : "Todos los asociados ya han sido registrados.";

                TempData["IdActa"] = model.IdActa;

                return asociadosRestantes.Any()
                    ? RedirectToAction(nameof(Create), new { id = model.IdActa })
                    : RedirectToAction("Details", "TbActums", new { id = model.IdActa });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar la asistencia: " + ex.Message;
                await PrepararViewDataAsync(model);
                return View(model);
            }
        }

        private TbActaAsistencium MapearActa(ActaAsistenciaViewModel model)
        {
            return new TbActaAsistencium
            {
                IdActa = model.IdActa,
                IdAsociado = model.IdAsociado,
                Fecha = model.Fecha
            };
        }

        private async Task PrepararViewDataAsync(ActaAsistenciaViewModel model)
        {
            ViewData["IdActa"] = new SelectList(
                await _context.TbActa.ToListAsync(),
                "IdActa", "NumeroActa", model.IdActa);

            List<SelectListItem> asociadosDisponibles;

            if (model.IdActa > 0)
            {
                // Obtener la asociación del acta
                var idAsociacion = await _context.TbActa
                    .Where(a => a.IdActa == model.IdActa)
                    .Select(a => a.IdAsociacion)
                    .FirstOrDefaultAsync();

                // Asociados que ya tienen asistencia en ese acta
                var idsYaRegistrados = await _context.TbActaAsistencia
                    .Where(a => a.IdActa == model.IdActa)
                    .Select(a => a.IdAsociado)
                    .ToListAsync();

                // Asociados disponibles = no registrados aún
                asociadosDisponibles = await _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion && !idsYaRegistrados.Contains(a.IdAsociado))
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdAsociado.ToString(),
                        Text = $"{a.Nombre} {a.Apellido1}"
                    })
                    .ToListAsync();
            }
            else
            {
                // Carga general (caso raro pero por si acaso)
                asociadosDisponibles = await _context.TbAsociados
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdAsociado.ToString(),
                        Text = $"{a.Nombre} {a.Apellido1}"
                    })
                    .ToListAsync();
            }

            ViewData["IdAsociado"] = asociadosDisponibles;
        }


        [HttpGet]
        public JsonResult ObtenerAsociacionPorActa(int idActa)
        {
            if (idActa <= 0) return Json(new { success = false, message = "ID de acta no válido." });

            var acta = _context.TbActa.FirstOrDefault(a => a.IdActa == idActa);
            return acta == null
                ? Json(new { success = false, message = "Acta no encontrada." })
                : Json(new { success = true, idAsociacion = acta.IdAsociacion });
        }

        [HttpGet]
        public JsonResult ObtenerAsociadosPorAsociacion(int idAsociacion, int? idActa)
        {
            try
            {
                var asociadosConAsistencia = idActa.HasValue
                    ? _context.TbActaAsistencia
                        .Where(a => a.IdActa == idActa.Value)
                        .Select(a => a.IdAsociado)
                        .ToHashSet()
                    : new HashSet<int>();

                var asociadosDisponibles = _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion && !asociadosConAsistencia.Contains(a.IdAsociado))
                    .Select(a => new { a.IdAsociado, a.Nombre, a.Apellido1 })
                    .ToList();

                return !asociadosDisponibles.Any()
                    ? Json(new { success = false, message = "Todos los asociados ya fueron registrados en esta acta." })
                    : Json(new { success = true, data = asociadosDisponibles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los asociados: " + ex.Message });
            }
        }


        // GET: TbActaAsistenciums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbActaAsistencia == null)
            {
                return NotFound();
            }

            var tbActaAsistencium = await _context.TbActaAsistencia.FindAsync(id);
            if (tbActaAsistencium == null)
            {
                return NotFound();
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActaAsistencium.IdAsociado);
            return View(tbActaAsistencium);
        }

        // POST: TbActaAsistenciums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActaAsistencia,IdAsociado")] TbActaAsistencium tbActaAsistencium)
        {
            if (id != tbActaAsistencium.IdActaAsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbActaAsistencium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbActaAsistenciumExists(tbActaAsistencium.IdActaAsistencia))
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
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbActaAsistencium.IdAsociado);
            return View(tbActaAsistencium);
        }

        // GET: TbActaAsistenciums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbActaAsistencia == null)
            {
                return NotFound();
            }

            var tbActaAsistencium = await _context.TbActaAsistencia
                .Include(t => t.IdAsociadoNavigation)
                .FirstOrDefaultAsync(m => m.IdActaAsistencia == id);
            if (tbActaAsistencium == null)
            {
                return NotFound();
            }

            return View(tbActaAsistencium);
        }

        // POST: TbActaAsistenciums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbActaAsistencia == null)
            {
                return Problem("Entity set 'UcgdbContext.TbActaAsistencia'  is null.");
            }
            var tbActaAsistencium = await _context.TbActaAsistencia.FindAsync(id);
            if (tbActaAsistencium != null)
            {
                _context.TbActaAsistencia.Remove(tbActaAsistencium);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbActaAsistenciumExists(int id)
        {
          return (_context.TbActaAsistencia?.Any(e => e.IdActaAsistencia == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
