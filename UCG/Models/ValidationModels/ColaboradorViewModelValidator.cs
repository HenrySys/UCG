using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ColaboradorViewModelValidator : AbstractValidator<ColaboradorViewModel>
    {
        private readonly UcgdbContext _context;

        public ColaboradorViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una asociación válida.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
                }).WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("El nombre es obligatorio.")
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Cedula)
                .NotNull().WithMessage("La cédula no puede ser nula.") // Se permite vacía pero no nula
                .MaximumLength(20).WithMessage("La cédula no puede superar los 20 caracteres.")
                .Matches(@"^\d{9,12}$").When(x => !string.IsNullOrWhiteSpace(x.Cedula))
                .WithMessage("La cédula debe contener entre 9 y 12 dígitos numéricos.")
                .MustAsync(async (model, correo, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace(correo)) return true;
                    return !await _context.TbColaboradors
                        .AnyAsync(c => c.Correo == correo && c.IdColaborador != model.IdColaborador && c.IdAsociacion == model.IdAsociacion);
                })
                .WithMessage("Ya existe un colaborador con ese correo en la asociación.");


            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("El teléfono no puede ser nulo.") // Se permite vacía pero no nula
                .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres.")
                .Matches(@"^\d{8}$").When(x => !string.IsNullOrWhiteSpace(x.Telefono))
                .WithMessage("El número de teléfono debe tener 8 dígitos.")
                .MustAsync(async (model, telefono, cancellation) =>
                 {
                     if (string.IsNullOrWhiteSpace(telefono)) return true;
                     return !await _context.TbColaboradors
                         .AnyAsync(c => c.Telefono == telefono && c.IdColaborador != model.IdColaborador && c.IdAsociacion == model.IdAsociacion);
                 })
                .WithMessage("Ya existe un colaborador con ese teléfono en la asociación.");


            RuleFor(x => x.Correo)
                .NotNull().WithMessage("El correo no puede ser nulo.")
                .MaximumLength(100).WithMessage("El correo no puede superar los 100 caracteres.")
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Correo))
                .WithMessage("Debe ingresar un correo válido.");

            RuleFor(x => x.Observaciones)
                .NotNull().WithMessage("Las observaciones no pueden ser nulas.")
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }
    }
}
