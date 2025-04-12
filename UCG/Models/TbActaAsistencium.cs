using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbActaAsistencium
{
    public int IdActaAsistencia { get; set; }

    [Display(Name = "Asociado")]
    public int IdAsociado { get; set; }

    [Display(Name = "Acta")]
    public int IdActa { get; set; }

    public DateOnly Fecha { get; set; }

    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }

    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
}
