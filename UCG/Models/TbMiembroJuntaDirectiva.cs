using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbMiembroJuntaDirectiva
{
    public enum EstadoDeMiembroJD
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2
    }
    public int IdMiembrosJuntaDirectiva { get; set; }

    public int? IdJuntaDirectiva { get; set; }

    public int IdAsociado { get; set; } 

    public int IdPuesto { get; set; }
    [Display(Name = "Estado")]
    public EstadoDeMiembroJD? Estado { get; set; } = null!;
    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
    [Display(Name = "JuntaDirectiva")]
    public virtual TbJuntaDirectiva? IdJuntaDirectivaNavigation { get; set; }
    [Display(Name = "Puesto")]
    public virtual TbPuesto? IdPuestoNavigation { get; set; }
}
