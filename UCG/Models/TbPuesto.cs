using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbPuesto
{
    public int IdPuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Decripcion { get; set; }

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();
}
