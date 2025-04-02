using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class CategoriaViewModelValidator : AbstractValidator<CategoriaMovimientoViewModel>
    {
        private readonly UcgdbContext _context;
        public CategoriaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdCategoriaMovimiento)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbCategoriaMovimientos.AnyAsync(a => a.IdCategoriaMovimiento == id);
                })
                .WithMessage("Ya existe una categoria con ese id.");

            RuleFor(x => x.IdConceptoAsociacion)
                .NotNull().WithMessage("Debe de tener un id de concepto asociacion.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbConceptoAsociacions.AnyAsync(a => a.IdConceptoAsociacion == id);
                })
                .WithMessage("El concepto asociacion seleccionado no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un asociado valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
                })
                .WithMessage("El asociado seleccionado no existe.");

            RuleFor(x => x.NombreCategoria)
                .NotNull().WithMessage("Debe ingresar un nombre.")
                .NotEmpty().WithMessage("Debe ingresar un nombre.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una descripcion.")
                .NotEmpty().WithMessage("Debe ingresar una descripcion.")
                .MaximumLength(100).WithMessage("La descripcion no puede superar los 100 caracteres.");
        }
    }
}
