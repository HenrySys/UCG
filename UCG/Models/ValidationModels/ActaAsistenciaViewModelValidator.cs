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


            RuleFor(x => x.IdActaAsistencia)
                .NotNull().WithMessage("Debe de tener un id." )
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbActaAsistencia.AnyAsync(a => a.IdActaAsistencia == id);
                })
                .WithMessage("Ya existe un Acta de Asistencia con ese id.");

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
