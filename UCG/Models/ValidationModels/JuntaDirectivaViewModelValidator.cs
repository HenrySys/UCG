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

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una Asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociación válida.")
                .MustAsync(async (id, cancellation) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.IdActa)
                .GreaterThan(0).WithMessage("Debe seleccionar un Acta válida.")
                .MustAsync(async (id, cancellation) =>
                    await _context.TbActa.AnyAsync(a => a.IdActa == id))
                .WithMessage("El Acta seleccionada no existe.");

            RuleFor(x => x.PeriodoInicio)
                .NotNull().WithMessage("Debe seleccionar una fecha de inicio.")
                .NotEmpty().WithMessage("Debe ingresar una fecha de inicio válida.");

            RuleFor(x => x.PeriodoFin)
                .NotNull().WithMessage("Debe seleccionar una fecha de fin.")
                .NotEmpty().WithMessage("Debe ingresar una fecha de fin válida.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Debe ingresar un nombre.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar un estado válido.");


        }
    }
}
