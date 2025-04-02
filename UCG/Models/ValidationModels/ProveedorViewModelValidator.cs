using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ProveedorViewModelValidator : AbstractValidator<ProveedorViewModel>
    {
        private readonly UcgdbContext _context;
        public ProveedorViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdProveedor)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbProveedors.AnyAsync(a => a.IdProveedor == id);
                })
                .WithMessage("Ya existe un proveedor con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.TipoProveedor)
                .NotEmpty().WithMessage("Debe ingresar un Tipo Proveedor.")
                .IsInEnum().WithMessage("Debe seleccionar un Tipo Proveedor valido.")
              ;

            RuleFor(x => x.NombreEmpresa)
                .NotNull().WithMessage("Debe ingresar un Nombre de Empresa.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Empresa.")
                .MaximumLength(100).WithMessage("El Nombre de Empresa no puede superar los 100 caracteres.")
                 .MustAsync(async (nombreEmpresa, cancellation) =>
                 {
                     return !await _context.TbProveedors.AnyAsync(a => a.NombreEmpresa == nombreEmpresa);
                 })
                .WithMessage("Ya existe un proveedor con el nombre seleccionado.");


            RuleFor(x => x.CedulaJuridica)
                .NotNull().WithMessage("Debe ingresar su Cedula Juridica.")
                .NotEmpty().WithMessage("Debe ingresar su Cedula Juridica.")
                .MaximumLength(100).WithMessage("Su Cedula Juridica no puede superar los 100 caracteres.")
                .MustAsync(async (cedulaJuridica, cancellation) =>
                {
                    return !await _context.TbProveedors.AnyAsync(a => a.CedulaJuridica == cedulaJuridica);
                })
                .WithMessage("Ya existe un proveedor con el telefono seleccionado.");

            RuleFor(x => x.NombreContacto)
                .NotNull().WithMessage("Debe ingresar su Nombre Contacto.")
                .NotEmpty().WithMessage("Debe ingresar su Nombre Contacto.")
                .MaximumLength(100).WithMessage("Su Nombre Contacto no puede superar los 100 caracteres.");

            RuleFor(x => x.CedulaContacto)
                .NotNull().WithMessage("Debe ingresar una Cedula de Contacto.")
                .NotEmpty().WithMessage("Debe ingresar una Cedula de Contacto.")
                .MaximumLength(100).WithMessage("La Cedula de Contacto no puede superar los 100 caracteres.");

            RuleFor(x => x.Fax)
                .NotNull().WithMessage("Debe ingresar un Fax.")
                .NotEmpty().WithMessage("Debe ingresar un Fax.")
                .MaximumLength(100).WithMessage("El Fax no puede superar los 100 caracteres.")
                 .MustAsync(async (fax, cancellation) =>
                  {
                      return !await _context.TbProveedors.AnyAsync(a => a.Fax == fax);
                  })
                .WithMessage("Ya existe un proveedor con el fax seleccionado.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Telefono.")
                .NotEmpty().WithMessage("Debe ingresar un Telefono.")
                .MaximumLength(100).WithMessage("El Telfono no puede superar los 100 caracteres.")
                 .MustAsync(async (telefono, cancellation) =>
                  {
                      return !await _context.TbProveedors.AnyAsync(a => a.Telefono == telefono);
                  })
                .WithMessage("Ya existe un proveedor con el telefono seleccionado.");

            RuleFor(x => x.Correo)
                .NotNull().WithMessage("Debe ingresar un Correo.")
                .NotEmpty().WithMessage("Debe ingresar un Correo.")
                .MaximumLength(100).WithMessage("El Correo no puede superar los 100 caracteres.")
                .MustAsync(async (correo, cancellation) =>
                {
                    return !await _context.TbProveedors.AnyAsync(a => a.Correo == correo);
                })
               .WithMessage("Ya existe un proveedor con el correo seleccionado.");

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