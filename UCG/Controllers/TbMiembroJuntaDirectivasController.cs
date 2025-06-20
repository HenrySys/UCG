﻿using System;
using System.Collections.Generic;
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
    public class TbMiembroJuntaDirectivasController : Controller
    {
        private readonly UcgdbContext _context;

        public TbMiembroJuntaDirectivasController(UcgdbContext context)
        {
            _context = context;
        }

        // GET: TbMiembroJuntaDirectivas
        public async Task<IActionResult> Index()
        {
            var ucgdbContext = _context.TbMiembroJuntaDirectivas.Include(t => t.IdAsociadoNavigation).Include(t => t.IdJuntaDirectivaNavigation).Include(t => t.IdPuestoNavigation);
            return View(await ucgdbContext.ToListAsync());
        }

        // GET: TbMiembroJuntaDirectivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbMiembroJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdJuntaDirectivaNavigation)
                .Include(t => t.IdPuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdMiembrosJuntaDirectiva == id);
            if (tbMiembroJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbMiembroJuntaDirectiva);
        }



        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var model = new MiembroJuntaDirectivaViewModel();

            if (!id.HasValue) return RedirectToAction("Index");

            var junta = await _context.TbJuntaDirectivas.FindAsync(id.Value);
            if (junta == null) return NotFound();

            model.IdJuntaDirectiva = id.Value;

            var idAsociacion = junta.IdAsociacion;
            ViewData["IdJuntaDirectiva"] = id.Value;
            ViewData["IdAsociacion"] = idAsociacion;

            var asociadosExistentes = await _context.TbMiembroJuntaDirectivas
                .Where(m => m.IdJuntaDirectiva == id.Value)
                .Select(m => m.IdAsociado)
                .ToListAsync();

            var asociadosDisponibles = await _context.TbAsociados
                .Where(a => a.IdAsociacion == idAsociacion && !asociadosExistentes.Contains(a.IdAsociado))
                .Select(a => new SelectListItem
                {
                    Value = a.IdAsociado.ToString(),
                    Text = a.Nombre + " " + a.Apellido1
                }).ToListAsync();

            var puestosOcupados = await _context.TbMiembroJuntaDirectivas
                .Where(m => m.IdJuntaDirectiva == id.Value)
                .Select(m => m.IdPuesto)
                .ToListAsync();

            var puestosDisponibles = await _context.TbPuestos
                .Where(p => !puestosOcupados.Contains(p.IdPuesto))
                .Select(p => new SelectListItem
                {
                    Value = p.IdPuesto.ToString(),
                    Text = p.Nombre
                }).ToListAsync();

            ViewData["IdAsociado"] = asociadosDisponibles;
            ViewData["IdPuesto"] = puestosDisponibles;

            if (!asociadosDisponibles.Any())
                TempData["IdJuntaDirectiva"] = id.Value;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MiembroJuntaDirectivaViewModel model)
        {
            var validator = new MiembroJuntaDirectivaViewModelValidator(_context);
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
                var entidad = new TbMiembroJuntaDirectiva
                {
                    IdJuntaDirectiva = model.IdJuntaDirectiva,
                    IdAsociado = model.IdAsociado,
                    IdPuesto = model.IdPuesto
                };

                _context.TbMiembroJuntaDirectivas.Add(entidad);
                await _context.SaveChangesAsync();

                // Verificación de asociados restantes
                var idAsociacion = await _context.TbJuntaDirectivas
                    .Where(j => j.IdJuntaDirectiva == model.IdJuntaDirectiva)
                    .Select(j => j.IdAsociacion)
                    .FirstOrDefaultAsync();

                var todosAsociados = await _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion)
                    .Select(a => a.IdAsociado)
                    .ToListAsync();

                var yaRegistrados = await _context.TbMiembroJuntaDirectivas
                    .Where(m => m.IdJuntaDirectiva == model.IdJuntaDirectiva)
                    .Select(m => m.IdAsociado)
                    .ToListAsync();

                var restantes = todosAsociados.Except(yaRegistrados).ToList();

                TempData["SuccessMessage"] = restantes.Any()
                    ? "Miembro agregado correctamente."
                    : "Todos los asociados ya han sido registrados.";

                TempData["IdJuntaDirectiva"] = model.IdJuntaDirectiva;

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Create), new { id = model.IdJuntaDirectiva });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al guardar el miembro de junta: " + ex.Message;
                await PrepararViewDataAsync(model);
                return View(model);
            }
        }


        private async Task PrepararViewDataAsync(MiembroJuntaDirectivaViewModel model)
        {
            // Cargar todas las juntas disponibles para el combo, con selección automática si hay una ya definida
            ViewData["IdJuntaDirectiva"] = new SelectList(
                await _context.TbJuntaDirectivas.ToListAsync(), "IdJuntaDirectiva", "Nombre", model.IdJuntaDirectiva);

            List<SelectListItem> asociadosDisponibles;
            List<SelectListItem> puestosDisponibles;

            if (model.IdJuntaDirectiva > 0)
            {
                // Obtener la asociación vinculada a la junta
                var idAsociacion = await _context.TbJuntaDirectivas
                    .Where(j => j.IdJuntaDirectiva == model.IdJuntaDirectiva)
                    .Select(j => j.IdAsociacion)
                    .FirstOrDefaultAsync();

                // Filtrar asociados ya registrados
                var idsYaRegistrados = await _context.TbMiembroJuntaDirectivas
                    .Where(m => m.IdJuntaDirectiva == model.IdJuntaDirectiva)
                    .Select(m => m.IdAsociado)
                    .ToListAsync();

                // Obtener asociados disponibles
                asociadosDisponibles = await _context.TbAsociados
                    .Where(a => a.IdAsociacion == idAsociacion && !idsYaRegistrados.Contains(a.IdAsociado))
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdAsociado.ToString(),
                        Text = $"{a.Nombre} {a.Apellido1}"
                    })
                    .ToListAsync();

                // Filtrar puestos ya registrados
                var puestosOcupados = await _context.TbMiembroJuntaDirectivas
                    .Where(m => m.IdJuntaDirectiva == model.IdJuntaDirectiva)
                    .Select(m => m.IdPuesto)
                    .ToListAsync();

                puestosDisponibles = await _context.TbPuestos
                    .Where(p => !puestosOcupados.Contains(p.IdPuesto))
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdPuesto.ToString(),
                        Text = p.Nombre
                    })
                    .ToListAsync();
            }
            else
            {
                // Si no se ha elegido Junta, cargar todos los asociados y puestos
                asociadosDisponibles = await _context.TbAsociados
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdAsociado.ToString(),
                        Text = $"{a.Nombre} {a.Apellido1}"
                    })
                    .ToListAsync();

                puestosDisponibles = await _context.TbPuestos
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdPuesto.ToString(),
                        Text = p.Nombre
                    })
                    .ToListAsync();
            }

            ViewData["IdAsociado"] = asociadosDisponibles;
            ViewData["IdPuesto"] = puestosDisponibles;
        }


        [HttpGet]
        public JsonResult ObtenerAsociadosDisponiblesPorJunta(int idJunta)
        {
            try
            {
                // Obtener la junta
                var junta = _context.TbJuntaDirectivas.FirstOrDefault(j => j.IdJuntaDirectiva == idJunta);
                if (junta == null)
                {
                    return Json(new { success = false, message = "Junta directiva no encontrada." });
                }

                // Obtener asociados ya registrados en la junta
                var asociadosEnJunta = _context.TbMiembroJuntaDirectivas
                    .Where(m => m.IdJuntaDirectiva == idJunta)
                    .Select(m => m.IdAsociado)
                    .ToHashSet();

                // Obtener asociados disponibles de la misma asociación de la junta
                var asociadosDisponibles = _context.TbAsociados
                    .Where(a => a.IdAsociacion == junta.IdAsociacion && !asociadosEnJunta.Contains(a.IdAsociado))
                    .Select(a => new { a.IdAsociado, a.Nombre, a.Apellido1 })
                    .ToList();

                return !asociadosDisponibles.Any()
                    ? Json(new { success = false, message = "Todos los asociados ya han sido registrados en esta junta directiva." })
                    : Json(new { success = true, data = asociadosDisponibles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los asociados: " + ex.Message });
            }
        }



        [HttpGet]
        public JsonResult ObtenerPuestosPorJunta(int idJunta)
        {
            try
            {
                // Obtener los ID de los puestos ya asignados en esta junta
                var puestosOcupados = _context.TbMiembroJuntaDirectivas
                    .Where(m => m.IdJuntaDirectiva == idJunta)
                    .Select(m => m.IdPuesto)
                    .ToHashSet();

                // Obtener los puestos disponibles (no asignados)
                var puestosDisponibles = _context.TbPuestos
                    .Where(p => !puestosOcupados.Contains(p.IdPuesto))
                    .Select(p => new { p.IdPuesto, p.Nombre })
                    .ToList();

                return !puestosDisponibles.Any()
                    ? Json(new { success = false, message = "Todos los puestos ya fueron asignados en esta junta." })
                    : Json(new { success = true, data = puestosDisponibles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los puestos: " + ex.Message });
            }
        }


        // GET: TbMiembroJuntaDirectivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TbMiembroJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas.FindAsync(id);
            if (tbMiembroJuntaDirectiva == null)
            {
                return NotFound();
            }
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMiembroJuntaDirectiva.IdAsociado);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.TbJuntaDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", tbMiembroJuntaDirectiva.IdJuntaDirectiva);
            ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "IdPuesto", tbMiembroJuntaDirectiva.IdPuesto);
            return View(tbMiembroJuntaDirectiva);
        }

        // POST: TbMiembroJuntaDirectivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMiembrosJuntaDirectiva,IdJuntaDirectiva,IdAsociado,IdPuesto,Estado")] TbMiembroJuntaDirectiva tbMiembroJuntaDirectiva)
        {
            if (id != tbMiembroJuntaDirectiva.IdMiembrosJuntaDirectiva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMiembroJuntaDirectiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMiembroJuntaDirectivaExists(tbMiembroJuntaDirectiva.IdMiembrosJuntaDirectiva))
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
            ViewData["IdAsociado"] = new SelectList(_context.TbAsociados, "IdAsociado", "IdAsociado", tbMiembroJuntaDirectiva.IdAsociado);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.TbJuntaDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", tbMiembroJuntaDirectiva.IdJuntaDirectiva);
            ViewData["IdPuesto"] = new SelectList(_context.TbPuestos, "IdPuesto", "IdPuesto", tbMiembroJuntaDirectiva.IdPuesto);
            return View(tbMiembroJuntaDirectiva);
        }

        // GET: TbMiembroJuntaDirectivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TbMiembroJuntaDirectivas == null)
            {
                return NotFound();
            }

            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas
                .Include(t => t.IdAsociadoNavigation)
                .Include(t => t.IdJuntaDirectivaNavigation)
                .Include(t => t.IdPuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdMiembrosJuntaDirectiva == id);
            if (tbMiembroJuntaDirectiva == null)
            {
                return NotFound();
            }

            return View(tbMiembroJuntaDirectiva);
        }

        // POST: TbMiembroJuntaDirectivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TbMiembroJuntaDirectivas == null)
            {
                return Problem("Entity set 'UcgdbContext.TbMiembroJuntaDirectivas'  is null.");
            }
            var tbMiembroJuntaDirectiva = await _context.TbMiembroJuntaDirectivas.FindAsync(id);
            if (tbMiembroJuntaDirectiva != null)
            {
                _context.TbMiembroJuntaDirectivas.Remove(tbMiembroJuntaDirectiva);
                TempData["SuccessMessage"] = "El Miembro de la Junta Directiva fue eliminado correctamente.";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMiembroJuntaDirectivaExists(int id)
        {
          return (_context.TbMiembroJuntaDirectivas?.Any(e => e.IdMiembrosJuntaDirectiva == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
