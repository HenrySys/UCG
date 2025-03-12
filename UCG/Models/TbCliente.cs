using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbCliente
{
    public int IdCliente { get; set; }

    public int? IdAsociacion { get; set; }

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Correo { get; set; }

    public string Direccion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
}
