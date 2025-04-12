using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbAsociacion
{
    public enum EstadoDeAsociacion
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,

    }

    public int IdAsociacion { get; set; }

    [Display(Name = "Cédula Jurídica")]
    public string CedulaJuridica { get; set; } = null!;

    [Display(Name = "Codigo Registro")]
    public string CodigoRegistro { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    [Display(Name = "Fecha Constitucion")]
    public DateOnly FechaConstitucion { get; set; }

    public string Telefono { get; set; } = null!;

    public string? Fax { get; set; }


    public string Correo { get; set; } = null!;

    public string Provincia { get; set; } = null!;

    public string Canton { get; set; } = null!;

    public string Distrito { get; set; } = null!;

    public string Pueblo { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public EstadoDeAsociacion? Estado { get; set; }

    public virtual ICollection<TbActum> TbActa { get; } = new List<TbActum>();

    public virtual ICollection<TbAsociado> TbAsociados { get; } = new List<TbAsociado>();

    public virtual ICollection<TbCliente> TbClientes { get; } = new List<TbCliente>();

    public virtual ICollection<TbConceptoAsociacion> TbConceptoAsociacions { get; } = new List<TbConceptoAsociacion>();

    public virtual ICollection<TbCuentum> TbCuenta { get; } = new List<TbCuentum>();

    public virtual TbJuntaDirectiva? TbJuntaDirectiva { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();

    public virtual ICollection<TbProveedor> TbProveedors { get; } = new List<TbProveedor>();

    public virtual ICollection<TbProyecto> TbProyectos { get; } = new List<TbProyecto>();
}
