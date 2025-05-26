using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbDocumentoIngreso
{
    public enum MetodoDePago
    {
        [Display(Name = "Efectivo")]
        Efectivo = 1,

        [Display(Name = "Sinpe")]
        Sinpe = 2,

        [Display(Name = "Transferencia")]
        Transferencia = 3,

    }
    public int IdDocumentoIngreso { get; set; }

    public int IdMovimientoIngreso { get; set; }

    public int? IdConceptoAsociacion { get; set; }

    public int? IdCuenta { get; set; }

    public int? IdCliente { get; set; }

    public int? IdFinancista { get; set; }

    public int? IdActividad { get; set; }
    [Display(Name = "Numero Comprobante")]
    public string NumComprobante { get; set; } = null!;
    [Display(Name = "Fecha Comprobante")]
    public DateOnly? FechaComprobante { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }
    [Display(Name = "Metodo de Pago")]
    public MetodoDePago? MetodoPago { get; set; }

    public virtual TbActividad? IdActividadNavigation { get; set; }

    public virtual TbCliente? IdClienteNavigation { get; set; }

    public virtual TbConceptoAsociacion? IdConceptoAsociacionNavigation { get; set; }

    public virtual TbCuentum? IdCuentaNavigation { get; set; }

    public virtual TbFinancistum? IdFinancistaNavigation { get; set; }

    public virtual TbMovimientoIngreso IdMovimientoIngresoNavigation { get; set; } = null!;
}
