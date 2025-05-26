using static UCG.Models.TbCheque;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UCG.Models.ViewModels
{
    public class ChequeViewModel
    {
        public int IdCheque { get; set; }

        [Display(Name = "Asociacion")]
        public int IdAsociacion { get; set; }

        [Display(Name = "Cuenta")]
        public int IdCuenta { get; set; }
        [Display(Name = "Asociado")]
        public int? IdAsociadoAutoriza { get; set; }

        [Display(Name = "Numero Cheque")]
        public string NumeroCheque { get; set; } = null!;

        [Display(Name = "Fecha Emision")]
        public string? FechaTextoEmision { get; set; }

        [Display(Name = "Fecha Pago")]
        public string? FechaTextoPago { get; set; }

        [Display(Name = "Fecha Cobro")]
        public string? FechaTextoCobro { get; set; }

        [Display(Name = "Fecha Anulacion")]
        public string? FechaTextoAnulacion { get; set; }

        [Display(Name = "Beneficiario")]
        public string Beneficiario { get; set; } = null!;

        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Display(Name = "Estado")]
        public EstadoCheque? Estado { get; set; } = null!;
        
        [Display(Name = "Observacion")]
        public string? Observaciones { get; set; }
        
        [Display(Name = "MontoRestante")]
        public decimal? MontoRestante { get; set; }

        [BindNever]
        public DateOnly? FechaAnulacion { get; set; }
        [BindNever]
        public DateOnly? FechaCobro { get; set; }
        [BindNever]
        public DateOnly FechaPago { get; set; }
        [BindNever]
        public DateOnly FechaEmision { get; set; }

    }
}
