using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbJuntaDirectiva;

namespace UCG.Models.ViewModels
{
    public class JuntaDirectivaViewModel
    {
        public int IdJuntaDirectiva { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Periodo de Inicio")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido.")]
        public DateOnly PeriodoInicio { get; set; }

        [Display(Name = "Periodo de Fin")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido.")]
        public DateOnly PeriodoFin { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeJuntaDirectiva? Estado { get; set; }

    }
}
