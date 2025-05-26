using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbMovimientoEgreso
{
    public int IdMovimientoEgreso { get; set; }

    public int IdAsociacion { get; set; }

    public int? IdAsociado { get; set; }

    public int? IdActa { get; set; }
    [Display(Name = "Monto")]
    public decimal Monto { get; set; }
    [Display(Name = "Fecha")]
    public DateOnly Fecha { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }
    [Display(Name = "Asociacion")]
    public virtual TbAsociacion IdAsociacionNavigation { get; set; } = null!;
    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual ICollection<TbDetalleChequeFactura> TbDetalleChequeFacturas { get; } = new List<TbDetalleChequeFactura>();
}
