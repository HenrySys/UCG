using System;
using System.Collections.Generic;

namespace UCG.Models;

public enum TiposDeConceptoMovimientos
{
    Ingreso,
    Egreso
}

public partial class TbConceptoMovimiento
{
    public int IdConceptoMovimiento { get; set; }

    public int? IdAsociacion { get; set; }

    public TiposDeConceptoMovimientos TipoMovimiento { get; set; }

    public string? Concepto { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbCategoriaMovimiento> TbCategoriaMovimientos { get; } = new List<TbCategoriaMovimiento>();

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
