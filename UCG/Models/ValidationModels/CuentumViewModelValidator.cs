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
                .NotNull().WithMessage("Debe seleccionar una Asociaci�n.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociaci�n v�lida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociaci�n seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un Asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Asociado v�lido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbAsociados.AnyAsync(a =>
                        a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El Asociado no pertenece a la asociaci�n seleccionada.");

            RuleFor(x => x.TituloCuenta)
                .NotNull().WithMessage("Debe ingresar un T�tulo de Cuenta.")
                .NotEmpty().WithMessage("Debe ingresar un T�tulo de Cuenta.")
                .MaximumLength(100).WithMessage("El T�tulo de Cuenta no puede superar los 100 caracteres.");

            RuleFor(x => x.NumeroCuenta)
                .NotNull().WithMessage("Debe ingresar un N�mero de Cuenta.")
                //.GreaterThanOrEqualTo(0).WithMessage("El N�mero de Cuenta debe ser un valor positivo.")
                ;

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Tel�fono.")
                .NotEmpty().WithMessage("Debe ingresar un Tel�fono.")
                .MaximumLength(20).WithMessage("El Tel�fono no puede superar los 20 caracteres.")
                .Matches(@"^\d{8}$").WithMessage("El Tel�fono debe contener exactamente 8 d�gitos.");

            RuleFor(x => x.TipoCuenta)
                .NotNull().WithMessage("Debe seleccionar un Tipo de Cuenta.")
                .IsInEnum().WithMessage("Debe seleccionar un Tipo de Cuenta v�lido.");

            RuleFor(x => x.Banco)
                .NotNull().WithMessage("Debe seleccionar un Banco.")
                .IsInEnum().WithMessage("Debe seleccionar un Banco v�lido.");

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("Debe seleccionar un estado.")
                .IsInEnum().WithMessage("Debe seleccionar un estado v�lido.");
        }
    }
}
