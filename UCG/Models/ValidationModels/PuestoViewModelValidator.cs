using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;


namespace UCG.Models.ValidationModels
{
    public class PuestoViewModelValidator : AbstractValidator<PuestoViewModel>
    {
        private readonly UcgdbContext _context;
        public PuestoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdPuesto)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbPuestos.AnyAsync(a => a.IdPuesto == id);
                })
                .WithMessage("Ya existe un puesto con ese id.");

            RuleFor(x => x.NombrePuesto)
                .NotNull().WithMessage("Debe de tener un nombre de puesto.")
                .NotEmpty().WithMessage("Debe ingresar un nombre de puesto.")
                .MaximumLength(100).WithMessage("El nombre de puesto no puede superar los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }       
    }
}
