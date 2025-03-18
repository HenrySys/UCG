using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class ConceptoMovimientoViewModel
    {
        public int IdConceptoMovimiento { get; set; }

        [Display(Name = "Asociacion")]
        [Required]
        public int IdAsociacion { get; set; }

        [Display(Name = "Tipo de movimiento")]
        [Required]
        public TiposDeConceptoMovimientos TipoMovimiento { get; set; }

        [Display(Name = "Concepto de movimiento")]
        [Required]
        public string? Concepto { get; set; }



    }
}
