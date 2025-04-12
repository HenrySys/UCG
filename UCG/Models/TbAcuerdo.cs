using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbAcuerdo
{
    public int IdAcuerdo { get; set; }

    public int? IdActa { get; set; }

    [Display(Name = "Numero Acuerdo")]
    public string? NumeroAcuerdo { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    [Display(Name = "Monto del Acuerdo")]
    public decimal? MontoAcuerdo { get; set; }

    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual ICollection<TbDetalleMovimiento> TbDetalleMovimientos { get; } = new List<TbDetalleMovimiento>();
}
