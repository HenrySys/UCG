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

            RuleFor(x => x.IdCliente)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbClientes.AnyAsync(a => a.IdCliente == id);
                })
                .WithMessage("Ya existe un cliente con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbClientes.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociación seleccionada no existe.");


            RuleFor(x => x.Cedula)
                .NotNull().WithMessage("Debe ingresar una Cedula.")
                .NotEmpty().WithMessage("Debe ingresar una Cedula.")
                .MaximumLength(100).WithMessage("La cedula no puede superar los 100 caracteres.")
                .MustAsync(async (cedula, cancellation) =>
                {
                    return !await _context.TbClientes.AnyAsync(a => a.Cedula == cedula);
                })
                .WithMessage("Ya existe un Cliente con la cedula seleccionada.");

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
                .NotNull().WithMessage("Debe ingresar un Telefono.")
                .NotEmpty().WithMessage("Debe ingresar un Telefono.")
                .MaximumLength(100).WithMessage("El Telfono no puede superar los 100 caracteres.")
                .MustAsync(async (telefono, cancellation) =>
                {
                    return !await _context.TbClientes.AnyAsync(a => a.Telefono == telefono);
                })
                .WithMessage("Ya existe un asociado con el telefono seleccionado.");

            RuleFor(x => x.Correo)
                .NotNull().WithMessage("Debe ingresar un Correo.")
                .NotEmpty().WithMessage("Debe ingresar un Correo.")
                .MaximumLength(100).WithMessage("El Correo no puede superar los 100 caracteres.")
                .MustAsync(async (correo, cancellation) =>
                 {
                     return !await _context.TbClientes.AnyAsync(a => a.Correo == correo);
                 })
               .WithMessage("Ya existe un asociado con el correo seleccionado.");

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