using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class AcuerdoViewModelValidator : AbstractValidator<AcuerdoViewModel>
    {
        private readonly UcgdbContext _context;

        public AcuerdoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            // Validación: ID de Acta
            RuleFor(x => x.IdActa)
                .NotNull().WithMessage("Debe seleccionar un acta.")
                .MustAsync(async (id, _) =>
                {
                    return id.HasValue && await _context.TbActa.AnyAsync(a => a.IdActa == id.Value);
                })
                .WithMessage("El acta seleccionada no existe.");

            // Validación: Número de Acuerdo
            RuleFor(x => x.NumeroAcuerdo)
                .NotEmpty().WithMessage("Debe ingresar el número del acuerdo.")
                .MaximumLength(20).WithMessage("El número del acuerdo no debe superar los 20 caracteres.")
                .MustAsync(async (NumeroAcuerdo, _) =>
                {
                    return !string.IsNullOrWhiteSpace(NumeroAcuerdo) &&
                           !await _context.TbAcuerdos.AnyAsync(a => a.NumeroAcuerdo == NumeroAcuerdo);
                })
                .WithMessage("Ya existe un acuerdo con ese nombre.");

            // Validación: Nombre del Acuerdo
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Debe ingresar un nombre para el acuerdo.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
                .MustAsync(async (nombre, _) =>
                {
                    return !string.IsNullOrWhiteSpace(nombre) &&
                           !await _context.TbAcuerdos.AnyAsync(a => a.Nombre == nombre);
                })
                .WithMessage("Ya existe un acuerdo con ese nombre.");

            // Validación: Descripción
            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una descripción.")
                .NotEmpty().WithMessage("Debe ingresar una descripción.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            // Validación: Monto
            RuleFor(x => x.MontoAcuerdo)
                .NotNull().WithMessage("Debe ingresar el monto del acuerdo.")
                .GreaterThanOrEqualTo(0).WithMessage("El monto debe ser mayor o igual a ?0.");
        }
    }
}
