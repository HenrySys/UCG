using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbConceptoAsociacion
{
    public int IdConceptoAsociacion { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdConcepto { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual TbConceptoMovimiento? IdConceptoNavigation { get; set; }

    public virtual ICollection<TbCategoriaMovimiento> TbCategoriaMovimientos { get; } = new List<TbCategoriaMovimiento>();
}
