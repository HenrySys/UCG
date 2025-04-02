using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbActaAsistencium
{
    public int IdActaAsistencia { get; set; }

    public int? IdAsociado { get; set; }

    public int? IdActa { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual TbAsociacion? IdActaNavigation { get; set; }

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
}
