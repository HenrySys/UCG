using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbActum
{
    public enum EstadoDeActa
    {
        [Display(Name = "Cerrado")]
        Cerrado = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,

        [Display(Name = "En Proceso")]
        EnProceso = 3,
        
    }
    public int IdActa { get; set; }

    public int? IdAsociacion { get; set; }

    [Display(Name = "Fecha de Sesion")]
    public DateOnly FechaSesion { get; set; }

    [Display(Name = "Fecha de Acta")]
    public string? NumeroActa { get; set; }

    public string? Descripcion { get; set; }

    public EstadoDeActa? Estado { get; set; }

    [Display(Name = "Monto Total Acordado")]
    public decimal? MontoTotalAcordado { get; set; }

    public int? IdAsociado { get; set; }

    [Display(Name = "Cod Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    [Display(Name = "Ced Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual ICollection<TbAcuerdo> TbAcuerdos { get; } = new List<TbAcuerdo>();

    public virtual ICollection<TbJuntaDirectiva> TbJuntaDirectivas { get; } = new List<TbJuntaDirectiva>();

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();

    public virtual ICollection<TbProyecto> TbProyectos { get; } = new List<TbProyecto>();

    public virtual ICollection<TbActaAsistencium> TbActaAsistencias { get; } = new List<TbActaAsistencium>();


}
