using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbProveedor;

namespace UCG.Models.ViewModels
{
    public class AcuerdoViewModel{
        public int IdAcuerdo { get; set; }
        
        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Numero Acuerdo")]
        public string? NumeroAcuerdo { get; set; }

        [Display(Name = "Nombre Acuerdo")]
        public string? Nombre { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Display(Name = "Monto del Acuerdo")] 
        public decimal? MontoAcuerdo { get; set; }
    }
}
