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

            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("Este campo no puede ser vacío")
                .EmailAddress().WithMessage("Ingrese un correo electrónico válido");



        }
    }
}
