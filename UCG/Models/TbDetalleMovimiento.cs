using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbDetalleMovimiento
{
    public int IdDetalleMovimiento { get; set; }

    public int IdMovimiento { get; set; }

    public int? IdAcuerdo { get; set; }

    public string? TipoMovimiento { get; set; }

    public string? Decripcion { get; set; }

    public decimal? Subtotal { get; set; }

    public int? IdInformeEconomico { get; set; }

    public virtual TbAcuerdo? IdAcuerdoNavigation { get; set; }

    public virtual TbMovimiento IdMovimientoNavigation { get; set; } = null!;
}
