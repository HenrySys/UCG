using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ActaViewModelValidator : AbstractValidator<ActaViewModel>
    {
        private readonly UcgdbContext _context;

        public  ActaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdActa)
            .NotNull().WithMessage("Debe seleccionar una Acta.")
            .GreaterThan(0).WithMessage("Debe seleccionar una Acta valida.")
             .MustAsync(async (id, cancellation) =>
             {
                 return !await _context.TbActa.AnyAsync(a => a.IdActa == id);
             })
             .WithMessage("Ya existe un Acta con ese id.");

            RuleFor(x => x.IdAsociado)
            .NotNull().WithMessage("Debe seleccionar una Acta.")
            .GreaterThan(0).WithMessage("Debe seleccionar una Acta valida.")
             .MustAsync(async (id, cancellation) =>
             {
                 return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
             })
             .WithMessage("El asociado seleccionada no existe.");




        }

    }
}
