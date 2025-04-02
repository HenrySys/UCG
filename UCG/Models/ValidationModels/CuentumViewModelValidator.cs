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

            RuleFor(x => x.IdCuenta)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbCuenta.AnyAsync(a => a.IdCuenta == id);
                })
                .WithMessage("Ya existe una cuenta con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
              .NotNull().WithMessage("Debe seleccionar un Asociado.")
              .GreaterThan(0).WithMessage("Debe seleccionar un Asociado valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
               })
               .WithMessage("El Asociado seleccionado no existe.");

            RuleFor(x => x.TituloCuenta)
                .NotNull().WithMessage("Debe ingresar un Titulo Cuenta.")
                .NotEmpty().WithMessage("Debe ingresar un Titulo Cuenta.")
                .MaximumLength(100).WithMessage("El Titulo Cuenta no puede superar los 100 caracteres.");

            RuleFor(x => x.NumeroCuenta)
                .NotNull().WithMessage("Debe ingresar un Numero Cuenta.")
                .NotEmpty().WithMessage("Debe ingresar un Numero Cuenta.")
                .GreaterThanOrEqualTo(0).WithMessage("El Numero Cuenta debe ser un valor positivo.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Telefono.")
                .NotEmpty().WithMessage("Debe ingresar un Telefono.")
                .MaximumLength(100).WithMessage("El Telfono no puede superar los 100 caracteres.");

            RuleFor(x => x.TipoCuenta)
                .IsInEnum().WithMessage("Debe seleccionar un Tipo de Cuenta.")
                .NotEmpty().WithMessage("Debe ingresar un Tipo de Cuenta.");

            RuleFor(x => x.Banco)
                .IsInEnum().WithMessage("Debe seleccionar un Banco.")
                .NotEmpty().WithMessage("Debe ingresar un Banco.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado.");

        }
    }
}