using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbDetalleMovimiento;

namespace UCG.Models.ViewModels
{
    public class DetalleMovimientoViewModel
    {
        public int IdDetalleMovimiento { get; set; }

        [Display(Name = "ID Movimiento")]
        public int IdMovimiento { get; set; }

        [Display(Name = "ID Acuerdo")]
        public int? IdAcuerdo { get; set; }

        [Display(Name = "Descripcion")]
        public string? Decripcion { get; set; }

        [Display(Name = "Subtotal")]
        public decimal? Subtotal { get; set; }

    }
}
