using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class ActividadViewModel
    {
        public int IdActividad { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Fecha Actividad")]
        public string? FechaTextoActividad { get; set; }
        
        [Display(Name = "Razon")]
        public string? Razon { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Lugar")]
        public string? Lugar { get; set; }

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
        [BindNever]
        public DateOnly? Fecha { get; set; }

        [Display(Name = "Monto total recaudado")]
        public decimal? MontoTotalRecuadado { get; set; }

    }
}
