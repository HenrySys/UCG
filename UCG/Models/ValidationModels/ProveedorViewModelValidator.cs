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

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una Asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociación válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.TipoProveedor)
                .NotNull().WithMessage("Debe seleccionar un Tipo Proveedor.")
                .IsInEnum().WithMessage("Debe seleccionar un Tipo Proveedor válido.");

            RuleFor(x => x.NombreEmpresa)
                .NotNull().WithMessage("Debe ingresar un Nombre de Empresa.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Empresa.")
                .MaximumLength(100).WithMessage("El Nombre de Empresa no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.NombreEmpresa == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese nombre en la asociación.");

            RuleFor(x => x.CedulaJuridica)
                .NotNull().WithMessage("Debe ingresar una Cédula Jurídica.")
                .NotEmpty().WithMessage("Debe ingresar una Cédula Jurídica.")
                .MaximumLength(100).WithMessage("La Cédula Jurídica no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.CedulaJuridica == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con esa cédula jurídica en la asociación.");

            RuleFor(x => x.NombreContacto)
                .NotNull().WithMessage("Debe ingresar un Nombre de Contacto.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Contacto.")
                .MaximumLength(100).WithMessage("El Nombre de Contacto no puede superar los 100 caracteres.");

            RuleFor(x => x.CedulaContacto)
                .NotNull().WithMessage("Debe ingresar una Cédula de Contacto.")
                .NotEmpty().WithMessage("Debe ingresar una Cédula de Contacto.")
                .MaximumLength(100).WithMessage("La Cédula de Contacto no puede superar los 100 caracteres.");

            RuleFor(x => x.Fax)
                .NotNull().WithMessage("Debe ingresar un Fax.")
                .NotEmpty().WithMessage("Debe ingresar un Fax.")
                .MaximumLength(100).WithMessage("El Fax no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.Fax == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese fax en la asociación.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Teléfono.")
                .NotEmpty().WithMessage("Debe ingresar un Teléfono.")
                .MaximumLength(100).WithMessage("El Teléfono no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.Telefono == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese teléfono en la asociación.");

            RuleFor(x => x.Correo)
                .NotNull().WithMessage("Debe ingresar un Correo.")
                .NotEmpty().WithMessage("Debe ingresar un Correo.")
                .EmailAddress().WithMessage("Debe ingresar un correo válido.")
                .MaximumLength(100).WithMessage("El Correo no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.Correo == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese correo en la asociación.");

            RuleFor(x => x.Direccion)
                .NotNull().WithMessage("Debe ingresar una Dirección.")
                .NotEmpty().WithMessage("Debe ingresar una Dirección.")
                .MaximumLength(100).WithMessage("La Dirección no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("Debe seleccionar un Estado.")
                .IsInEnum().WithMessage("Debe seleccionar un Estado válido.");
        }
    }
}
