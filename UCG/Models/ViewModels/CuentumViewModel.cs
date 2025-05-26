using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbCuentum;

namespace UCG.Models.ViewModels
{
    public class CuentumViewModel
    {
        public int IdCuenta { get; set; }

        [Display(Name = "Asociación")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        [Display(Name = "Tipo Cuenta")]
        public TipoDeCuenta? TipoCuenta { get; set; }

        [Display(Name = "Titulo Cuenta")]
        public string? TituloCuenta { get; set; }

        [Display(Name = "Numero Cuenta")]
        public string? NumeroCuenta { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeCuenta? Estado { get; set; }

        [Display(Name = "Banco")]
        public BancoDeCuenta Banco { get; set; }

        [Display(Name = "Saldo")]
        public decimal Saldo { get; set; }

    }
}
