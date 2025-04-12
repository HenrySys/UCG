using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbProyecto
{
    public int IdProyecto { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdActa { get; set; }

    public int? IdAsociado { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    [Display(Name = "Costo")]
    public decimal? MontoTotalDestinado { get; set; }

    [Display(Name = "Num Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }

    [Display(Name = "Cod Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    [Display(Name = "Ced Asociado")]
    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
