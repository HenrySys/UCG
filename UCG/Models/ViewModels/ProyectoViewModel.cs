using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbProyecto;

namespace UCG.Models.ViewModels
{
    public class ProyectoViewModel{
        public int IdProyecto { get; set; }
        
        [Display(Name = "Asociación")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Display(Name = "Monto Total Destinado")]
        public decimal? MontoTotalDestinado { get; set; }

    }
}
