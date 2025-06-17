using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbCliente
{
     public enum EstadoDeCliente
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,

    }

    public int IdCliente { get; set; }

    public int? IdAsociacion { get; set; }
    [Display(Name = "Primer Apellido")]
    public string Apellido1 { get; set; } = null!;
    [Display(Name = "Segundo Apellido")]
    public string? Apellido2 { get; set; }
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    [Display(Name = "Cedula")]
    public string Cedula { get; set; } = null!;
    [Display(Name = "Telefono")]
    public string Telefono { get; set; } = null!;
    [Display(Name = "Correo")]
    public string? Correo { get; set; }
    [Display(Name = "Direccion")]
    public string Direccion { get; set; } = null!;
    [Display(Name = "Estado")]
    public EstadoDeCliente? Estado { get; set; } = null!;
    [Display(Name = "Tipo Cliente")]

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbDocumentoIngreso> TbDocumentoIngresos { get; } = new List<TbDocumentoIngreso>();
}
