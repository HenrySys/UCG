using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ActaViewModelValidator : AbstractValidator<ActaViewModel>
    {
        private readonly UcgdbContext _context;

        public ActaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un Asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Asociado válido.")
                .MustAsync(async (model, id, cancellation) =>
                {
                    return await _context.TbAsociados
                        .AnyAsync(a => a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion);
                })
                .WithMessage("El asociado seleccionado no existe o no pertenece a la asociación elegida.");

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una Asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociación válida.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
                })
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.FechaSesion)
                .NotNull().WithMessage("Debe ingresar una fecha de sesión.");
                
              
            RuleFor(x => x.NumeroActa)
                .NotNull().WithMessage("Debe ingresar un número de acta.")
                .NotEmpty().WithMessage("Debe ingresar un número de acta.")
                .MaximumLength(100).WithMessage("El número de acta no puede superar los 100 caracteres.")
                .MustAsync(async (numeroActa, cancellation) =>
                {
                    return !await _context.TbActa.AnyAsync(a => a.NumeroActa == numeroActa);
                })
                .WithMessage("Ya existe un acta con el número ingresado.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una descripción.")
                .NotEmpty().WithMessage("Debe ingresar una descripción.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar un estado válido.")
                .NotEmpty().WithMessage("Debe seleccionar un estado.");

            RuleFor(x => x.MontoTotalAcordado)
                .GreaterThanOrEqualTo(0).WithMessage("El monto total acordado debe ser un valor mayor o igual a ₡0.")
                .LessThanOrEqualTo(9999999999).WithMessage("El monto total acordado no puede superar ₡9,999,999,999.");
        }
    }
}
