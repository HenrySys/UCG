using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ChequeViewModelValidator : AbstractValidator<ChequeViewModel>
    {
        private readonly UcgdbContext _context;

        public ChequeViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .GreaterThan(0).WithMessage("Debe seleccionar una asociación válida.")
                .MustAsync(async (id, cancellation) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdCuenta)
                .GreaterThan(0).WithMessage("Debe seleccionar una cuenta válida.")
                .MustAsync(async (model, id, cancellation) =>
                    await _context.TbCuenta.AnyAsync(c => c.IdCuenta == id && c.IdAsociacion == model.IdAsociacion))
                .WithMessage("La cuenta no pertenece a la asociación seleccionada.");

            RuleFor(x => x.IdAsociadoAutoriza)
                .NotNull().WithMessage("Debe seleccionar un asociado que autoriza.")
                .MustAsync(async (model, id, cancellation) =>
                    await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El asociado que autoriza no pertenece a la asociación seleccionada.");

            RuleFor(x => x.NumeroCheque)
                .NotEmpty().WithMessage("Debe ingresar el número de cheque.")
                .MaximumLength(20).WithMessage("El número de cheque no puede superar los 20 caracteres.")
                .MustAsync(async (model, numero, cancellation) =>
                    !await _context.TbCheques.AnyAsync(c => c.NumeroCheque == numero && c.IdCheque != model.IdCheque))
                .WithMessage("Ya existe un cheque con ese número.");

            RuleFor(x => x.FechaTextoEmision)
                .NotEmpty().WithMessage("Debe ingresar la fecha de emisión.")
                .Must(SerFechaValida).WithMessage("La fecha de emisión no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.FechaTextoPago)
                .NotEmpty().WithMessage("Debe ingresar la fecha de pago.")
                .Must(SerFechaValida).WithMessage("La fecha de pago no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.FechaTextoCobro)
                .Must(SerFechaValida).When(x => !string.IsNullOrWhiteSpace(x.FechaTextoCobro))
                .WithMessage("La fecha de cobro no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.FechaTextoAnulacion)
                .Must(SerFechaValida).When(x => !string.IsNullOrWhiteSpace(x.FechaTextoAnulacion))
                .WithMessage("La fecha de anulación no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.Beneficiario)
                .NotEmpty().WithMessage("Debe ingresar el nombre del beneficiario.")
                .MaximumLength(100).WithMessage("El nombre del beneficiario no puede superar los 100 caracteres.");

            RuleFor(x => x.Monto)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a ₡0.")
                .LessThanOrEqualTo(9999999999).WithMessage("El monto no puede superar ₡9,999,999,999.");

            RuleFor(x => x.MontoRestante)
                .GreaterThanOrEqualTo(0).WithMessage("El monto restante no puede ser negativo.")
                .LessThanOrEqualTo(x => x.Monto).WithMessage("El monto restante no puede ser mayor al monto total.")
                .When(x => x.MontoRestante.HasValue);

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("Debe seleccionar un estado válido.")
                .IsInEnum().WithMessage("El estado seleccionado no es válido.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }

        private bool SerFechaValida(string? FechaTexto)
        {
            return DateTime.TryParseExact(FechaTexto, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var fecha);
        }
    }
}

