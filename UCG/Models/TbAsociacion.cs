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
    [Display(Name = "Cedula Juridica")]
    public string CedulaJuridica { get; set; } = null!;
    [Display(Name = "Codigo Registro")]
    public string CodigoRegistro { get; set; } = null!;
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    [Display(Name = "Fecha Constitucion")]
    public DateOnly FechaConstitucion { get; set; }
    [Display(Name = "Telefono")]
    public string Telefono { get; set; } = null!;
    [Display(Name = "Fax")]
    public string? Fax { get; set; }
    [Display(Name = "Correo")]
    public string Correo { get; set; } = null!;
    [Display(Name = "Provincia")]
    public string Provincia { get; set; } = null!;
    [Display(Name = "Canton")]
    public string Canton { get; set; } = null!;
    [Display(Name = "Distrito")]
    public string Distrito { get; set; } = null!;
    [Display(Name = "Pueblo")]
    public string Pueblo { get; set; } = null!;
    [Display(Name = "Direccion")]
    public string Direccion { get; set; } = null!;
    [Display(Name = "Descripcion")]
    public string Descripcion { get; set; } = null!;
    [Display(Name = "Estado")]
    public EstadoDeAsociacion? Estado { get; set; } = null!;

    public virtual ICollection<TbActum> TbActa { get; } = new List<TbActum>();

    public virtual ICollection<TbActividad> TbActividads { get; } = new List<TbActividad>();

    public virtual ICollection<TbAsociado> TbAsociados { get; } = new List<TbAsociado>();

    public virtual ICollection<TbCheque> TbCheques { get; } = new List<TbCheque>();

    public virtual ICollection<TbCliente> TbClientes { get; } = new List<TbCliente>();

    public virtual ICollection<TbConceptoAsociacion> TbConceptoAsociacions { get; } = new List<TbConceptoAsociacion>();

    public virtual ICollection<TbCuentum> TbCuenta { get; } = new List<TbCuentum>();

    public virtual ICollection<TbFactura> TbFacturas { get; } = new List<TbFactura>();

    public virtual ICollection<TbFinancistum> TbFinancista { get; } = new List<TbFinancistum>();

    public virtual ICollection<TbFolio> TbFolios { get; } = new List<TbFolio>();

    public virtual TbJuntaDirectiva? TbJuntaDirectiva { get; set; }

    public virtual ICollection<TbMovimientoEgreso> TbMovimientoEgresos { get; } = new List<TbMovimientoEgreso>();

    public virtual ICollection<TbMovimientoIngreso> TbMovimientoIngresos { get; } = new List<TbMovimientoIngreso>();

    public virtual ICollection<TbProveedor> TbProveedors { get; } = new List<TbProveedor>();

    public virtual ICollection<TbUsuario> TbUsuarios { get; } = new List<TbUsuario>();

    public virtual ICollection<TbColaborador> TbColaboradors { get; } = new List<TbColaborador>();
}
