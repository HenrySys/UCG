using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbCheque
{
    public enum EstadoCheque
    {
        [Display(Name = "Emitido")]
        Emitido = 1,

        [Display(Name = "Usado Parcial")]
        UsadoParcial = 2,

        [Display(Name = "Usado Total")]
        UsadoTotal = 3,

        [Display(Name = "Anulado")]
        Anulado = 4,

        [Display(Name = "Vencido")]
        Vencido = 5

    }
    public int IdCheque { get; set; }
    public int IdAsociacion { get; set; }
    public int? IdAsociadoAutoriza { get; set; }
    public int IdCuenta { get; set; }

    [Display(Name = "Numero Cheque")]
    public string NumeroCheque { get; set; } = null!;

    [Display(Name = "Fecha Emision")]
    public DateOnly FechaEmision { get; set; }

    [Display(Name = "Fecha Pago")]
    public DateOnly FechaPago { get; set; }

    [Display(Name = "Beneficiario")]
    public string Beneficiario { get; set; } = null!;

    [Display(Name = "Monto")]
    public decimal Monto { get; set; }

    [Display(Name = "Estado")]
    public EstadoCheque? Estado { get; set; } = null!;

    [Display(Name = "Fecha Cobro")]
    public DateOnly? FechaCobro { get; set; }

    [Display(Name = "Fecha Anulacion")]
    public DateOnly? FechaAnulacion { get; set; }

    [Display(Name = "Observacion")]
    public string? Observaciones { get; set; }

    [Display(Name = "MontoRestante")]
    public decimal? MontoRestante { get; set; }

    [Display(Name = "Asociacion")]
    public virtual TbAsociacion IdAsociacionNavigation { get; set; } = null!;

    [Display(Name = "Asociado Autorizado")]
    public virtual TbAsociado? IdAsociadoAutorizaNavigation { get; set; }

    [Display(Name = "Cuenta")]
    public virtual TbCuentum IdCuentaNavigation { get; set; } = null!;

    public virtual ICollection<TbDetalleChequeFactura> TbDetalleChequeFacturas { get; } = new List<TbDetalleChequeFactura>();
}
