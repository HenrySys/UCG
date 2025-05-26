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
                .NotNull().WithMessage("Debe seleccionar una Asociaci�n.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociaci�n v�lida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La Asociaci�n seleccionada no existe.");

            RuleFor(x => x.TipoProveedor)
                .NotNull().WithMessage("Debe seleccionar un Tipo Proveedor.")
                .IsInEnum().WithMessage("Debe seleccionar un Tipo Proveedor v�lido.");

            RuleFor(x => x.NombreEmpresa)
                .NotNull().WithMessage("Debe ingresar un Nombre de Empresa.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Empresa.")
                .MaximumLength(100).WithMessage("El Nombre de Empresa no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.NombreEmpresa == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese nombre en la asociaci�n.");

            RuleFor(x => x.CedulaJuridica)
                .NotNull().WithMessage("Debe ingresar una C�dula Jur�dica.")
                .NotEmpty().WithMessage("Debe ingresar una C�dula Jur�dica.")
                .MaximumLength(100).WithMessage("La C�dula Jur�dica no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.CedulaJuridica == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con esa c�dula jur�dica en la asociaci�n.");

            RuleFor(x => x.NombreContacto)
                .NotNull().WithMessage("Debe ingresar un Nombre de Contacto.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Contacto.")
                .MaximumLength(100).WithMessage("El Nombre de Contacto no puede superar los 100 caracteres.");

            RuleFor(x => x.CedulaContacto)
                .NotNull().WithMessage("Debe ingresar una C�dula de Contacto.")
                .NotEmpty().WithMessage("Debe ingresar una C�dula de Contacto.")
                .MaximumLength(100).WithMessage("La C�dula de Contacto no puede superar los 100 caracteres.");

            RuleFor(x => x.Fax)
                .NotNull().WithMessage("Debe ingresar un Fax.")
                .NotEmpty().WithMessage("Debe ingresar un Fax.")
                .MaximumLength(100).WithMessage("El Fax no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.Fax == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese fax en la asociaci�n.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar un Tel�fono.")
                .NotEmpty().WithMessage("Debe ingresar un Tel�fono.")
                .MaximumLength(100).WithMessage("El Tel�fono no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.Telefono == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese tel�fono en la asociaci�n.");

            RuleFor(x => x.Correo)
                .NotNull().WithMessage("Debe ingresar un Correo.")
                .NotEmpty().WithMessage("Debe ingresar un Correo.")
                .EmailAddress().WithMessage("Debe ingresar un correo v�lido.")
                .MaximumLength(100).WithMessage("El Correo no puede superar los 100 caracteres.")
                .MustAsync(async (model, value, _) =>
                    !await _context.TbProveedors.AnyAsync(a =>
                        a.Correo == value && a.IdProveedor != model.IdProveedor && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un proveedor con ese correo en la asociaci�n.");

            RuleFor(x => x.Direccion)
                .NotNull().WithMessage("Debe ingresar una Direcci�n.")
                .NotEmpty().WithMessage("Debe ingresar una Direcci�n.")
                .MaximumLength(100).WithMessage("La Direcci�n no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("Debe seleccionar un Estado.")
                .IsInEnum().WithMessage("Debe seleccionar un Estado v�lido.");
        }
    }
}
