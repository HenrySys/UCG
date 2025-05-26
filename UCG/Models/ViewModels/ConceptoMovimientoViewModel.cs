using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbConceptoMovimiento;

namespace UCG.Models.ViewModels
{
    public class ConceptoMovimientoViewModel
    {
        public int IdConceptoMovimiento { get; set; }

        
        [Display(Name= "Tipo Movimiento")]
        public TiposDeConceptoMovimientos? TipoMovimiento { get; set; }
        [Display(Name= "Concepto Movimiento")]
        public string? Concepto { get; set; }
        [Display(Name= "Origen Ingreso")]
        public TipoDeOrigenIngreso? TipoOrigenIngreso { get; set; }
        [Display(Name= "Emisor Egreso")]
        public TipoDeOrigenEgreso? TipoEmisorEgreso { get; set; }



    }
}
