using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ActaViewModelValidator : AbstractValidator<ActaViewModel>
    {
        private readonly UcgdbContext _context;

        public  ActaViewModelValidator(UcgdbContext context)
        {
            _context = context;


            RuleFor(x => x.IdAsociado)
                 .NotNull().WithMessage("Debe seleccionar una Asociado.")
                 .GreaterThan(0).WithMessage("Debe seleccionar una Acta valida.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
                 })
                 .WithMessage("El asociado seleccionada no existe.");

             RuleFor(x => x.IdAsociacion)
                 .NotNull().WithMessage("Debe seleccionar una Acta.")
                 .GreaterThan(0).WithMessage("Debe seleccionar una Acta valida.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
                 })
                 .WithMessage("El asociacion seleccionada no existe.");

            RuleFor(x => x.FechaSesion)
                .NotNull().WithMessage("Debe ingresar una fecha de sesion.")
                .NotEmpty().WithMessage("Debe ingresar una fecha de sesion.")
                .Must(date => date != default(DateOnly)).WithMessage("La fecha de sesion no es valida.");

            RuleFor(x => x.NumeroActa)
                .NotNull().WithMessage("Debe ingresar un numero de acta.")
                .NotEmpty().WithMessage("Debe ingresar un numero de acta.")
                .MaximumLength(100).WithMessage("El numero de acta no puede superar los 100 caracteres.")
                .MustAsync(async (numeroActa, cancellation) =>
                {
                    return !await _context.TbActa.AnyAsync(a => a.NumeroActa == numeroActa);
                })
                .WithMessage("Ya existe un Acta con el numero seleccionado.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una descripcion.")
                .NotEmpty().WithMessage("Debe ingresar una descripcion.")
                .MaximumLength(100).WithMessage("La descripcion no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado.");

            RuleFor(x => x.MontoTotalAcordado)
                .GreaterThanOrEqualTo(0).WithMessage("El Monto Total Acordado debe ser un valor Mayor o Igual a ¢0.")
                .LessThanOrEqualTo(9999999999).WithMessage("El Monto Total Acordado no puede ser mayor a ¢9.999.999.999.");

        }
    }
}
