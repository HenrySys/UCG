using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class usuarioViewModel
    {
        public int IdUsuario { get; set; }
        [Display(Name = "Nombre de usuario")]
        [Required]
        public string? NombreUsuario { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Contraseña { get; set; }
        [Required]
        public RolUsuario Rol { get; set; } 
        [Required]
        public string? Correo { get; set; }
        [Required]
        public EstadoUsuario Estado { get; set; } 
    }
}
