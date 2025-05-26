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

            RuleFor(x => x.IdJuntaDirectiva)
                .NotNull().WithMessage("Debe seleccionar una Junta Directiva.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Junta Directiva válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbJuntaDirectivas.AnyAsync(j => j.IdJuntaDirectiva == id))
                .WithMessage("La Junta Directiva seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un Asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Asociado válido.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id))
                .WithMessage("El Asociado seleccionado no existe.");

            RuleFor(x => x.IdPuesto)
                .NotNull().WithMessage("Debe seleccionar un Puesto.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Puesto válido.")
                .MustAsync(async (id, _) =>
                    await _context.TbPuestos.AnyAsync(p => p.IdPuesto == id))
                .WithMessage("El Puesto seleccionado no existe.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar un estado válido.");
        }
    }
}
