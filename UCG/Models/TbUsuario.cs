using System;
using System.Collections.Generic;

namespace UCG.Models;

public enum EstadoUsuario
{
    Activo,
    Inactivo
}
public enum RolUsuario
{
    Admin,
    root
}

public partial class TbUsuario
{
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public string? Contraseña { get; set; }

    public RolUsuario Rol { get; set; } 

    public string? Correo { get; set; }

    public  EstadoUsuario Estado { get; set; }

    public virtual ICollection<TbAsociado> TbAsociados { get; } = new List<TbAsociado>();
}
