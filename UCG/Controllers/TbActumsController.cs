using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text.RegularExpressions;
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

        public async Task<IActionResult> Create()
        {
            var model = new ActaViewModel();
            await ConfigurarAsociacionActaAsync(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActaViewModel model)
        {
            // 1. Parsear fecha
            if (!await ParseFechaSesionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
                await ConfigurarAsociacionActaAsync(model);
                return View(model);
            }

            // 2. Deserializar los asistentes y acuerdos
            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de asistencias o acuerdos.";
                await ConfigurarAsociacionActaAsync(model);
                return View(model);
            }

            // 3. Validar el modelo
            var validator = new ActaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionActaAsync(model);
                return View(model);
            }

            // 4. Crear la transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 5. Mapear el modelo a la entidad
                var acta = MapearActa(model);

                
                // 5.1 Generar número automáticamente
                acta.NumeroActa = await GenerarNumeroActaPorFolioAsync(acta.IdFolio, acta.FechaSesion);

                // 6. Insertar acta
                _context.TbActa.Add(acta);
                await _context.SaveChangesAsync();

                // 7. Insertar asistentes
                await GuardarAsistenciasAsync(model.ActaAsistencia, acta.IdActa);

                // 8. Insertar acuerdos
                await GuardarAcuerdosAsync(model.ActaAcuerdo, acta.IdActa);

                // 9. Guardar todo y finalizar
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El acta fue creada exitosamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al guardar el acta: " + ex.Message;
                await ConfigurarAsociacionActaAsync(model);
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

        private async Task ConfigurarAsociacionActaAsync(ActaViewModel model)
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            if (rol == "Admin")
            {
                var idAsociacionClaim = User.FindFirst("IdAsociacion")?.Value;
                if (int.TryParse(idAsociacionClaim, out int idAsociacion))
                {
                    var nombre = await _context.TbAsociacions
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .Select(a => a.Nombre)
                        .FirstOrDefaultAsync();

                    model.IdAsociacion = idAsociacion;
                    ViewBag.IdAsociacion = idAsociacion;
                    ViewBag.Nombre = nombre;
                    ViewBag.EsAdmin = true;

                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    var folios = await _context.TbFolios
                        .Where(f => f.IdAsociacion == idAsociacion)
                        .ToListAsync();

                    ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
                    ViewData["IdFolio"] = new SelectList(folios, "IdFolio", "NumeroFolio", model.IdFolio);
                }
            }
            else
            {
                ViewBag.EsAdmin = false;

                ViewData["IdAsociacion"] = new SelectList(
                    await _context.TbAsociacions.ToListAsync(),
                    "IdAsociacion", "Nombre", model.IdAsociacion);

                if (model.IdAsociacion > 0)
                {
                    var asociados = await _context.TbAsociados
                        .Where(a => a.IdAsociacion == model.IdAsociacion)
                        .ToListAsync();

                    var folios = await _context.TbFolios
                        .Where(f => f.IdAsociacion == model.IdAsociacion)
                        .ToListAsync();

                    ViewData["IdAsociado"] = new SelectList(asociados, "IdAsociado", "Nombre", model.IdAsociado);
                    ViewData["IdFolio"] = new SelectList(folios, "IdFolio", "NumeroFolio", model.IdFolio);
                }
              
            }
        }


        private TbActum MapearActa(ActaViewModel model)
        {
            return new TbActum
            {
                IdAsociacion = model.IdAsociacion,
                IdAsociado = model.IdAsociado,
                IdFolio = model.IdFolio,
                Tipo = model.Tipo,
                FechaSesion = model.FechaSesion,
                NumeroActa = model.NumeroActa,
                Descripcion = model.Descripcion,
                Estado = model.Estado,
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
                    Tipo = acuerdo.Tipo
                });
            }
        }

        [HttpGet]
        public JsonResult ObtenerFoliosPorAsociacion(int idAsociacion)
        {
            try
            {
                var folios = _context.TbFolios
                    .Where(f => f.IdAsociacion == idAsociacion)
                    .Select(f => new
                    {
                        f.IdFolio,
                        f.NumeroFolio
                    })
                    .ToList();

                if (!folios.Any())
                {
                    return Json(new { success = false, message = "No hay folios disponibles." });
                }

                return Json(new { success = true, data = folios });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los folios: " + ex.Message });
            }
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var acta = await _context.TbActa
                 .Include(a => a.TbActaAsistencias)
                    .ThenInclude(a => a.IdAsociadoNavigation) 
                    .Include(a => a.TbAcuerdos)
                    .FirstOrDefaultAsync(a => a.IdActa == id);

            if (acta is null)
                return NotFound();

            var model = MapearActaViewModel(acta);

            model.ActaAsistenciaJason = JsonConvert.SerializeObject(model.ActaAsistencia);
            model.ActaAcuerdoJason = JsonConvert.SerializeObject(model.ActaAcuerdo);

            await ConfigurarAsociacionActaAsync(model);
            return View(model);
        }

        private ActaViewModel MapearActaViewModel(TbActum acta)
        {
            return new ActaViewModel
            {
                IdActa = acta.IdActa,
                IdAsociacion = acta.IdAsociacion,
                IdAsociado = acta.IdAsociado,
                IdFolio = acta.IdFolio,
                Tipo = acta.Tipo,
                FechaSesion = acta.FechaSesion,
                FechaSesionTexto = acta.FechaSesion.ToString("yyyy-MM-dd"),
                NumeroActa = acta.NumeroActa,
                Descripcion = acta.Descripcion,
                Estado = acta.Estado,
                ActaAsistencia = acta.TbActaAsistencias.Select(a => new ActaAsistenciaViewModel
                {
                    IdAsociado = a.IdAsociado,
                    Fecha = a.Fecha,
                    Nombre = a.IdAsociadoNavigation?.Nombre ?? "N/D",
                    Apellido1 = a.IdAsociadoNavigation?.Apellido1 ?? ""

                }).ToList(),
                ActaAcuerdo = acta.TbAcuerdos.Select(a => new AcuerdoViewModel
                {
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion,
                    MontoAcuerdo = a.MontoAcuerdo
                }).ToList()

            };
        }

        // Método para generar el número de acta basado en el folio y la fecha de sesión
        private async Task<string> GenerarNumeroActaPorFolioAsync(int? idFolio, DateOnly fechaSesion)
        {
            if (idFolio == null)
                return "Folio-N/D No. 001-" + fechaSesion.Year;

            // Buscar la última acta registrada con ese folio
            var ultimaActa = await _context.TbActa
                .Where(a => a.IdFolio == idFolio)
                .OrderByDescending(a => a.IdActa) // Puede cambiarse por FechaSesion si es mejor
                .FirstOrDefaultAsync();

            // Inicializar número en 1
            int siguienteNumero = 1;

            if (ultimaActa != null)
            {
                // Extraer número con regex si el formato anterior era "Folio-xxx No. 001-2025"
                var match = Regex.Match(ultimaActa.NumeroActa ?? "", @"No\.\s*(\d{3})");

                if (match.Success && int.TryParse(match.Groups[1].Value, out int ultimoNumero))
                {
                    siguienteNumero = ultimoNumero + 1;
                }
            }

            // Obtener el numeroFolio real
            var folio = await _context.TbFolios.FirstOrDefaultAsync(f => f.IdFolio == idFolio);
            var numeroFolio = folio?.NumeroFolio ?? "N/D";
            var year = fechaSesion.Year;

            return $"Folio-{numeroFolio} No. {siguienteNumero.ToString("D3")}-{year}";
        }

        // Método para obtener el número de acta basado en el folio y la fecha de sesión
        [HttpGet]
        public async Task<JsonResult> ObtenerNumeroActa(int idFolio, string fecha)
        {
            if (!DateOnly.TryParse(fecha, out var fechaSesion))
            {
                return Json(new { error = "Fecha inválida" });
            }

            var numero = await GenerarNumeroActaPorFolioAsync(idFolio, fechaSesion);
            return Json(new { numeroActa = numero });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idActa, ActaViewModel model)
        {
            if (idActa != model.IdActa)
                return NotFound();

            // Parsear la fecha desde el texto
            if (!await ParseFechaSesionAsync(model))
            {
                TempData["ErrorMessage"] = "Debe ingresar una fecha de sesión válida.";
                await ConfigurarAsociacionActaAsync(model);
                return View(model);
            }

            // Deserializar listas de asistencia y acuerdos
            if (!await DeserializarJsonAsync(model))
            {
                TempData["ErrorMessage"] = "Error en los datos de asistencias o acuerdos.";
                await ConfigurarAsociacionActaAsync(model);
                return View(model);
            }

            // Validar el modelo con FluentValidation
            var validator = new ActaViewModelValidator(_context);
            var validationResult = await validator.ValidateAsync(model);

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores de validación en el formulario.";
                await ConfigurarAsociacionActaAsync(model);
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
                    return NotFound();

                // Actualizar campos principales del acta
                actaExistente.IdAsociacion = model.IdAsociacion;
                actaExistente.IdAsociado = model.IdAsociado;
                actaExistente.IdFolio = model.IdFolio;
                actaExistente.FechaSesion = model.FechaSesion;
                actaExistente.NumeroActa = model.NumeroActa;
                actaExistente.Descripcion = model.Descripcion;
                actaExistente.Estado = model.Estado;

                // Reemplazar asistencias
                _context.TbActaAsistencia.RemoveRange(actaExistente.TbActaAsistencias);
                await GuardarAsistenciasAsync(model.ActaAsistencia, idActa);

                // Reemplazar acuerdos
                _context.TbAcuerdos.RemoveRange(actaExistente.TbAcuerdos);
                await GuardarAcuerdosAsync(model.ActaAcuerdo, idActa);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "El acta fue actualizada exitosamente.";
                return RedirectToAction(nameof(Edit), new { idActa });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el acta: " + ex.Message;
                await ConfigurarAsociacionActaAsync(model);
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
