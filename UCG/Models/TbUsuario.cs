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

    [Display(Name = "Nombre")]
    public string? NombreUsuario { get; set; }

    public string? Contraseña { get; set; }

    public RolUsuario? Rol { get; set; } = null!;

    public string? Correo { get; set; }

    public EstadoUsuario? Estado { get; set; } = null!;

    public virtual ICollection<TbAsociado> TbAsociados { get; } = new List<TbAsociado>();
}
