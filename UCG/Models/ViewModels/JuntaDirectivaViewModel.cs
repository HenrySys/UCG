using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbJuntaDirectiva;

namespace UCG.Models.ViewModels
{
    public class JuntaDirectivaViewModel
    {
        [Display(Name = "Periodo de Inicio")]
        public string? FechaPeriodoInicioTexto { get; set; } 
        [Display(Name = "Periodo de Fin")]
        public string? FechaPeriodoFinTexto { get; set; }

        public int IdJuntaDirectiva { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

		[BindNever]
		public DateOnly PeriodoInicio { get; set; }
		[BindNever]
		public DateOnly PeriodoFin { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeJuntaDirectiva? Estado { get; set; }

        public string MiembrosJuntaJson { get; set; } = null!;

        public List<MiembroJuntaDirectivaViewModel> MiembroJunta { get; set; } = new();


    }
}
