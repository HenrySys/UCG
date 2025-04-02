using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class MiembroJuntaDirectivaViewModelValidator : AbstractValidator<MiembroJuntaDirectivaViewModel>
    {
        private readonly UcgdbContext _context;
        public MiembroJuntaDirectivaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdMiembrosJuntaDirectiva)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbMiembroJuntaDirectivas.AnyAsync(a => a.IdMiembrosJuntaDirectiva == id);
                })
                .WithMessage("Ya existe un Miembro de Junta Directiva con ese id.");

            RuleFor(x => x.IdJuntaDirectiva)
              .NotNull().WithMessage("Debe seleccionar una Junta Directiva.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Junta Directiva valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbMiembroJuntaDirectivas.AnyAsync(a => a.IdJuntaDirectiva == id);
               })
               .WithMessage("La Junta Directiva seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un Asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Asociado valido.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbMiembroJuntaDirectivas.AnyAsync(a => a.IdAsociado == id);
                 })
               .WithMessage("El Asociado seleccionado no existe.");

            RuleFor(x => x.IdPuesto)
              .NotNull().WithMessage("Debe seleccionar un Puesto.")
              .GreaterThan(0).WithMessage("Debe seleccionar un Puesto valido.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbMiembroJuntaDirectivas.AnyAsync(a => a.IdPuesto == id);
               })
               .WithMessage("El Puesto seleccionado no existe.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado.");

        }
    }
}