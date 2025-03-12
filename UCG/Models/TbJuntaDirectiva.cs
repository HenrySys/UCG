using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbJuntaDirectiva
{
    public int IdJuntaDirectiva { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdActa { get; set; }

    public DateOnly? PeriodoInicio { get; set; }

    public DateOnly? PeriodoFin { get; set; }

    public string? Nombre { get; set; }

    public string Estado { get; set; } = null!;

    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();
}
