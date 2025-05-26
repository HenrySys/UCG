using static UCG.Models.TbFinancistum;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class FinancistaViewModel
    {
        public int IdFinancista { get; set; }

        [Display(Name = "Asociacion")]
        public int IdAsociacion { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Tipo Entidad")]
        public TipoEntidades? TipoEntidad { get; set; } = null!;

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Sitio Web")]
        public string? SitioWeb { get; set; }

    }
}
