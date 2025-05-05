using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
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
            string rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            var model = new ActaViewModel();

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                bool tieneAsociacion = int.TryParse(idAsociacionClaim, out int idAsociacion);

                var nombre = _context.TbAsociacions
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => a.Nombre)
                    .FirstOrDefault();

                ViewBag.IdAsociacion = idAsociacion;
                ViewBag.NombreAsociacion = nombre;
                ViewBag.EsAdmin = true;
                model.IdAsociacion = idAsociacion;

                ViewData["IdAsociado"] = new SelectList(
                    _context.TbAsociados
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .Select(a => new { a.IdAsociado, NombreCompleto = a.Nombre + " " + a.Apellido1 }),
                    "IdAsociado",
                    "NombreCompleto");
            }
            else
            {
                ViewBag.EsAdmin = false;
                ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
                //ViewData["IdAsociado"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(model);
            // var model = new ActaViewModel();
            // ViewData["IdAsociacion"] = new SelectList(_context.TbAsociacions, "IdAsociacion", "Nombre");
            // return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActaViewModel model)
        {
            if (!await ParseFechaSesionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de asistencias o acuerdos.";
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
                return RedirectToAction(nameof(Create));

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el acta: " + ex.Message;
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var acta = await _context.TbActa
                .Include(a => a.TbActaAsistencias)
                .Include(a => a.TbAcuerdos)
                .FirstOrDefaultAsync(a => a.IdActa == id);

            if (acta is null)
                return NotFound();

            var model = MapearActaViewModel(acta);

            model.ActaAsistenciaJason = JsonConvert.SerializeObject(model.ActaAsistencia);
            model.ActaAcuerdoJason = JsonConvert.SerializeObject(model.ActaAcuerdo);

            await PrepararViewDataAsync(model);
            return View(model);
        }

        private ActaViewModel MapearActaViewModel(TbActum acta)
        {
            return new ActaViewModel
            {
                IdActa = acta.IdActa,
                IdAsociacion = acta.IdAsociacion,
                IdAsociado = acta.IdAsociado,
                FechaSesion = acta.FechaSesion,
                FechaSesionTexto = acta.FechaSesion.ToString("yyyy-MM-dd"),
                NumeroActa = acta.NumeroActa,
                Descripcion = acta.Descripcion,
                Estado = acta.Estado,
                MontoTotalAcordado = acta.MontoTotalAcordado,
                ActaAsistencia = acta.TbActaAsistencias.Select(a => new ActaAsistenciaViewModel
                {
                    IdAsociado = a.IdAsociado,
                    Fecha = a.Fecha
                }).ToList(),
                ActaAcuerdo = acta.TbAcuerdos.Select(a => new AcuerdoViewModel
                {
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion,
                    MontoAcuerdo = a.MontoAcuerdo
                }).ToList()
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idActa, ActaViewModel model)
        {
            if (idActa != model.IdActa)
            {
                return NotFound();
            }

            if (!await ParseFechaSesionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
                await PrepararViewDataAsync(model);
                return View(model);
            }

            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de asistencias o acuerdos.";
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
                await PrepararViewDataAsync(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var actaExistente = await _context.TbActa
                    .Include(a => a.TbActaAsistencias)
                    .Include(a => a.TbAcuerdos)
                    .FirstOrDefaultAsync(a => a.IdActa == idActa);

                if (actaExistente == null)
                {
                    return NotFound();
                }

                // Actualizar campos principales del acta
                actaExistente.IdAsociacion = model.IdAsociacion;
                actaExistente.IdAsociado = model.IdAsociado;
                actaExistente.FechaSesion = model.FechaSesion;
                actaExistente.NumeroActa = model.NumeroActa;
                actaExistente.Descripcion = model.Descripcion;
                actaExistente.Estado = model.Estado;
                actaExistente.MontoTotalAcordado = model.MontoTotalAcordado;

                // Reemplazar asistencias
                _context.TbActaAsistencia.RemoveRange(actaExistente.TbActaAsistencias);
                await GuardarAsistenciasAsync(model.ActaAsistencia, idActa);

                // Reemplazar acuerdos
                _context.TbAcuerdos.RemoveRange(actaExistente.TbAcuerdos);
                await GuardarAcuerdosAsync(model.ActaAcuerdo, idActa);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El acta fue actualizada exitosamente.";
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el acta: " + ex.Message;
                await PrepararViewDataAsync(model);
                return View(model);
            }
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
