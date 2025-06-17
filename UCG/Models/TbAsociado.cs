using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCG.Models;

public partial class TbAsociado
{
    public enum EstadoDeAsociado
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Inactivo")]
        Inactivo = 2,
        
    }

    public enum SexoDeAsociado{
        [Display(Name = "Femenino")]
        Femenino= 1,
        [Display(Name = "Masculino")]
        Masculino= 2,
        [Display(Name = "Otro")]
        Otro= 3,
    }

    public enum NacionalidadDeAsociado{
        [Display(Name = "Nacional")]
        Nacional= 1,
        [Display(Name = "Residente")]
        Residente= 2,
        [Display(Name = "Extranjero")]
        Extranjero= 3,
    }

    public enum EstadoCivilDeAsociado{
        [Display(Name = "Soltero(a)")]
        Soltero= 1,
        [Display(Name = "Casado(a)")]
        Casado= 2,
        [Display(Name = "Divorciado(a)")]
        Divorciado= 3,
        [Display(Name = "Viudo(a)")]
        Viudo= 4,
        [Display(Name = "Union libre")]
        UnionLibre= 5
    }
    public int IdAsociado { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdUsuario { get; set; }
    [Display(Name = "Nacionalidad")]
    public NacionalidadDeAsociado? Nacionalidad { get; set; }
    [Display(Name = "Cedula")]
    public string Cedula { get; set; } = null!;
    [Display(Name = "Primer Apellido")]
    public string Apellido1 { get; set; } = null!;
    [Display(Name = "Segundo Apellido")]
    public string? Apellido2 { get; set; }
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    [Display(Name = "Fecha Nacimiento")]
    public DateOnly FechaNacimiento { get; set; }
    [Display(Name = "Sexo")]
    public SexoDeAsociado? Sexo { get; set; }
    [Display(Name = "Estado Civil")]
    public EstadoCivilDeAsociado? EstadoCivil { get; set; }
    [Display(Name = "Telefono")]
    public string Telefono { get; set; } = null!;
    [Display(Name = "Correo")]
    public string Correo { get; set; } = null!;
    [Display(Name = "Direccion")]
    public string Direccion { get; set; } = null!;
    [Display(Name = "Estado")]
    public EstadoDeAsociado? Estado { get; set; } = null!;
    [Display(Name = "Asociacion")]
    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }
    [Display(Name = "Usuario")]
    public virtual TbUsuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<TbActum> TbActa { get; } = new List<TbActum>();

    public virtual ICollection<TbActaAsistencium> TbActaAsistencia { get; } = new List<TbActaAsistencium>();

    public virtual ICollection<TbActividad> TbActividads { get; } = new List<TbActividad>();

    public virtual ICollection<TbCheque> TbCheques { get; } = new List<TbCheque>();

    public virtual ICollection<TbFactura> TbFacturas { get; } = new List<TbFactura>();

    public virtual ICollection<TbFolio> TbFolios { get; } = new List<TbFolio>();

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();

    public virtual ICollection<TbMovimientoEgreso> TbMovimientoEgresos { get; } = new List<TbMovimientoEgreso>();

    public virtual ICollection<TbMovimientoIngreso> TbMovimientoIngresos { get; } = new List<TbMovimientoIngreso>();
}
