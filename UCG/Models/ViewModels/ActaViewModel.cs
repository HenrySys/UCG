
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

        [Display(Name = "Fecha Sesion")]
        public string? FechaSesionTexto { get; set; }
        [Display(Name = "Folio")]
        public int? IdFolio { get; set; }

        [Display(Name = "Tipo Acta")]
        public TipoDeActa? Tipo { get; set; }

        [Display(Name = "Numero de acta")]
        public string? NumeroActa { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }
        [Display(Name = "Estado")]
        public EstadoDeActa? Estado { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [BindNever]
        public DateOnly FechaSesion { get; set; } 

        public string ActaAsistenciaJason { get; set; } = null!;

        public string ActaAcuerdoJason { get; set; } = null!;

        public List<AcuerdoViewModel> ActaAcuerdo { get; set; } = new();

        public List<ActaAsistenciaViewModel> ActaAsistencia { get; set; } = new ();
   

    }
}
