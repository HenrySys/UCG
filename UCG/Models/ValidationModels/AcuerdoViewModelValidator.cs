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

            RuleFor(x => x.NumeroAcuerdo)
            .NotEmpty().WithMessage("Debe ingresar el n�mero del acuerdo.")
            .MaximumLength(20).WithMessage("El n�mero del acuerdo no debe superar los 20 caracteres.")
            .MustAsync(async (viewModel, numeroAcuerdo, _) =>
            {
                return !string.IsNullOrWhiteSpace(numeroAcuerdo) &&
                       !await _context.TbAcuerdos
                           .AnyAsync(a => a.NumeroAcuerdo == numeroAcuerdo && a.IdActa == viewModel.IdActa);
            })
            .WithMessage("Ya existe un acuerdo con ese n�mero en esta acta.");


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

            RuleFor(x => x.Tipo)
              .IsInEnum().WithMessage("Debe seleccionar un tipo v�lido.")
              .NotEmpty().WithMessage("Debe seleccionar un tipo.");
        }
    }
}
