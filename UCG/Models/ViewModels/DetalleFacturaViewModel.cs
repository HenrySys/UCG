using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class DetalleFacturaViewModel
    {
        [Display(Name = "ID Detalle Factura")]
        public int IdDetalleFactura { get; set; }

        [Display(Name = "Factura")]
        public int IdFactura { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = null!;

        [Display(Name = "Unidad")]
        public string? Unidad { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Porcentaje IVA (%)")]
        public decimal? PorcentajeIva { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal? PrecioUnitario { get; set; }

        [Display(Name = "Porcentaje Descuento (%)")]
        public decimal? PorcentajeDescuento { get; set; }

        [Display(Name = "Descuento")]
        public decimal? Descuento { get; set; }

        [Display(Name = "Monto IVA")]
        public decimal? MontoIva { get; set; }

        [Display(Name = "Base Imponible")]
        public decimal? BaseImponible { get; set; }

        [Display(Name = "Total Línea")]
        public decimal? TotalLinea { get; set; }
    }
}

