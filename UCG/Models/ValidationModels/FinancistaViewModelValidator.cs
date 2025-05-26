using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class FinancistaViewModelValidator : AbstractValidator<FinancistaViewModel>
    {
        private readonly UcgdbContext _context;

        public FinancistaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una asociación válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del financista es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
                .MustAsync(async (model, nombre, _) =>
                    !await _context.TbFinancista
                        .AnyAsync(f => f.Nombre == nombre && f.IdFinancista != model.IdFinancista && f.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe un financista con ese nombre en la asociación.");

            RuleFor(x => x.TipoEntidad)
                .NotNull().WithMessage("Debe seleccionar un tipo de entidad.")
                .IsInEnum().WithMessage("Debe seleccionar un tipo de entidad válido.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("La descripción no puede ser nula.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("El teléfono no puede ser nulo.")
                .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres.")
                .Matches(@"^\d{8}$").When(x => !string.IsNullOrWhiteSpace(x.Telefono))
                .WithMessage("El teléfono debe contener exactamente 8 dígitos.");

            RuleFor(x => x.Correo)
                .NotNull().WithMessage("El correo no puede ser nulo.")
                .MaximumLength(100).WithMessage("El correo no puede superar los 100 caracteres.")
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Correo))
                .WithMessage("Debe ingresar un correo válido.");

            RuleFor(x => x.SitioWeb)
                .NotNull().WithMessage("El sitio web no puede ser nulo.")
                .MaximumLength(200).WithMessage("El sitio web no puede superar los 200 caracteres.")
                .Matches(@"^https?:\/\/.*$").When(x => !string.IsNullOrWhiteSpace(x.SitioWeb))
                .WithMessage("Debe ingresar una URL válida que inicie con http:// o https://");
        }
    }
}
