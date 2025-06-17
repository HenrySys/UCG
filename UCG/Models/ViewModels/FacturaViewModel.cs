using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbFactura;

namespace UCG.Models.ViewModels
{
    public class FacturaViewModel
    {
        public int IdFactura { get; set; }

        
        [Display(Name = "Numero Factura")]
        public string NumeroFactura { get; set; } = null!;
        [Display(Name = "Fecha Emision")]
        public string? FechaTextoEmision { get; set; }
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Display(Name = "Concepto Asociacion")]
        public int? IdConceptoAsociacion { get; set; }

        [Display(Name = "Colaborador")]
        public int? IdColaborador { get; set; }

        [Display(Name = "Proveedor")]
        public int? IdProveedor { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [Display(Name = "Monto Total")]
        public decimal MontoTotal { get; set; }

        public EstadoDeFactura Estado { get; set; }

        [Display(Name = "RutaURL")]
        public string? ArchivoUrl { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string? NombreArchivo { get; set; }

        [Display(Name = "Fecha Subida")]
        public string? FechaTextoSubida { get; set; }

        [BindNever]
        public DateOnly FechaEmision { get; set; }
        [BindNever]
        public DateTime? FechaSubida { get; set; }

        public string? TipoEmisor { get; set; }

        public decimal? Subtotal { get; set; }

        [Display(Name = "Total IVA")]

        public decimal? TotalIva { get; set; }


        public string DetalleFacturaJason { get; set; } = null!;

        public List<DetalleFacturaViewModel > DetalleFactura { get; set; } = new();



    }
}
