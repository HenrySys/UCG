using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbColaborador
{
    public int IdColaborador { get; set; }

    [Display(Name= "Nombre")]
    public string Nombre { get; set; } = null!;
    [Display(Name= "Cedula")]
    public string? Cedula { get; set; }
    [Display(Name= "Telefono")]
    public string? Telefono { get; set; }
    [Display(Name= "Correo")]
    public string? Correo { get; set; }
    [Display(Name= "Observaciones")]
    public string? Observaciones { get; set; }

    [Display(Name= "Asociacion")]
    public int? IdAsociacion { get; set; }

    [Display(Name= "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; } = null!;
    public virtual ICollection<TbFactura> TbFacturas { get; } = new List<TbFactura>();
}
