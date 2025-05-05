using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ConceptoViewModelValidator : AbstractValidator<ConceptoMovimientoViewModel>
    {
        private readonly UcgdbContext _context;
        public ConceptoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.TipoMovimiento)
               .NotNull().WithMessage("Debe seleccionar un movimiento.")
               .NotEmpty().WithMessage("Debe ingresar un movimiento.")
               .IsInEnum().WithMessage("Debe seleccionar un tipo de movimiento valido.");

            RuleFor(x => x.Concepto)
                .NotNull().WithMessage("Debe ingresar un concepto.")
                .NotEmpty().WithMessage("Debe ingresar un concepto.")
                .MaximumLength(100).WithMessage("El concepto no puede superar los 100 caracteres.");

        }
    }
}
