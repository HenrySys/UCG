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
        Egreso = 2,

    }

    public enum TipoDeOrigenIngreso
    {
        [Display(Name = "donante")]
        donante = 1,

        [Display(Name = "actividad")]
        actividad = 2,

        [Display(Name= "financista")]
        financista = 3,
    }

    public enum TipoDeOrigenEgreso
    {
        [Display(Name = "proveedor")]
        proveedor = 1,

        [Display(Name = "colaborador")]
        colaborador = 2,

        [Display(Name= "asociado")]
        asociado = 3,

    }

    public int IdConceptoMovimiento { get; set; }
    [Display(Name= "Tipo Movimiento")]
    public TiposDeConceptoMovimientos? TipoMovimiento { get; set; }
    [Display(Name= "Concepto")]
    public string? Concepto { get; set; }
    [Display(Name= "Origen Ingreso")]
    public TipoDeOrigenIngreso? TipoOrigenIngreso { get; set; }
    [Display(Name= "Emisor Egreso")]
    public TipoDeOrigenEgreso? TipoEmisorEgreso { get; set; }

    public virtual ICollection<TbConceptoAsociacion> TbConceptoAsociacions { get; } = new List<TbConceptoAsociacion>();
}
