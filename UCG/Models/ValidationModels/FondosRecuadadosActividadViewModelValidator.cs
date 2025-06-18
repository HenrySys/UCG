using FluentValidation;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class FondosRecaudadosActividadViewModelValidator : AbstractValidator<FondosRecaudadosActividadViewModel>
    {
        public FondosRecaudadosActividadViewModelValidator()
        {
            RuleFor(x => x.IdActividad)
                .GreaterThan(0).WithMessage("Debe seleccionar una actividad válida.");

            RuleFor(x => x.Detalle)
                .NotNull().WithMessage("Debe ingresar un detalle.")
                .NotEmpty().WithMessage("Debe ingresar un detalle.")
                .MaximumLength(250).WithMessage("El detalle no puede superar los 250 caracteres.");

            RuleFor(x => x.Monto)
                .NotNull().WithMessage("Debe ingresar un monto.")
                .GreaterThan(0).WithMessage("El monto debe ser mayor que cero.");


            RuleFor(x => x.FechaTextoRegistro)
               .NotEmpty().WithMessage("Debe ingresar la fecha de emisión.")
               .Must(SerFechaValida).WithMessage("La fecha de emisión no es válida (formato requerido: dd/MM/yyyy).");
        }

        private bool SerFechaValida(string? fecha)
        {
            return DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
