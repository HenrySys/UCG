using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbMovimientoIngreso
{
    public int IdMovimientoIngreso { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdAsociado { get; set; }
    [Display(Name = "Fecha Ingreso")]
    public DateOnly? FechaIngreso { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Monto Total")]
    public decimal? MontoTotalIngresado { get; set; }
    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
    [Display(Name = "Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual ICollection<TbDocumentoIngreso> TbDocumentoIngresos { get; } = new List<TbDocumentoIngreso>();
}
