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

    public DateOnly? PeriodoInicio { get; set; }

    public DateOnly? PeriodoFin { get; set; }

    public string? Nombre { get; set; }

    public EstadoDeJuntaDirectiva? Estado { get; set; }

    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();
}
