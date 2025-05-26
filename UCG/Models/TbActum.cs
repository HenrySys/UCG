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
        EnProceso = 0,
        
    }

    public enum TipoDeActa
    {
        [Display(Name = "Ordinario")]
        Ordinario = 1,

        [Display(Name = "Extraordinaria")]
        Extraordinaria = 2,    
    }
    public int IdActa { get; set; }

    public int? IdAsociacion { get; set; }
    [Display(Name = "Fecha de Sesion")]
    public DateOnly FechaSesion { get; set; }
    [Display(Name = "Numero Acta")]
    public string? NumeroActa { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Estado")]
    public EstadoDeActa? Estado { get; set; }

    public int? IdAsociado { get; set; }
    [Display(Name = "Tipo Acta")]
    public TipoDeActa? Tipo { get; set; }

    public int? IdFolio { get; set; }

    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
    [Display(Name = "Folio")]
    public virtual TbFolio? IdFolioNavigation { get; set; }

    public virtual ICollection<TbActaAsistencium> TbActaAsistencias { get; } = new List<TbActaAsistencium>();

    public virtual ICollection<TbActividad> TbActividads { get; } = new List<TbActividad>();

    public virtual ICollection<TbAcuerdo> TbAcuerdos { get; } = new List<TbAcuerdo>();

    public virtual ICollection<TbJuntaDirectiva> TbJuntaDirectivas { get; } = new List<TbJuntaDirectiva>();

    public virtual ICollection<TbMovimientoEgreso> TbMovimientoEgresos { get; } = new List<TbMovimientoEgreso>();
}
