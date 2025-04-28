using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
             .Include(a => a.TbActaAsistencias)
                 .ThenInclude(a => a.IdAsociadoNavigation)
             .Include(a => a.TbAcuerdos)
             .Include(a => a.IdAsociacionNavigation)
             .Include(a => a.IdAsociadoNavigation)
             .FirstOrDefaultAsync(a => a.IdActa == id);
            if (tbActum == null)
            {
                return NotFound();
            }

            return View(tbActum);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ActaViewModel();
            ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActaViewModel model)
        {
            if (!await ParseFechaSesionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
                TempData.Keep("ErrorMessage");
                await PrepararViewDataAsync(model);
                return View(model);
            }

            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de asistencias o acuerdos.";
                TempData.Keep("ErrorMessage");
                await PrepararViewDataAsync(model);
                return View(model);
            }

            var validator = new ActaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                TempData.Keep("ErrorMessage");
                await PrepararViewDataAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var acta = MapearActa(model);

                _context.TbActa.Add(acta);
                await _context.SaveChangesAsync();

                await GuardarAsistenciasAsync(model.ActaAsistencia, acta.IdActa);
                await GuardarAcuerdosAsync(model.ActaAcuerdo, acta.IdActa);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El acta fue creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el acta: " + ex.Message;
                TempData.Keep("ErrorMessage");
                await PrepararViewDataAsync(model);
                return View(model);
            }
        }


        private async Task<bool> ParseFechaSesionAsync(ActaViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FechaSesionTexto))
            {
                if (DateOnly.TryParseExact(model.FechaSesionTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    model.FechaSesion = fecha;
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
                    TempData.Keep("ErrorMessage");

                    return false;
                }
            }
            TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
            TempData.Keep("ErrorMessage");

            return false;
        }


        private async Task<bool> DeserializarJsonAsync(ActaViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.ActaAsistenciaJason))
                    model.ActaAsistencia = JsonConvert.DeserializeObject<List<ActaAsistenciaViewModel>>(model.ActaAsistenciaJason);

                if (!string.IsNullOrWhiteSpace(model.ActaAcuerdoJason))
                    model.ActaAcuerdo = JsonConvert.DeserializeObject<List<AcuerdoViewModel>>(model.ActaAcuerdoJason);

                model.ActaAsistencia ??= new();
                model.ActaAcuerdo ??= new();
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al deserializar los datos: " + ex.Message;
                return false;
            }
        }


        private async Task PrepararViewDataAsync(ActaViewModel model)
        {
            ViewData["IdAsociacion"] = new SelectList(
                await _context.TbAsociacions.ToListAsync(),
                "IdAsociacion",
                "Nombre",
                model.IdAsociacion);

            ViewData["IdAsociado"] = new SelectList(
                await _context.TbAsociados
                    .Where(a => a.IdAsociacion == model.IdAsociacion)
                    .Select(a => new { a.IdAsociado, NombreCompleto = a.Nombre + " " + a.Apellido1 })
                    .ToListAsync(),
                "IdAsociado",
                "NombreCompleto",
                model.IdAsociado);

            model.ActaAsistenciaJason = JsonConvert.SerializeObject(model.ActaAsistencia ?? new());
            model.ActaAcuerdoJason = JsonConvert.SerializeObject(model.ActaAcuerdo ?? new());
        }

        private TbActum MapearActa(ActaViewModel model)
        {
            return new TbActum
            {
                IdAsociacion = model.IdAsociacion,
                IdAsociado = model.IdAsociado,
                FechaSesion = model.FechaSesion,
                NumeroActa = model.NumeroActa,
                Descripcion = model.Descripcion,
                Estado = model.Estado,
                MontoTotalAcordado = model.MontoTotalAcordado,
            };
        }

        private async Task GuardarAsistenciasAsync(List<ActaAsistenciaViewModel> asistencias, int idActa)
        {
            foreach (var asistencia in asistencias)
            {
                _context.TbActaAsistencia.Add(new TbActaAsistencium
                {
                    IdActa = idActa,
                    IdAsociado = asistencia.IdAsociado,
                    Fecha = asistencia.Fecha
                });
            }
        }

        private async Task GuardarAcuerdosAsync(List<AcuerdoViewModel> acuerdos, int idActa)
        {
            foreach (var acuerdo in acuerdos)
            {
                _context.TbAcuerdos.Add(new TbAcuerdo
                {
                    IdActa = idActa,
                    Nombre = acuerdo.Nombre,
                    Descripcion = acuerdo.Descripcion,
                    MontoAcuerdo = acuerdo.MontoAcuerdo
                });
            }
        }




        //[HttpGet]
        //public JsonResult ValidarAcuerdosPorAsociacion(string nombreAcuerdo, int idAsociacion)
        //{
        //    try
        //    {
        //        // Verifica si ya existe un acuerdo con ese nombre en alguna acta de esa asociación
        //        bool existe = _context.TbAcuerdos
        //            .Any(a => a.Nombre == nombreAcuerdo && a.IdActaNavigation.IdAsociacion == idAsociacion);

        //        if (existe)
        //        {
        //            return Json(new { success = false, message = "Ya existe un acuerdo con ese nombre en esta asociación." });
        //        }

        //        return Json(new { success = true, message = "Nombre de acuerdo disponible." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error al validar acuerdo: " + ex.Message });
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
