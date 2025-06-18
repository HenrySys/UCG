using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class FondosRecaudadosActividadViewModel
    {
        public int IdFondosRecaudadosActividad { get; set; }

        [Display(Name = "Actividad")]
        public int IdActividad { get; set; }
        
        [Display(Name = "Detalle")]
        public string? Detalle { get; set; }

        [Display(Name = "Monto total recaudado")]
        public decimal? Monto { get; set; }

        [Display(Name = "Fecha de Registro")]
        public string? FechaTextoRegistro { get; set; }

        [BindNever]
        public DateOnly? FechaRegistro { get; set; }
    }
}
