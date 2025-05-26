using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UCG.Models;

public partial class TbActaAsistencium
{
    public int IdActaAsistencia { get; set; }


    public int IdAsociado { get; set; }
    [Display(Name = "Fecha")]
    public DateOnly Fecha { get; set; }

    public int? IdActa { get; set; }
    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }
    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
}
