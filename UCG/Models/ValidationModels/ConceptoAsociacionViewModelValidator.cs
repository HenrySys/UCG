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

           

            RuleFor(x => x.DescripcionPersonalizada)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200).WithMessage("La descripción no puede superar los 200 caracteres.")
                .MinimumLength(3).When(x => !string.IsNullOrWhiteSpace(x.DescripcionPersonalizada))
                    .WithMessage("La descripción debe tener al menos 3 caracteres.")
                .Must(desc => string.IsNullOrWhiteSpace(desc) || !desc.All(char.IsDigit))
                    .WithMessage("La descripción no puede ser completamente numérica.")
                .Must(desc => string.IsNullOrWhiteSpace(desc) || desc.Trim().Length > 0)
                    .WithMessage("La descripción no puede estar vacía o contener solo espacios.");

        }
    }
}
