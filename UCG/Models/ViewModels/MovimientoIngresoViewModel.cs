using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class MovimientoIngresoViewModel
    {
        public int IdMovimientoIngreso { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }


        [Display(Name = "Fecha Ingreso")]
        public string? FechaTextoIngreso { get; set; }
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Display(Name = "Monto Total Ingresado")]
        public decimal? MontoTotalIngresado { get; set; }


        [Display(Name = "Fecha Ingreso")]
        public DateOnly? FechaIngreso { get; set; }


        [Display(Name = "Concepto Asociación")]
        public int IdConceptoAsociacion { get; set; }

        [Display(Name = "Donante")]
        public int? IdDonante { get; set; }

        [Display(Name = "Actividad")]
        public int? IdActividad { get; set; }

        [Display(Name = "Financista")]
        public int? IdFinancista { get; set; }

        public int? TipoOrigenIngreso { get; set; } // Esto será útil para enviar el tipo y usarlo en JS si querés


        public string DetalleDocumentoIngresoJson { get; set; } = null!;
        [Display(Name = "Asociacion")]
        public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
        [Display(Name = "Asociado")]
        public virtual TbAsociado? IdAsociadoNavigation { get; set; }

        public List<DocumentoIngresoViewModel> DetalleDocumentoIngreso { get; set; } = new();
    }
}
