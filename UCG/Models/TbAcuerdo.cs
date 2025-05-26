using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UCG.Models;

public partial class TbAcuerdo
{
    public enum TipoDeAcuerdo
    {
        [Display(Name = "Compra")]
        Compra = 1,

        [Display(Name = "Pago")]
        Pago = 2,

        [Display(Name = "Ordinario")]
        Ordinario = 3 
    }
    public int IdAcuerdo { get; set; }

    public int? IdActa { get; set; }
    [Display(Name = "Numero Acuerdo")]
    public string? NumeroAcuerdo { get; set; }
    [Display(Name = "Nombre")]
    public string? Nombre { get; set; }
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Monto Acuerdo")]
    public decimal? MontoAcuerdo { get; set; }
    [Display(Name = "Tipo Acuerdo")]
    public TipoDeAcuerdo? Tipo { get; set; }
    [Display(Name = "Acta")]
    public virtual TbActum? IdActaNavigation { get; set; }

    public virtual ICollection<TbDetalleChequeFactura> TbDetalleChequeFacturas { get; } = new List<TbDetalleChequeFactura>();
}
