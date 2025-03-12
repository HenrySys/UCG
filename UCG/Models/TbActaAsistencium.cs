using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbActaAsistencium
{
    public int IdActaAsistencia { get; set; }

    public int? IdAsociado { get; set; }

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
}
