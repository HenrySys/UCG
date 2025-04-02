using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbCuentum
{

    public enum EstadoDeCuenta
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,    
    }

    public enum TipoDeCuenta
    {
        [Display(Name = "Ahorro")]
        Ahorro = 1,

        [Display(Name = "Credito")]
        Credito = 2,

        [Display(Name = "Debito")]
        Debito = 3,
        
    }

    public enum BancoDeCuenta
    {
        [Display(Name = "Banco Nacional")]
        BN = 1,

        [Display(Name = "Banco Popular")]
        BP = 2,

        [Display(Name = "BCR")]
        BCR = 3,
        
        [Display(Name = "BAC")]
        BAC = 4,
        
    }
    public int IdCuenta { get; set; }

    public int? IdAsociacion { get; set; }

    public TipoDeCuenta? TipoCuenta { get; set; }

    public string TituloCuenta { get; set; } = null!;

    public int NumeroCuenta { get; set; }

    public string Telefono { get; set; } = null!;

    public EstadoDeCuenta? Estado { get; set; } 

    public int? IdAsociado { get; set; }

    public BancoDeCuenta? Banco { get; set; }

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();
}
