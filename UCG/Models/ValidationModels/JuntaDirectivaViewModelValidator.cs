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
                .NotNull().WithMessage("Debe seleccionar una Asociaci�n.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociaci�n v�lida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociaci�n seleccionada no existe.");

            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("Debe ingresar un nombre.")
                .NotEmpty().WithMessage("Debe ingresar un nombre.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar un estado v�lido.");

            RuleFor(x => x.FechaPeriodoInicioTexto)
                .NotNull().WithMessage("Debe ingresar la fecha de inicio del per�odo.")
                .NotEmpty().WithMessage("Debe ingresar la fecha de inicio del per�odo.")
                .Must(ValidarFechaInicio).WithMessage("La fecha debe tener el formato yyyy-MM-dd y estar entre 2015 y hoy.");

            RuleFor(x => x.FechaPeriodoFinTexto)
                .NotNull().WithMessage("Debe ingresar la fecha de fin del per�odo.")
                .NotEmpty().WithMessage("Debe ingresar la fecha de fin del per�odo.")
                .Must(ValidarFechaFutura).WithMessage("La fecha debe tener el formato yyyy-MM-dd y ser al menos 2 a�os despu�s de hoy.");

            RuleFor(x => new { x.FechaPeriodoInicioTexto, x.FechaPeriodoFinTexto })
                .Must(x =>
                {
                    if (!DateOnly.TryParseExact(x.FechaPeriodoInicioTexto, "yyyy-MM-dd", out var inicio)) return false;
                    if (!DateOnly.TryParseExact(x.FechaPeriodoFinTexto, "yyyy-MM-dd", out var fin)) return false;
                    return fin > inicio;
                })
                .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        private bool ValidarFechaInicio(string? fechaTexto)
        {
            if (!DateOnly.TryParseExact(fechaTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return false;

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            return fecha >= new DateOnly(2015, 1, 1) && fecha <= hoy;
        }

        private bool ValidarFechaFutura(string? fechaTexto)
        {
            if (!DateOnly.TryParseExact(fechaTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return false;

            var minimo = DateOnly.FromDateTime(DateTime.Today.AddYears(2));
            return fecha >= minimo;
        }
    }
}
