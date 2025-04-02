using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class JuntaDirectivaViewModelValidator : AbstractValidator<JuntaDirectivaViewModel>
    {
        private readonly UcgdbContext _context;
        public JuntaDirectivaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdJuntaDirectiva)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbJuntaDirectivas.AnyAsync(a => a.IdJuntaDirectiva == id);
                })
                .WithMessage("Ya existe una Junta Directiva con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociacion.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociacion valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbJuntaDirectivas.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociacion seleccionada no existe.");

            RuleFor(x => x.IdActa)
                .NotNull().WithMessage("Debe seleccionar un Acta.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Acta valido.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbJuntaDirectivas.AnyAsync(a => a.IdActa == id);
                 })
               .WithMessage("El Acta seleccionado no existe.");

            RuleFor(x => x.PeriodoInicio)
                .NotNull().WithMessage("Debe seleccionar una fecha.")
                .NotEmpty().WithMessage("Debe ingresar una fecha válida.");

            RuleFor(x => x.PeriodoFin)
                .NotNull().WithMessage("Debe seleccionar una fecha.")
                .NotEmpty().WithMessage("Debe ingresar una fecha válida.");

            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("Debe ingresar un Nombre.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre.")
                .MaximumLength(100).WithMessage("El Nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado.");

        }
    }
}