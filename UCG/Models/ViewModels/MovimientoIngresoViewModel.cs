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

        public string? Descripcion { get; set; }

        [Display(Name = "Monto Total Ingresado")]
        public decimal? MontoTotalIngresado { get; set; }


        public string DocumentoIngresoJason { get; set; } = null!;

        public List<DocumentoIngresoViewModel> DocumentoIngreso { get; set; } = new();
    }
}
