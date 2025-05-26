using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbDetalleChequeFactura
{
    public int IdDetalleChequeFactura { get; set; }

    [Display(Name = "Movimiento Egreso")]
    public int IdMovimientoEgreso { get; set; }
    [Display(Name = "Acuerdo")]
    public int? IdAcuerdo { get; set; }
    [Display(Name = "Cheque")]
    public int IdCheque { get; set; }
    [Display(Name = "Factura")]
    public int IdFactura { get; set; }
    [Display(Name = "Monto")]
    public decimal Monto { get; set; }
    [Display(Name = "Observacion")]
    public string? Observacion { get; set; }
    [Display(Name = "Acuerdo")]
    public virtual TbAcuerdo? IdAcuerdoNavigation { get; set; }
    [Display(Name = "Cheque")]
    public virtual TbCheque IdChequeNavigation { get; set; } = null!;
    [Display(Name = "Factura")]
    public virtual TbFactura IdFacturaNavigation { get; set; } = null!;

    public virtual TbMovimientoEgreso IdMovimientoEgresoNavigation { get; set; } = null!;
}
