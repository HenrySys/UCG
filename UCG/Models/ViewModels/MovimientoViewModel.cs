using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbMovimiento;

namespace UCG.Models.ViewModels
{
    public class MovimientoViewModel
    {
        public int IdMovimiento { get; set; }

        [Display(Name = "Asociación")]
        public int IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int IdAsociado { get; set; }

        [Display(Name = "Tipo de movimiento")]
        public TipoDeMovimiento TipoMovimiento { get; set; }

        [Display(Name = "Concepto de movimiento")]
        public int IdConceptoMovimiento { get; set; }

        [Display(Name = "Categoría de movimiento")]
        public int IdCategoriaMovimiento { get; set; }

        [Display(Name = "Fuente de fondo")]
        public FuentesDeFondo FuenteFondo { get; set; }

        [Display(Name = "Método de pago")]
        public MetodosDePago MetodoPago { get; set; }

        [Display(Name = "Cuenta")]
        public int IdCuenta { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeMovimiento Estado { get; set; }

        [Display(Name = "Monto subtotal del movimiento")]

        public decimal SubtotalMovido { get; set; }

        [Display(Name = "Monto total del movimiento")]

        public decimal MontoTotalMovido { get; set; }

        [Display(Name = "Descripción del movimiento")]
        public string Descripcion { get; set; } = null!;

        [Display(Name = "Fecha de movimiento")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido.")]
        public DateOnly FechaMovimiento { get; set; }

        [Display(Name = "Proveedor")]
        public int? IdProveedor { get; set; }

        [Display(Name = "Cliente")]
        public int? IdCliente { get; set; }

        [Display(Name = "Proyecto")]
        public int? IdProyecto { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Detalles de movimiento")]
        // **Lista de detalles del movimiento**
        public List<DetalleMovimientoViewModel> DetallesMovimiento { get; set; } = new List<DetalleMovimientoViewModel>();


    }
}
