using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class MovimientoEgresoViewModel
    {

        public int IdMovimientoEgreso { get; set; }

        [Display(Name = "Fecha Egreso")]
        public string? FechaTextoEgreso { get; set; }

        [Display(Name = "Asociacion")]
        public int IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        public string DetalleChequeFacturaEgresoJason { get; set; } = null!;

        public List<DetalleChequeFacturaViewModel> DetalleChequeFacturaEgreso { get; set; } = new();


    }
}
