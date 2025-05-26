using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbConceptoAsociacion;
namespace UCG.Models.ViewModels
{
    public class ConceptoAsociacionViewModel
    {
        [Display(Name = "Concepto Asociacion")]
        public int IdConceptoAsociacion { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Concepto")]
        public int? IdConcepto { get; set; }

        [Display(Name= "Descripcion")]
        public string? DescripcionPersonalizada { get; set; }
    }
}
