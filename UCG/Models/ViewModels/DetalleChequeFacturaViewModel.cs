using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class DetalleChequeFacturaViewModel
    {
        public int IdDetalleChequeFactura { get; set; }

        [Display(Name = "Movimiento Egreso")]
        public int IdMovimientoEgreso { get; set;}

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
        
    }
}
