using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbActividad
{
    public int IdActividad { get; set; }

    [Display(Name = "Nombre")]
    public string? Nombre { get; set; }

    public DateOnly? Fecha { get; set; }
    [Display(Name = "Razon")]
    public string? Razon { get; set; }

    [Display(Name = "Asociacion")]
    public int? IdAsociacion { get; set; }

    [Display(Name = "Asociado")]
    public int? IdAsociado { get; set; }

    [Display(Name = "Acta")]
    public int? IdActa { get; set; }
    [Display(Name = "Lugar")]
    public string? Lugar { get; set; }
    [Display(Name = "Observacion")]
    public string? Observaciones { get; set; }

    [Display(Name = "Monto total recaudado")]
    public decimal? MontoTotalRecuadado { get; set; }

    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }

    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual ICollection<TbDocumentoIngreso> TbDocumentoIngresos { get; } = new List<TbDocumentoIngreso>();

    public virtual ICollection<TbFondosRecaudadosActividad> TbFondosRecaudadosActividads { get; } = new List<TbFondosRecaudadosActividad>();
}
