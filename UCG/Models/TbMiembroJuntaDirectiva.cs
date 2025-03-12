using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbMiembroJuntaDirectiva
{
    public int IdMiembrosJuntaDirectiva { get; set; }

    public int? IdJuntaDirectiva { get; set; }

    public int? IdAsociado { get; set; }

    public int? IdPuesto { get; set; }

    public string Estado { get; set; } = null!;

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual TbJuntaDirectiva? IdJuntaDirectivaNavigation { get; set; }

    public virtual TbPuesto? IdPuestoNavigation { get; set; }
}
