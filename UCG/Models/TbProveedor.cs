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

    [Display(Name = "Tipo")]
    public TipoDeProveedor? TipoProveedor { get; set; } = null!;

    [Display(Name = "Nombre Proveedor")]
    public string NombreEmpresa { get; set; } = null!;

    [Display(Name = "Cedula Juridica")]
    public string CedulaJuridica { get; set; } = null!;

    [Display(Name = "Nombre Contacto")]
    public string NombreContacto { get; set; } = null!;

    [Display(Name = "Cedula Contacto")]
    public string CedulaContacto { get; set; } = null!;

    [Display(Name = "Direccion")]
    public string Direccion { get; set; } = null!;

    [Display(Name = "Telefono")]
    public string Telefono { get; set; } = null!;

    [Display(Name = "Correo")]
    public string? Correo { get; set; }

    [Display(Name = "Fax")]
    public string? Fax { get; set; }

    public EstadoDeProveedor? Estado { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbFactura> TbFacturas { get; } = new List<TbFactura>();
}
