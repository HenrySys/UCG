using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class DetalleFacturaViewModelValidator : AbstractValidator<DetalleFacturaViewModel>
    {
        private readonly UcgdbContext _context;

        public DetalleFacturaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            // Validación: factura existente
            RuleFor(x => x.IdFactura)
                .NotNull().WithMessage("Debe seleccionar una factura.")
                .MustAsync(async (id, _) =>
                {
                    return await _context.TbFacturas.AnyAsync(f => f.IdFactura == id);
                })
                .WithMessage("La factura seleccionada no existe.");

            // Descripción
            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("Debe ingresar una descripción.")
                .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.");

            // Unidad (puede ser null, pero si se incluye debe tener longitud válida)
            RuleFor(x => x.Unidad)
                .MaximumLength(50).WithMessage("La unidad no puede superar los 50 caracteres.");

            // Cantidad
            RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

            // Precio unitario
            RuleFor(x => x.PrecioUnitario)
                .NotNull().WithMessage("Debe ingresar el precio unitario.")
                .GreaterThan(0).WithMessage("El precio unitario debe ser mayor que cero.");

            // Porcentaje de descuento
            RuleFor(x => x.PorcentajeDescuento)
                .InclusiveBetween(0, 100)
                .When(x => x.PorcentajeDescuento.HasValue)
                .WithMessage("El porcentaje de descuento debe estar entre 0 y 100.");

            // Porcentaje de IVA
            RuleFor(x => x.PorcentajeIva)
                .InclusiveBetween(0, 100)
                .When(x => x.PorcentajeIva.HasValue)
                .WithMessage("El porcentaje de IVA debe estar entre 0 y 100.");

            // Descuento
            RuleFor(x => x.Descuento)
                .GreaterThanOrEqualTo(0).When(x => x.Descuento.HasValue)
                .WithMessage("El monto de descuento no puede ser negativo.");

            // Monto IVA
            RuleFor(x => x.MontoIva)
                .GreaterThanOrEqualTo(0).When(x => x.MontoIva.HasValue)
                .WithMessage("El monto de IVA no puede ser negativo.");

            // Base Imponible
            RuleFor(x => x.BaseImponible)
                .GreaterThanOrEqualTo(0).When(x => x.BaseImponible.HasValue)
                .WithMessage("La base imponible no puede ser negativa.");

            // Total Línea
            RuleFor(x => x.TotalLinea)
                .GreaterThan(0).When(x => x.TotalLinea.HasValue)
                .WithMessage("El total de la línea debe ser mayor que cero.");
        }
    }
}
