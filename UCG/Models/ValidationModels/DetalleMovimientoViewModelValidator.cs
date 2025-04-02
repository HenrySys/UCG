using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class DetalleMovimientoViewModelValidator : AbstractValidator<DetalleMovimientoViewModel>
    {
        private readonly UcgdbContext _context;
        public DetalleMovimientoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdDetalleMovimiento)
                .NotNull().WithMessage("Debe seleccionar un ID de detalle demovimiento.")
                .GreaterThan(0).WithMessage("Debe seleccionar un ID válido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbDetalleMovimientos.AnyAsync(a => a.IdDetalleMovimiento == id);
                })
                .WithMessage("Ya existe un detalle de movimiento con ese ID.");

            RuleFor(x => x.IdMovimiento)
                .NotNull().WithMessage("Debe seleccionar un ID de movimiento.")
                .GreaterThan(0).WithMessage("Debe seleccionar un ID válido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbMovimientos.AnyAsync(a => a.IdMovimiento == id);
                })
                .WithMessage("No existe un movimiento con ese ID.");

            RuleFor(x => x.IdAcuerdo)
              .GreaterThan(0).WithMessage("Debe seleccionar un acuerdo valido.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAcuerdos.AnyAsync(a => a.IdAcuerdo == id);
               })
               .WithMessage("El acuerdo seleccionado no existe.");

            RuleFor(x => x.Decripcion)
              .NotEmpty().WithMessage("Debe ingresar una descripción.")
              .NotNull().WithMessage("Debe ingresar una descripción.")
              .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.Subtotal)
               .GreaterThanOrEqualTo(0).WithMessage("El subtotal debe ser un valor positivo.");

        }
    }
}
