using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ActaAsistenciaViewModelValidator : AbstractValidator<ActaAsistenciaViewModel>
    {
        private readonly UcgdbContext _context;

        public ActaAsistenciaViewModelValidator (UcgdbContext context)
        {
            _context = context;


            RuleFor(x => x.IdAsociado)
              .NotNull().WithMessage("Debe seleccionar una Asociado.")
              .GreaterThan(0).WithMessage("Debe seleccionar un Asociado Valido .")
              .MustAsync(async (id, cancellation) =>
              {
                  return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
              })
              .WithMessage("El asociado seleccionado no existe.");

            RuleFor(x => x.FechaTexto)
           .NotEmpty().WithMessage("Debe ingresar una fecha de asistencia.")
           .Must(ValidarFechaTexto).WithMessage("La fecha debe tener el formato válido yyyy-MM-dd y estar entre el año 2000 y hoy.");

        }

        private bool ValidarFechaTexto(string? fechaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto))
                return false;

            if (!DateOnly.TryParseExact(fechaTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return false;

            if (fecha > DateOnly.FromDateTime(DateTime.Today)) return false;
            if (fecha < new DateOnly(2000, 1, 1)) return false;

            return true;
        }
    }
}
