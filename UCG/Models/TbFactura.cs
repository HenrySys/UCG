using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbFactura
{
    public enum EstadoDeFactura
    {
        [Display(Name = "Pendiente")]
        Pendiente = 1,

        [Display(Name = "Pagada")]
        Pagada = 2,

        [Display(Name = "Rechazada")]
        Rechazada = 3
    }
   public int IdFactura { get; set; }
    [Display(Name = "Numero Factura")]
    public string NumeroFactura { get; set; } = null!;
    [Display(Name = "Fecha Emision")]
    public DateOnly FechaEmision { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Monto Total")]
    public decimal MontoTotal { get; set; }

    [Display(Name = "Colaborador")]
    public int? IdColaborador { get; set; }
    [Display(Name = "Proveedor")]
    public int? IdProveedor { get; set; }
    [Display(Name = "Asociacion")]
    public int? IdAsociacion { get; set; }
    [Display(Name = "Asociado")]

    public int? IdAsociado { get; set; }
    [Display(Name = "RutaURL")]
    public string? ArchivoUrl { get; set; }
    [Display(Name = "Nombre Archivo")]
    public string? NombreArchivo { get; set; }
    [Display(Name = "Fecha Subida")]
    public DateTime? FechaSubida { get; set; }

    [Display(Name = "Concepto Asociacion")]
    public int? IdConceptoAsociacion { get; set; }

    public EstadoDeFactura Estado { get; set; }

    public decimal? Subtotal { get; set; }

    [Display(Name = "Total IVA")]

    public decimal? TotalIva { get; set; }

    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    [Display(Name = "Colaborador")]
    public virtual TbColaborador? IdColaboradorNavigation { get; set; }

    [Display(Name = "Concepto Asociacion")]
    public virtual TbConceptoAsociacion? IdConceptoAsociacionNavigation { get; set; }

    [Display(Name = "Proveedor")]
    public virtual TbProveedor? IdProveedorNavigation { get; set; }

    public virtual ICollection<TbDetalleChequeFactura> TbDetalleChequeFacturas { get; } = new List<TbDetalleChequeFactura>();

    public virtual ICollection<TbDetalleFactura> TbDetalleFacturas { get; } = new List<TbDetalleFactura>();
}
