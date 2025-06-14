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
using System.Globalization;

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
                .Include(j => j.IdAsociacionNavigation) // Asociación
                .Include(j => j.TbMiembroJuntaDirectivas) // Miembros
                    .ThenInclude(m => m.IdAsociadoNavigation) // Asociación del miembro
                .Include(j => j.TbMiembroJuntaDirectivas)
                    .ThenInclude(m => m.IdPuestoNavigation) // Puesto del miembro
                .FirstOrDefaultAsync(j => j.IdJuntaDirectiva == id);

            if (tbJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbJuntaDirectiva);
        }


        [HttpGet]
        public IActionResult Create()
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var model = new JuntaDirectivaViewModel();

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                bool tieneAsociacion = int.TryParse(idAsociacionClaim, out int idAsociacion);

                var nombre = _context.TbAsociacions
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => a.Nombre)
                    .FirstOrDefault();

                // Asignación igual que en Acta
                ViewBag.IdAsociacion = idAsociacion;
                ViewBag.Nombre = nombre;
                ViewBag.EsAdmin = true;
                model.IdAsociacion = idAsociacion;


                // También podrías cargar los puestos si aplica
                ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "Nombre");
            }
            else
            {
                ViewBag.EsAdmin = false;
                ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
                // Puedes dejar vacío el SelectList de asociados hasta que se seleccione una asociación
                ViewData["IdAsociado"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "Nombre");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JuntaDirectivaViewModel model)
        {

            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de miembros.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

         
                if (DateOnly.TryParseExact(model.FechaPeriodoInicioTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var inicio))
                    model.PeriodoInicio = inicio;

                if (DateOnly.TryParseExact(model.FechaPeriodoFinTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fin))
                    model.PeriodoFin = fin;
 

        

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
                await _context.SaveChangesAsync(); // ← genera IdJuntaDirectiva

                await GuardarMiembrosAsync(model.MiembroJunta, nuevaJunta.IdJuntaDirectiva);
                await _context.SaveChangesAsync(); // ← guarda miembros

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Junta Directiva creada correctamente.";
                return RedirectToAction("Create");
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
                    model.MiembroJunta = JsonConvert.DeserializeObject<List<MiembroJuntaDirectivaViewModel>>(model.MiembrosJuntaJson);

                    model.MiembroJunta ??= new();
              
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al deserializar los datos: " + ex.Message;
                return false;
            }
        }

        private async Task GuardarMiembrosAsync(List<MiembroJuntaDirectivaViewModel> miembros, int idJuntaDirectiva)
        {
            foreach (var miembro in miembros)
            {
                _context.TbMiembroJuntaDirectivas.Add(new TbMiembroJuntaDirectiva
                {
                    IdJuntaDirectiva = idJuntaDirectiva,
                    IdAsociado = miembro.IdAsociado,
                    IdPuesto = miembro.IdPuesto,
                    Estado = TbMiembroJuntaDirectiva.EstadoDeMiembroJD.Activo
                });
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

            ViewData["IdPuesto"] = new SelectList(
              await _context.TbPuestos.ToListAsync(),
              "IdPuesto", "Nombre");

            model.MiembrosJuntaJson = JsonConvert.SerializeObject(model.MiembroJunta ?? new());
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


        private async Task<bool> ParseFechasPeriodoAsync(JuntaDirectivaViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaPeriodoInicioTexto))
            {
                if (!DateOnly.TryParseExact(model.FechaPeriodoInicioTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var inicio))
                    return false;

                model.PeriodoInicio = inicio;
            }

            if (!string.IsNullOrWhiteSpace(model.FechaPeriodoFinTexto))
            {
                if (!DateOnly.TryParseExact(model.FechaPeriodoFinTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fin))
                    return false;

                model.PeriodoFin = fin;
            }

            return true;
        }

        private JuntaDirectivaViewModel MapearJuntaDirectivaViewModel(TbJuntaDirectiva junta)
        {
            return new JuntaDirectivaViewModel
            {
                IdJuntaDirectiva = junta.IdJuntaDirectiva,
                IdAsociacion = junta.IdAsociacion,
                IdActa = junta.IdActa,
                Nombre = junta.Nombre,
                Estado = junta.Estado,
                PeriodoInicio = junta.PeriodoInicio ?? default,
                PeriodoFin = junta.PeriodoFin ?? default,
                FechaPeriodoInicioTexto = (junta.PeriodoInicio ?? default).ToString("yyyy-MM-dd"),
                FechaPeriodoFinTexto = (junta.PeriodoFin ?? default).ToString("yyyy-MM-dd"),
                MiembroJunta = junta.TbMiembroJuntaDirectivas.Select(m => new MiembroJuntaDirectivaViewModel
                {
                    IdAsociado = m.IdAsociado,
                    IdPuesto = m.IdPuesto,
                    Estado = m.Estado,
                    Nombre = m.IdAsociadoNavigation!.Nombre + " " + m.IdAsociadoNavigation.Apellido1,
                    Puesto = m.IdPuestoNavigation!.Nombre
                }).ToList()
            };
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var junta = await _context.TbJuntaDirectivas
                 .Include(j => j.TbMiembroJuntaDirectivas)
                     .ThenInclude(m => m.IdAsociadoNavigation)
                 .Include(j => j.TbMiembroJuntaDirectivas)
                     .ThenInclude(m => m.IdPuestoNavigation)
                 .FirstOrDefaultAsync(j => j.IdJuntaDirectiva == id);


            if (junta is null)
                return NotFound();

            var model = MapearJuntaDirectivaViewModel(junta);

            model.MiembrosJuntaJson = JsonConvert.SerializeObject(model.MiembroJunta ?? new());

            await PrepararViewDataAsync(model);
            return View(model);
        }




        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, JuntaDirectivaViewModel model)
		{
			if (id != model.IdJuntaDirectiva)
				return NotFound();

			if (!await DeserializarJsonAsync(model))
			{
				TempData["ErrorMessage"] = "Error en los datos de miembros.";
				await PrepararViewDataAsync(model);
				return View(model);
			}

            if (!await ParseFechasPeriodoAsync(model))
            {
                TempData["ErrorMessage"] = "Error en el formato de fechas. Debe ser yyyy-MM-dd.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

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
				var juntaExistente = await _context.TbJuntaDirectivas
					.Include(j => j.TbMiembroJuntaDirectivas)
					.FirstOrDefaultAsync(j => j.IdJuntaDirectiva == id);

				if (juntaExistente is null)
					return NotFound();

				// Actualiza los datos principales
				juntaExistente.IdAsociacion = model.IdAsociacion;
				juntaExistente.IdActa = model.IdActa;
				juntaExistente.Nombre = model.Nombre;
				juntaExistente.Estado = model.Estado;
				juntaExistente.PeriodoInicio = model.PeriodoInicio;
				juntaExistente.PeriodoFin = model.PeriodoFin;

				// Borra los miembros antiguos
				_context.TbMiembroJuntaDirectivas.RemoveRange(juntaExistente.TbMiembroJuntaDirectivas);
				await GuardarMiembrosAsync(model.MiembroJunta, juntaExistente.IdJuntaDirectiva);

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				TempData["SuccessMessage"] = "La junta directiva fue actualizada correctamente.";

                if (Request.Form["RedirigirMiembro"] == "true")
                {
                    return RedirectToAction("Create", "TbMiembroJuntaDirectivas", new { id = model.IdJuntaDirectiva });
                }

                return RedirectToAction(nameof(Edit), new { id });
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				TempData["ErrorMessage"] = "Error al actualizar la junta directiva: " + ex.Message;
				await PrepararViewDataAsync(model);
				return View(model);
			}
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
