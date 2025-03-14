using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string? Correo { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string? Contraseña { get; set; }

    }
}
