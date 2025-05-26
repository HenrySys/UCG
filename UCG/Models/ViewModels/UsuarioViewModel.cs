using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbUsuario;

namespace UCG.Models.ViewModels
{
   public class UsuarioViewModel
   {

       public int IdUsuario { get; set; }

       [Display(Name = "Asociación")]
       public int? IdAsociacion { get; set; }

        [Display(Name = "Nombre de usuario")]
       public string? NombreUsuario { get; set; }

       [DataType(DataType.Password)]
       public string? Contraseña { get; set; }

       [Display(Name = "Confirmar contraseña")]
       [DataType(DataType.Password)]
       public string? ConfirmarContraseña { get; set; }

       [Display(Name = "Rol de usuario")]
       public RolUsuario? Rol { get; set; }

       [DataType(DataType.EmailAddress)]
       public string? Correo { get; set; }

       public EstadoUsuario? Estado { get; set; } 

   }
}
