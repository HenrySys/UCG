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

            // Validaci�n: ID de Acta
            RuleFor(x => x.IdActa)
                .NotNull().WithMessage("Debe seleccionar un acta.")
                .MustAsync(async (id, _) =>
                {
                    return id.HasValue && await _context.TbActa.AnyAsync(a => a.IdActa == id.Value);
                })
                .WithMessage("El acta seleccionada no existe.");

            // Validaci�n: N�mero de Acuerdo
            RuleFor(x => x.NumeroAcuerdo)
                .NotEmpty().WithMessage("Debe ingresar el n�mero del acuerdo.")
                .MaximumLength(20).WithMessage("El n�mero del acuerdo no debe superar los 20 caracteres.")
                .MustAsync(async (NumeroAcuerdo, _) =>
                {
                    return !string.IsNullOrWhiteSpace(NumeroAcuerdo) &&
                           !await _context.TbAcuerdos.AnyAsync(a => a.NumeroAcuerdo == NumeroAcuerdo);
                })
                .WithMessage("Ya existe un acuerdo con ese nombre.");

            // Validaci�n: Nombre del Acuerdo
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Debe ingresar un nombre para el acuerdo.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
                .MustAsync(async (nombre, _) =>
                {
                    return !string.IsNullOrWhiteSpace(nombre) &&
                           !await _context.TbAcuerdos.AnyAsync(a => a.Nombre == nombre);
                })
                .WithMessage("Ya existe un acuerdo con ese nombre.");

            // Validaci�n: Descripci�n
            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una descripci�n.")
                .NotEmpty().WithMessage("Debe ingresar una descripci�n.")
                .MaximumLength(500).WithMessage("La descripci�n no puede superar los 500 caracteres.");

            // Validaci�n: Monto
            RuleFor(x => x.MontoAcuerdo)
                .NotNull().WithMessage("Debe ingresar el monto del acuerdo.")
                .GreaterThanOrEqualTo(0).WithMessage("El monto debe ser mayor o igual a ?0.");
        }
    }
}
