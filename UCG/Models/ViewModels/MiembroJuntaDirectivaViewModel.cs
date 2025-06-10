using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbMiembroJuntaDirectiva;

namespace UCG.Models.ViewModels
{
    public class MiembroJuntaDirectivaViewModel
    {
        public int IdMiembrosJuntaDirectiva { get; set; }

        [Display(Name = "Junta Directiva")]
        public int IdJuntaDirectiva { get; set; }

        [Display(Name = "Asociado")]
        public int IdAsociado { get; set; }

        [Display(Name = "Puesto")]
        public int IdPuesto { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeMiembroJD? Estado { get; set; }

        [BindNever]
        public string? Nombre { get; set; }
        [BindNever]
        public string? Puesto { get; set; }

    }
}
