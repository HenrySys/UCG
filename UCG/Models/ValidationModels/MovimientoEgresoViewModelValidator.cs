using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class MovimientoEgresoViewModelValidator : AbstractValidator<MovimientoEgresoViewModel>
    {
        private readonly UcgdbContext _context;

        public MovimientoEgresoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.FechaTextoEgreso)
                .NotEmpty().WithMessage("Debe ingresar la fecha de egreso.")
                .Must(EsFechaValida).WithMessage("La fecha de egreso no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una asociación válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un asociado válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbAsociados.AnyAsync(a =>
                        a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El asociado no pertenece a la asociación seleccionada.");

            RuleFor(x => x.IdActa)
                .NotNull().WithMessage("Debe seleccionar un acta.")
                .GreaterThan(0).WithMessage("Debe seleccionar un acta válida.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbActa.AnyAsync(a =>
                        a.IdActa == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El acta no pertenece a la asociación seleccionada.");

            RuleFor(x => x.Monto)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a ₡0.")
                .LessThanOrEqualTo(9999999999).WithMessage("El monto no puede superar ₡9,999,999,999.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("La descripción no puede ser nula.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            // Validación de detalle si fuera obligatorio:
            // RuleFor(x => x.DetalleChequeFacturaEgreso)
            //     .NotEmpty().WithMessage("Debe agregar al menos un detalle de cheque o factura.");
        }


        private bool EsFechaValida(string? FechaTexto)
        {
            return DateTime.TryParseExact(FechaTexto, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var fecha);
        }
    }
}
