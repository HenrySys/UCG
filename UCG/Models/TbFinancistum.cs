using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbFinancistum
{
    public enum TipoEntidades
    {
        [Display(Name = "Publica")]
        publica = 1,

        [Display(Name = "Privada")]
        privada = 2,

    }   
    
    public int IdFinancista { get; set; }
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    [Display(Name = "Tipo Entidad")]
    public TipoEntidades? TipoEntidad { get; set; } = null!;
    [Display(Name = "Descripcion")]
    public string? Descripcion { get; set; }
    [Display(Name = "Telefono")]
    public string? Telefono { get; set; }
    [Display(Name = "Correo")]
    public string? Correo { get; set; }
    [Display(Name = "Sitio Web")]
    public string? SitioWeb { get; set; }

    public int IdAsociacion { get; set; }
    [Display(Name = "Asociacion")]
    public virtual TbAsociacion IdAsociacionNavigation { get; set; } = null!;

    public virtual ICollection<TbDocumentoIngreso> TbDocumentoIngresos { get; } = new List<TbDocumentoIngreso>();
}
