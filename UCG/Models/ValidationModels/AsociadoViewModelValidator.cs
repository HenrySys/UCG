using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class AsociadoViewModelValidator : AbstractValidator<AsociadoViewModel>
    {
        private readonly UcgdbContext _context;
        public AsociadoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
                })
                .WithMessage("Ya existe un asociado con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.IdUsuario)
              .NotNull().WithMessage("Debe seleccionar un Usuario.")
              .GreaterThan(0).WithMessage("Debe seleccionar un Usuario valido.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbUsuarios.AnyAsync(a => a.IdUsuario == id && a.TbAsociados == null);
               })
               .WithMessage("El Usuario seleccionado no existe.");

            RuleFor(x => x.Nacionalidad)
                .IsInEnum().WithMessage("Debe seleccionar una Nacionalidad valido.")
                .NotEmpty().WithMessage("Debe ingresar una Nacionalidad.");

            RuleFor(x => x.Cedula)
                .NotNull().WithMessage("Debe ingresar una Cedula.")
                .NotEmpty().WithMessage("Debe ingresar una Cedula.")
                .MaximumLength(100).WithMessage("La cedula no puede superar los 100 caracteres.")
                .MustAsync(async (cedula, cancellation) =>
                {
                    return !await _context.TbAsociados.AnyAsync(a => a.Cedula == cedula);
                })
                .WithMessage("Ya existe un asociado con la cedula seleccionada.");

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

            RuleFor(x => x.FechaNacimiento)
                .NotNull().WithMessage("Debe seleccionar una fecha.")
                .NotEmpty().WithMessage("Debe ingresar una fecha válida.");

            RuleFor(x => x.Sexo)
                .IsInEnum().WithMessage("Debe ingresar una Sexo valido.")
                .NotEmpty().WithMessage("Debe ingresar una Sexo valido.");
             

            RuleFor(x => x.EstadoCivil)
                .IsInEnum().WithMessage("Debe seleccionar una estado civil valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado civil.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Telefono.")
                .NotEmpty().WithMessage("Debe ingresar un Telefono.")
                .MaximumLength(100).WithMessage("El Telfono no puede superar los 100 caracteres.")
                .MustAsync(async (telefono, cancellation) =>
                {
                    return !await _context.TbAsociados.AnyAsync(a => a.Telefono == telefono);
                })
                .WithMessage("Ya existe un asociado con el telefono seleccionado.");

            RuleFor(x => x.Correo)
                .NotNull().WithMessage("Debe ingresar un Correo.")
                .NotEmpty().WithMessage("Debe ingresar un Correo.")
                .MaximumLength(100).WithMessage("El Correo no puede superar los 100 caracteres.")
                .MustAsync(async (correo, cancellation) =>
                 {
                     return !await _context.TbAsociados.AnyAsync(a => a.Correo == correo);
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