using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbPuesto
{
    public int IdPuesto { get; set; }

    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Descripcion")]
    public string? Decripcion { get; set; }

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();
}
