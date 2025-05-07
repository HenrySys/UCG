using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class JuntaDirectivaViewModelValidator : AbstractValidator<JuntaDirectivaViewModel>
    {
        private readonly UcgdbContext _context;

        public JuntaDirectivaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una Asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociación válida.")
                .MustAsync(async (id, cancellation) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociación seleccionada no existe.");

            


            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Debe ingresar un nombre.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar un estado válido.");



            RuleFor(x => x.FechaPeriodoInicioTexto)
             .NotEmpty().WithMessage("Debe ingresar una fecha de asistencia.")
             .Must(ValidarFechaTexto).WithMessage("La fecha debe tener el formato válido yyyy-MM-dd y estar entre el año 2000 y hoy.");

            RuleFor(x => x.FechaPeriodoFinTexto)
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
