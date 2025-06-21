using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class MovimientoIngresoViewModelValidator : AbstractValidator<MovimientoIngresoViewModel>
    {
        private readonly UcgdbContext _context;

        public MovimientoIngresoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una asociación.")
                .Must(id => id.HasValue && id.Value > 0)
                .WithMessage("Debe seleccionar una asociación válida.")
                .MustAsync(async (id, _) =>
                    id.HasValue && await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un asociado.")
                .Must(id => id.HasValue && id.Value > 0)
                .WithMessage("Debe seleccionar un asociado válido.")
                .MustAsync(async (model, id, _) =>
                    id.HasValue &&
                    await _context.TbAsociados.AnyAsync(a =>
                        a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El asociado no pertenece a la asociación seleccionada.");

            RuleFor(x => x.FechaTextoIngreso)
                .NotNull().WithMessage("Debe ingresar la fecha de ingreso.")
                .NotEmpty().WithMessage("Debe ingresar la fecha de ingreso.")
                .Must(EsFechaValida)
                .WithMessage("La fecha de ingreso debe tener el formato válido (dd/MM/yyyy).");

            RuleFor(x => x.MontoTotalIngresado)
                .NotNull().WithMessage("Debe ingresar el monto total.")
                .GreaterThan(0).WithMessage("El monto total debe ser mayor a ₡0.")
                .LessThanOrEqualTo(9999999999)
                .WithMessage("El monto total no puede superar ₡9,999,999,999.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("La descripción no puede ser nula.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            // Si querés que sea obligatorio al menos un documento de ingreso:
            // RuleFor(x => x.DocumentoIngreso)
            //     .NotEmpty().WithMessage("Debe agregar al menos un documento de ingreso.");
        }

        private bool EsFechaValida(string? fecha)
        {
            return !string.IsNullOrWhiteSpace(fecha) &&
                DateTime.TryParseExact(
                    fecha,
                    "yyyy-MM-aa",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out _);
        }
    }
}
