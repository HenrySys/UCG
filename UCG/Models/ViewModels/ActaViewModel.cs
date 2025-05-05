
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static UCG.Models.TbActum;
namespace UCG.Models.ViewModels
{
    public class ActaViewModel
    {
        public int IdActa { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        public string? FechaSesionTexto { get; set; } // Recibe el texto del input (yyyy-MM-dd)


        [Display(Name = "Numero de acta")]
        public string? NumeroActa { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }
        [Display(Name = "Estado")]
        public EstadoDeActa? Estado { get; set; }
        [Display(Name = "Monto total acordado")]
        public decimal? MontoTotalAcordado { get; set; }
        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [BindNever]
        public DateOnly FechaSesion { get; set; } // Aquí guardas el DateOnly real

        public string ActaAsistenciaJason { get; set; } = null!;

        public string ActaAcuerdoJason { get; set; } = null!;

        public List<AcuerdoViewModel> ActaAcuerdo { get; set; } = new();

        public List<ActaAsistenciaViewModel> ActaAsistencia { get; set; } = new ();
   

    }
}
