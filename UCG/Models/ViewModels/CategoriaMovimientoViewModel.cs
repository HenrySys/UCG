using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbCategoriaMovimiento;

namespace UCG.Models.ViewModels
{
    public class CategoriaMovimientoViewModel
    {
        public int IdCategoriaMovimiento { get; set; }

        [Display(Name = "Concepto Asociacion")]
        public int IdConceptoAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int IdAsociado { get; set; }

        [Display(Name = "Nombre")]
        public string? NombreCategoria { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }        
    }
}
