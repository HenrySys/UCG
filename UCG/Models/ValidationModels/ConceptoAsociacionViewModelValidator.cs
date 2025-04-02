using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ConceptoAsociacionViewModelValidator : AbstractValidator<ConceptoAsociacionViewModel>
    {
        private readonly UcgdbContext _context;
        public ConceptoAsociacionViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdConceptoAsociacion)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbConceptoAsociacions.AnyAsync(a => a.IdConceptoAsociacion == id );
                })
                .WithMessage("Ya existe un concepto con ese id.");

            RuleFor(x => x.IdConcepto)
                .NotNull().WithMessage("Debe de tener un id de concepto.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbConceptoMovimientos.AnyAsync(a => a.IdConceptoMovimiento == id);
                })
                .WithMessage("El concepto seleccionado no existe.");

            RuleFor(x => x.IdAsociacion)
               .NotNull().WithMessage("Debe seleccionar una asociación.")
               .GreaterThan(0).WithMessage("Debe seleccionar una asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => new { x.IdConcepto, x.IdAsociacion })
                .MustAsync(async (ids, cancellation) =>
                {
                    return !await _context.TbConceptoAsociacions
                        .AnyAsync(a => a.IdConcepto == ids.IdConcepto && a.IdAsociacion == ids.IdAsociacion);
                })
                .WithMessage("Ya existe un concepto de asociación con el mismo IdConcepto e IdAsociacion.");


        }
    }
}
