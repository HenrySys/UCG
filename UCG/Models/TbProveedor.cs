using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbProveedor
{
     public enum EstadoDeProveedor
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,
        
    }

    public enum TipoDeProveedor
    {
        [Display(Name = "Fisico")]
        Fisico = 1,

        [Display(Name = "Juridico")]
        Juridico = 2,

    }
    public int IdProveedor { get; set; }

    public int? IdAsociacion { get; set; }

    public TipoDeProveedor TipoProveedor { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public string CedulaJuridica { get; set; } = null!;

    public string NombreContacto { get; set; } = null!;

    public string CedulaContacto { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Correo { get; set; }

    public string? Fax { get; set; }

    public EstadoDeProveedor? Estado { get; set; } = null!;

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
