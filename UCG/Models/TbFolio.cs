using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbFolio
{
    public enum EstadoDeFolio
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Anulado")]
        Anulado = 2,

        [Display(Name= "Cerrado")]
        Cerrado = 3
    }
    public int IdFolio { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdAsociado { get; set; }

    [Display(Name = "Fecha Emision")]
    public DateOnly FechaEmision { get; set; }
    [Display(Name = "Numero Folio")]
    public string? NumeroFolio { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Estado")]
    public EstadoDeFolio? Estado { get; set; } = null!;
    [Display(Name = "FechaCierre")]
    public DateOnly FechaCierre { get; set; }
    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
    [Display(Name = "Asociado")]

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual ICollection<TbActum> TbActa { get; } = new List<TbActum>();
}
