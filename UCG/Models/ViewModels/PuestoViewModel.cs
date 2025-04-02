using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbPuesto;

namespace UCG.Models.ViewModels
{
    public class PuestoViewModel
    {
        public int IdPuesto { get; set; }

        [Display(Name = "Nombre de puesto")]
        [Required]
        public string? NombrePuesto { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

    }
}
