using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbConceptoAsociacion;
namespace UCG.Models.ViewModels
{
    public class ConceptoAsociacionViewModel
    {
        [Display(Name = "ID Concepto Asociacion")]
        public int IdConceptoAsociacion { get; set; }

        [Display(Name = "ID Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "ID Concepto")]
        public int? IdConcepto { get; set; }
    }
}
