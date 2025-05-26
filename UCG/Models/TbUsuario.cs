using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbUsuario
{

    public enum RolUsuario
    {
        [Display(Name = "Admin")]
        Admin = 1,

        [Display(Name = "root")]
        root = 2
    }

    public enum EstadoUsuario
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2
    }

    public int IdUsuario { get; set; }
    [Display(Name = "Nombre Usuario")]
    public string? NombreUsuario { get; set; }
    [Display(Name = "Contraseña")]
    public string? Contraseña { get; set; }
    [Display(Name = "Rol")]
    public RolUsuario? Rol { get; set; } = null!;
    [Display(Name = "Correo")]
    public string? Correo { get; set; }
    [Display(Name = "Estado")]
    public EstadoUsuario? Estado { get; set; } = null!;

    public int? IdAsociacion { get; set; }
    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbAsociado> TbAsociados { get; } = new List<TbAsociado>();
}
