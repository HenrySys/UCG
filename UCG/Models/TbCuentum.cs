using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbCuentum
{
    public int IdCuenta { get; set; }

    public int? IdAsociacion { get; set; }

    public string TipoCuenta { get; set; } = null!;

    public string TituloCuenta { get; set; } = null!;

    public int NumeroCuenta { get; set; }

    public string Telefono { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
