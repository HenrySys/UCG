using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbConceptoAsociacion
{
    public int IdConceptoAsociacion { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdConcepto { get; set; }

    [Display(Name= "Descripcion")]
    public string? DescripcionPersonalizada { get; set; }

    [Display(Name= "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    [Display(Name= "Concepto Movimiento")]
    public virtual ICollection<TbDocumentoIngreso> TbDocumentoIngresos { get; } = new List<TbDocumentoIngreso>();

    public virtual ICollection<TbFactura> TbFacturas { get; } = new List<TbFactura>();
    public virtual TbConceptoMovimiento IdConceptoNavigation { get; internal set; } = null!;
}
