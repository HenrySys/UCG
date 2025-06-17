using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbJuntaDirectiva
{
    public enum EstadoDeJuntaDirectiva
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2
    }
    public int IdJuntaDirectiva { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdActa { get; set; }

    [Display(Name = "Periodo Inicio")]
    public DateOnly? PeriodoInicio { get; set; }

    [Display(Name = "Periodo Fin")]
    public DateOnly? PeriodoFin { get; set; }

    [Display(Name = "Nombre")]
    public string? Nombre { get; set; }

    [Display(Name = "Estado")]
    public EstadoDeJuntaDirectiva? Estado { get; set; } = null!;
    
    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }

    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();
}
