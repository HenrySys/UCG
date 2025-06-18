using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels

{
    public class AsociacionViewModelValidator : AbstractValidator<AsociacionViewModel>
    {
        private readonly UcgdbContext _context;
        public AsociacionViewModelValidator(UcgdbContext context)
        {
            _context = context;

           
            RuleFor(x => x.CedulaJuridica)
                .NotNull().WithMessage("Debe ingresar una Cedula Juridica.")
                .NotEmpty().WithMessage("Debe ingresar una Cedula Juridica.")
                .MaximumLength(100).WithMessage("La cedula juridica no puede superar los 100 caracteres.");

            RuleFor(x => x.CodigoRegistro)
                .NotNull().WithMessage("Debe ingresar un Codigo Registro.")
                .NotEmpty().WithMessage("Debe ingresar un Codigo Registro.")
                .MaximumLength(100).WithMessage("El Codigo Registro no puede superar los 100 caracteres.");

            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("Debe ingresar un nombre.")
                .NotEmpty().WithMessage("Debe ingresar un nombre.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.FechaConstitucionTexto)
                .NotNull().WithMessage("Debe seleccionar una fecha.")
                .Must(ValidarFechaTexto).WithMessage("La fecha debe tener el formato válido yyyy-MM-dd y estar entre el año 1940 y hoy.");

            RuleFor(x => x.Telefono)
                .NotNull().WithMessage("Debe ingresar una Telefono.")
                .NotEmpty().WithMessage("Debe ingresar una Telefono.")
                .MaximumLength(100).WithMessage("El Telefono no puede superar los 100 caracteres.");

            RuleFor(x => x.Fax)
                .NotNull().WithMessage("Debe ingresar una Fax.")
                .NotEmpty().WithMessage("Debe ingresar una Fax.")
                .MaximumLength(100).WithMessage("El Fax no puede superar los 100 caracteres.");

            RuleFor(x => x.Correo)
                .EmailAddress().WithMessage("Debe ingresar un correo válido.")
                .NotEmpty().WithMessage("Debe ingresar un Correo.")
                .MaximumLength(100).WithMessage("El correo no puede superar los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una descripción.")
                .NotEmpty().WithMessage("Debe ingresar una descripción.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");

            RuleFor(x => x.Provincia)
                .NotNull().WithMessage("Debe ingresar una provincia.")
                .NotEmpty().WithMessage("Debe ingresar una provincia.")
                .MaximumLength(100).WithMessage("La provincia no puede superar los 100 caracteres.");

            RuleFor(x => x.Canton)
               .NotNull().WithMessage("Debe ingresar una canton.")
               .NotEmpty().WithMessage("Debe ingresar una canton.")
               .MaximumLength(100).WithMessage("el canton no puede superar los 100 caracteres.");

            RuleFor(x => x.Distrito)
               .NotNull().WithMessage("Debe ingresar una distrito.")
               .NotEmpty().WithMessage("Debe ingresar una distrito.")
               .MaximumLength(100).WithMessage("La distrito no puede superar los 100 caracteres.");

            RuleFor(x => x.Pueblo)
                .NotNull().WithMessage("Debe ingresar una pueblo.")
                .NotEmpty().WithMessage("Debe ingresar una pueblo.")
                .MaximumLength(100).WithMessage("La pueblo no puede superar los 100 caracteres.");

            RuleFor(x => x.Direccion)
               .NotNull().WithMessage("Debe ingresar una direccion.")
               .NotEmpty().WithMessage("Debe ingresar una direccion.")
               .MaximumLength(100).WithMessage("La direccion no puede superar los 100 caracteres.");

            RuleFor(x => x.Estado)
               .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
               .NotEmpty().WithMessage("Debe ingresar un estado.");
        }
        private bool ValidarFechaTexto(string? fechaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto))
                return false;

            if (!DateOnly.TryParseExact(fechaTexto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return false;

            if (fecha > DateOnly.FromDateTime(DateTime.Today)) return false;
            if (fecha < new DateOnly(1940, 1, 1)) return false;

            return true;
        }
    }
}
