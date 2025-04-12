using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbCategoriaMovimiento
{
    public int IdCategoriaMovimiento { get; set; }

    public int? IdAsociado { get; set; }

    [Display(Name = "Categoria")]
    public string? NombreCategoria { get; set; }

    [Display(Name = "Descripcion")]
    public string? DescripcionCategoria { get; set; }

    public int? IdConceptoAsociacion { get; set; }

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual TbConceptoAsociacion? IdConceptoAsociacionNavigation { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
