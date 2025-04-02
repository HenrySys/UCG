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
        Inactivo = 2
    }

    public enum TipoDeCliente{
        [Display(Name = "Donante")]
        Donante = 1,
        [Display(Name = "Colaborador")]
        Colaborador = 2,
        [Display(Name = "Cliente")]
        Cliente = 3
    }

    public int IdCliente { get; set; }

    public int? IdAsociacion { get; set; }

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Correo { get; set; }

    public string Direccion { get; set; } = null!;

    public EstadoDeCliente? Estado { get; set; } = null!;

    public string? TipoCliente { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
}
