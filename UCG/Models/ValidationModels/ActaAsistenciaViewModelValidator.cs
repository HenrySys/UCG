using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ActaAsistenciaViewModelValidator : AbstractValidator<ActaAsistenciaViewModel>
    {
        private readonly UcgdbContext _context;

        public ActaAsistenciaViewModelValidator (UcgdbContext context)
        {
            _context = context;


            RuleFor(x => x.IdAsociado)
              .NotNull().WithMessage("Debe seleccionar una Asociado.")
              .GreaterThan(0).WithMessage("Debe seleccionar un Asociado Valido .")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbActaAsistencia.AnyAsync(a => a.IdAsociado == id);
               })
               .WithMessage("El Asociado seleccionada no existe.");

        }
    }
}
