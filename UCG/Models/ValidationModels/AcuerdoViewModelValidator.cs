using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class AcuerdoViewModelValidator: AbstractValidator<AcuerdoViewModel>
    {
        private readonly UcgdbContext _context;
        public AcuerdoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("Debe ingresar un Nombre de Proyecto.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Proyecto.")
                .MaximumLength(100).WithMessage("El Nombre de Proyecto no puede superar los 100 caracteres.")
                 .MustAsync(async (nombre, cancellation) =>
                 {
                     return !await _context.TbAcuerdos.AnyAsync(a => a.Nombre == nombre);
                 })
                .WithMessage("Ya existe un Proyecto con el nombre seleccionado.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una Descripcion.")
                .NotEmpty().WithMessage("Debe ingresar una Descripcion.")
                .MaximumLength(100).WithMessage("La Descripcion no puede superar los 100 caracteres.");

            RuleFor(x => x.MontoAcuerdo)
                .GreaterThanOrEqualTo(0).WithMessage("El Monto del Alcuerdo debe ser un valor Mayor o Igual a ¢0.");

        }
    }
}