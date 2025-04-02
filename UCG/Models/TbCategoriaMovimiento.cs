using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbCategoriaMovimiento
{
    public int IdCategoriaMovimiento { get; set; }

    public int? IdAsociado { get; set; }

    public string? NombreCategoria { get; set; }

    public string? DescripcionCategoria { get; set; }

    public int? IdConceptoAsociacion { get; set; }

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual TbConceptoAsociacion? IdConceptoAsociacionNavigation { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
