using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbConceptoMovimiento
{
    public enum TiposDeConceptoMovimientos
    {
        [Display(Name = "Ingreso")]
        Ingreso = 1,

        [Display(Name = "Egreso")]
        Egreso = 2
    }

    public int IdConceptoMovimiento { get; set; }

    [Display(Name = "Tipo Movimiento")]
    public TiposDeConceptoMovimientos? TipoMovimiento { get; set; }

    public string? Concepto { get; set; }

    public virtual ICollection<TbConceptoAsociacion> TbConceptoAsociacions { get; } = new List<TbConceptoAsociacion>();

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
