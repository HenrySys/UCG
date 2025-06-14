using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ActividadViewModelValidator : AbstractValidator<ActividadViewModel>
    {
        private readonly UcgdbContext _context;

        public ActividadViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la actividad es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.FechaTextoActividad)
                .NotEmpty().WithMessage("Debe ingresar la fecha de la actividad.")
                .Must(SerFechaValida).WithMessage("La fecha no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.Razon)
                .MaximumLength(500).WithMessage("La razón no puede superar los 500 caracteres.");

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una asociación válida.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
                }).WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un asociado válido.")
                .MustAsync(async (model, id, cancellation) =>
                {
                    return await _context.TbAsociados
                        .AnyAsync(a => a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion);
                }).WithMessage("El asociado no pertenece a la asociación seleccionada.");

            RuleFor(x => x.IdActa)
                .NotNull().WithMessage("Debe seleccionar un acta.")
                .GreaterThan(0).WithMessage("Debe seleccionar un acta válida.")
                .MustAsync(async (model, id, cancellation) =>
                {
                    return await _context.TbActa
                        .AnyAsync(a => a.IdActa == id && a.IdAsociacion == model.IdAsociacion);
                }).WithMessage("El acta seleccionada no pertenece a la asociación seleccionada.");

            RuleFor(x => x.Lugar)
                .MaximumLength(200).WithMessage("El lugar no puede superar los 200 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }

        private bool SerFechaValida(string? fecha)
        {
            return DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
