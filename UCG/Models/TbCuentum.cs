using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbCuentum
{
    public enum TipoDeCuenta
    {
        [Display(Name = "Ahorro")]
        Ahorro = 1,

        [Display(Name = "Credito")]
        Credito = 2,
        
        [Display(Name= "Debito")]
        Debito = 3
    }

    public enum EstadoDeCuenta
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,

    }   

    public enum BancoDeCuenta
    {
        [Display(Name = "Banco Nacional")]
        BN = 1,

        [Display(Name = "Banco Popular")]
        BP = 2,

        [Display(Name= "Banco Costa Rica")]
        BCR = 3,

        [Display(Name= "BAC")]
        BAC = 4
    }      

    public int IdCuenta { get; set; }

    public int? IdAsociacion { get; set; }
    [Display(Name= "Cuenta")]

    public TipoDeCuenta? TipoCuenta { get; set; }
    [Display(Name= "Titulo de Cuenta")]
    public string? TituloCuenta { get; set; }
    [Display(Name= "Numero Cuenta")]
    public string? NumeroCuenta { get; set; }
    [Display(Name= "Telefono")]
    public string? Telefono { get; set; } 
    [Display(Name= "Estado")]
    public EstadoDeCuenta? Estado { get; set; } 
    public int? IdAsociado { get; set; }
    [Display(Name= "Banco")]
    public BancoDeCuenta? Banco { get; set; } 
    [Display(Name= "Saldo")]
    public decimal? Saldo { get; set; }
    [Display(Name= "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual ICollection<TbCheque> TbCheques { get; } = new List<TbCheque>();

    public virtual ICollection<TbDocumentoIngreso> TbDocumentoIngresos { get; } = new List<TbDocumentoIngreso>();
}
