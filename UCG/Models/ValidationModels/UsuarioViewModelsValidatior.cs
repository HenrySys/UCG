using FluentValidation;

namespace UCG.Models.ViewModels
{
    public class UsuarioViewModelsValidatior : AbstractValidator<UsuarioViewModel>
    {
        public UsuarioViewModelsValidatior() // Corregido: ahora coincide con el nombre de la clase
        {
            RuleFor(x => x.NombreUsuario)
                .NotEmpty().WithMessage("Este campo no puede ser vacío")
                .Length(4, 100).WithMessage("El nombre de usuario debe de tener entre 3 y 50 caracteres");

            RuleFor(x => x.Contraseña)
                .NotEmpty().WithMessage("Este campo no puede ser vacío")
                .Length(4, 100).WithMessage("La Constraseña debe de tener entre 3 y 50 caracteres");

            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("Este campo no puede ser vacío")
                .EmailAddress().WithMessage("Ingrese un correo electrónico válido");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("Debe seleccionar una estado valido.")
                .NotEmpty().WithMessage("Debe ingresar un estado.");

            RuleFor(x => x.Rol)
                .IsInEnum().WithMessage("Debe seleccionar una Rol valido.")
                .NotEmpty().WithMessage("Debe ingresar un Rol.");


        }
    }
}
