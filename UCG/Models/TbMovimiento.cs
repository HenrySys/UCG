using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbMovimiento
{
    public enum FuentesDeFondo
    {
        [Display(Name = "Fondos Propios")]
        FondosPropios = 1,

        [Display(Name = "Aporte Dinadeco")]
        AporteDinadeco = 2
    }

    public enum MetodosDePago
    {
        [Display(Name = "Transferencia")]
        Transferencia = 1,

        [Display(Name = "SinpeMovil")]
        SinpeMovil = 2,

        [Display(Name = "Cheque")]
        Cheque = 3,

        [Display(Name = "Efectivo")]
        Efectivo = 4

    }
    public enum TipoDeMovimiento
    {
        [Display(Name = "Ingreso")]
        Ingresos = 1,

        [Display(Name = "Egreso")]
        Egresos = 2
    }

    public enum EstadoDeMovimiento
    {
        [Display(Name = "Procesado")]
        Procesado = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,

        [Display(Name = "En Proceso")]
        EnProceso = 3
    }

    public int IdMovimiento { get; set; }

    public int IdAsociacion { get; set; }

    public int? IdAsociado { get; set; }

    public TipoDeMovimiento? TipoMovimiento { get; set; }

    public int? IdCategoriaMovimiento { get; set; }

    public FuentesDeFondo? FuenteFondo { get; set; }

    public int? IdProyecto { get; set; }

    public int? IdCuenta { get; set; }

    public int? IdActa { get; set; }

    public int? IdProveedor { get; set; }

    public int? IdCliente { get; set; }

    public string Descripcion { get; set; } = null!;

    public MetodosDePago? MetdodoPago { get; set; }

    public DateOnly FechaMovimiento { get; set; }

    public decimal? SubtotalMovido { get; set; }

    public decimal? MontoTotalMovido { get; set; }

    public EstadoDeMovimiento? Estado { get; set; }

    public int? IdConcepto { get; set; }

    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual TbAsociacion IdAsociacionNavigation { get; set; } = null!;

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual TbCategoriaMovimiento? IdCategoriaMovimientoNavigation { get; set; }

    public virtual TbConceptoMovimiento? IdConceptoNavigation { get; set; }

    public virtual TbCuentum? IdCuentaNavigation { get; set; }

    public virtual TbProveedor? IdProveedorNavigation { get; set; }

    public virtual TbProyecto? IdProyectoNavigation { get; set; }

    public virtual ICollection<TbDetalleMovimiento> TbDetalleMovimientos { get; } = new List<TbDetalleMovimiento>();
}
