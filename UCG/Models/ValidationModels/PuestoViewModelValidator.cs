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

            RuleFor(x => x.NombrePuesto)
                .NotNull().WithMessage("Debe ingresar un nombre de puesto.")
                .NotEmpty().WithMessage("Debe ingresar un nombre de puesto.")
                .MaximumLength(100).WithMessage("El nombre de puesto no puede superar los 100 caracteres.")
                .MustAsync(async (model, nombre, cancellation) =>
                {
                    return !await _context.TbPuestos
                        .AnyAsync(p => p.Nombre == nombre && p.IdPuesto != model.IdPuesto);
                })
                .WithMessage("Ya existe un puesto con ese nombre.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
