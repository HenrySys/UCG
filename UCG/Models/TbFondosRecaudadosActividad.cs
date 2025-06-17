using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbFondosRecaudadosActividad
{
    public int IdFondosRecaudadosActividad { get; set; }

    public int IdActividad { get; set; }

    public string? Detalle { get; set; }

    public decimal? Monto { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    public virtual TbActividad IdActividadNavigation { get; set; } = null!;
}
