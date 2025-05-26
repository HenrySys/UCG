using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbFolio;

namespace UCG.Models.ViewModels
{
    public class FolioViewModel
    {

        public int IdFolio { get; set; }

        [Display(Name = "Asociacion")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [Display(Name = "Fecha Emision")]
        public string? FechaTextoEmision { get; set; }

        [BindNever]
        public DateOnly FechaEmision { get; set; }

        [Display(Name = "Fecha Cierre")]
        public string? FechaTextoCierre { get; set; }

        [BindNever]
        public DateOnly FechaCierre { get; set; }

        [Display(Name = "Número de Folio")]
        public string? NumeroFolio { get; set; }
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }
        [Display(Name = "Estado")]
        public EstadoDeFolio? Estado { get; set; }


    }
}
