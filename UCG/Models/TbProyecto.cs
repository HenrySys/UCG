using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbProyecto
{
    public int IdProyecto { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdActa { get; set; }

    public int? IdAsociado { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public decimal? MontoTotalDestinado { get; set; }

    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }
}
