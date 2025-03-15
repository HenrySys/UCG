using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbMovimiento
{
    public int IdMovimiento { get; set; }

    public int IdAsociacion { get; set; }

    public int? IdAsociado { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int? IdCategoriaMovimiento { get; set; }

    public string FuenteFondo { get; set; } = null!;

    public int? IdProyecto { get; set; }

    public int? IdCuenta { get; set; }

    public int? IdActa { get; set; }

    public int? IdProveedor { get; set; }

    public int? IdCliente { get; set; }

    public string Descripcion { get; set; } = null!;

    public string MetdodoPago { get; set; } = null!;

    public DateOnly FechaMovimiento { get; set; }

    public decimal? SubtotalMovido { get; set; }

    public decimal? MontoTotalMovido { get; set; }

    public string Estado { get; set; } = null!;

    public int? IdConcepto { get; set; }

    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual TbAsociacion IdAsociacionNavigation { get; set; } = null!;

    public virtual TbAsociado? IdAsociadoNavigation { get; set; }

    public virtual TbCategoriaMovimiento? IdCategoriaMovimientoNavigation { get; set; }

    public virtual TbConceptoMovimiento? IdConceptoNavigation { get; set; }

    public virtual TbCuentum? IdCuentaNavigation { get; set; }

    public virtual TbProveedor? IdProveedorNavigation { get; set; }

    public virtual TbProyecto? IdProyectoNavigation { get; set; }

    public virtual ICollection<TbDetalleMovimiento> TbDetalleMovimientos { get; } = new List<TbDetalleMovimiento>();
}
