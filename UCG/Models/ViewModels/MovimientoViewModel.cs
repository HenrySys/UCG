
using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbMovimiento;

namespace UCG.Models.ViewModels
{
    public class MovimientoViewModel
    {
        public int IdMovimiento { get; set; }

        [Display(Name = "Asociación")]
        [Required(ErrorMessage = "Debe seleccionar una asociación.")]
        public int IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        [Required(ErrorMessage = "Debe seleccionar un asociado.")]
        public int IdAsociado { get; set; }

        [Display(Name = "Tipo de movimiento")]
        [Required(ErrorMessage = "Debe seleccionar un tipo de movimiento.")]
        public TiposDeMovimientos TipoMovimiento { get; set; }

        [Display(Name = "Concepto de movimiento")]
        [Required(ErrorMessage = "Debe seleccionar un concepto de movimiento.")]
        public int IdConceptoMovimiento { get; set; }

        [Display(Name = "Categoría de movimiento")]
        [Required(ErrorMessage = "Debe seleccionar una categoría de movimiento.")]
        public int IdCategoriaMovimiento { get; set; }

        [Display(Name = "Fuente de fondo")]
        [Required(ErrorMessage = "Debe seleccionar una fuente de fondo.")]
        public FuentesDeFondo FuenteFondo { get; set; }

        [Display(Name = "Método de pago")]
        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        public MetodosDePago MetodoPago { get; set; }

        [Display(Name = "Cuenta")]
        [Required(ErrorMessage = "Debe seleccionar una cuenta.")]
        public int IdCuenta { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public EstadoDeMovimiento Estado { get; set; }

        [Display(Name = "Monto subtotal del movimiento")]
        [Required(ErrorMessage = "El monto subtotal es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser un valor positivo.")]
        public decimal SubtotalMovido { get; set; }

        [Display(Name = "Monto total del movimiento")]
        [Required(ErrorMessage = "El monto total es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto total debe ser un valor positivo.")]
        public decimal MontoTotalMovido { get; set; }

        [Display(Name = "Descripción del movimiento")]
        [Required(ErrorMessage = "Debe ingresar una descripción.")]
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [Display(Name = "Fecha de movimiento")]
        [Required(ErrorMessage = "Debe ingresar una fecha válida.")]
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

        public bool? SinAsociados { get; set; }
    }
}
