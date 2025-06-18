using static UCG.Models.TbDocumentoIngreso;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models.ViewModels
{
    public class DocumentoIngresoViewModel
    {
    public int IdDocumentoIngreso { get; set; }

    [Display(Name = "Fecha Comprobante")]
    public string? FechaComprobante { get; set; }

    [Display(Name = "Numero Comprobante")]
    public string NumComprobante { get; set; } = null!;

    [Display(Name = "Movimiento Ingreso")]
    public int IdMovimientoIngreso { get; set; }

    [Display(Name = "Concepto Asociacion")]
    public int? IdConceptoAsociacion { get; set; }

    [Display(Name = "Cuenta")]
    public int? IdCuenta { get; set; }

    [Display(Name = "Cliente")]
    public int? IdCliente { get; set; }

    [Display(Name = "Financista")]
    public int? IdFinancista { get; set; }

    [Display(Name = "Actividad")]
    public int? IdActividad { get; set; }

    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }

    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }

    [Display(Name = "Metodo de Pago")]
    public MetodoDePago? MetodoPago { get; set; }
    }
}
