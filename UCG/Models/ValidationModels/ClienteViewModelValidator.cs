using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ClienteViewModelValidator : AbstractValidator<ClienteViewModel>
    {
        private readonly UcgdbContext _context;
        public ClienteViewModelValidator(UcgdbContext context)
        {
            _context = context;

          

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbClientes.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociación seleccionada no existe.");


           RuleFor(x => x.Cedula)
                .NotEmpty().WithMessage("La cédula es obligatoria.")
                .MaximumLength(20).WithMessage("La cédula no puede superar los 20 caracteres.")
                .MustAsync(async (model, cedula, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace(cedula)) return true;
                    return !await _context.TbClientes
                        .AnyAsync(c => c.Cedula == cedula && c.IdCliente != model.IdCliente && c.IdAsociacion == model.IdAsociacion);
                })
                .WithMessage("Ya existe un Cliente con la cédula seleccionada.");


            RuleFor(x => x.Apellido1)
                .NotNull().WithMessage("Debe ingresar su primer Apellido.")
                .NotEmpty().WithMessage("Debe ingresar su primer Apellido.")
                .MaximumLength(100).WithMessage("Su primer Apellido no puede superar los 100 caracteres.");

            RuleFor(x => x.Apellido2)
                .NotNull().WithMessage("Debe ingresar su segundo Apellido.")
                .NotEmpty().WithMessage("Debe ingresar su segundo Apellido.")
                .MaximumLength(100).WithMessage("Su segundo Apellido no puede superar los 100 caracteres.");

            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("Debe ingresar un Nombre.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre.")
                .MaximumLength(100).WithMessage("El Nombre no puede superar los 100 caracteres.");



            RuleFor(x => x.Telefono)
                .MaximumLength(20)
                .MustAsync(async (model, telefono, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace(telefono)) return true;
                    return !await _context.TbClientes
                        .AnyAsync(c => c.Telefono == telefono && c.IdCliente != model.IdCliente && c.IdAsociacion == model.IdAsociacion);
                })
                .WithMessage("Ya existe un Cliente con el teléfono seleccionado.");

             RuleFor(x => x.Correo)
                    .MaximumLength(100)
                    .EmailAddress()
                    .When(x => !string.IsNullOrWhiteSpace(x.Correo))
                    .WithMessage("Debe ingresar un correo válido.")
                    .MustAsync(async (model, correo, cancellation) =>
                    {
                        if (string.IsNullOrWhiteSpace(correo)) return true;
                        return !await _context.TbClientes
                            .AnyAsync(c => c.Correo == correo && c.IdCliente != model.IdCliente && c.IdAsociacion == model.IdAsociacion);
                    })
                    .WithMessage("Ya existe un Cliente con el correo seleccionado.");


            RuleFor(x => x.Direccion)
                .NotNull().WithMessage("Debe ingresar una Direccion.")
                .NotEmpty().WithMessage("Debe ingresar una Direccion.")
                .MaximumLength(100).WithMessage("La Direccion no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado.");

        }
    }
}