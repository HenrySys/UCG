using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class CuentumViewModelValidator : AbstractValidator<CuentumViewModel>
    {
        private readonly UcgdbContext _context;

        public CuentumViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una Asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociación válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un Asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Asociado válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbAsociados.AnyAsync(a =>
                        a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El Asociado no pertenece a la asociación seleccionada.");

            RuleFor(x => x.TituloCuenta)
                .NotNull().WithMessage("Debe ingresar un Título de Cuenta.")
                .NotEmpty().WithMessage("Debe ingresar un Título de Cuenta.")
                .MaximumLength(100).WithMessage("El Título de Cuenta no puede superar los 100 caracteres.");

            RuleFor(x => x.NumeroCuenta)
                .NotNull().WithMessage("Debe ingresar un Número de Cuenta.")
                //.GreaterThanOrEqualTo(0).WithMessage("El Número de Cuenta debe ser un valor positivo.")
                ;

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Teléfono.")
                .NotEmpty().WithMessage("Debe ingresar un Teléfono.")
                .MaximumLength(20).WithMessage("El Teléfono no puede superar los 20 caracteres.")
                .Matches(@"^\d{8}$").WithMessage("El Teléfono debe contener exactamente 8 dígitos.");

            RuleFor(x => x.TipoCuenta)
                .NotNull().WithMessage("Debe seleccionar un Tipo de Cuenta.")
                .IsInEnum().WithMessage("Debe seleccionar un Tipo de Cuenta válido.");

            RuleFor(x => x.Banco)
                .NotNull().WithMessage("Debe seleccionar un Banco.")
                .IsInEnum().WithMessage("Debe seleccionar un Banco válido.");

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("Debe seleccionar un estado.")
                .IsInEnum().WithMessage("Debe seleccionar un estado válido.");
        }
    }
}
