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
        [Display(Name = "Masculino")]
        Masculino= 1,
        [Display(Name = "Femenino")]
        Femenino= 2,
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

    public NacionalidadDeAsociado? Nacionalidad { get; set; }

    public string Cedula { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public SexoDeAsociado? Sexo { get; set; }

    public EstadoCivilDeAsociado? EstadoCivil { get; set; }

    public string Telefono { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public EstadoDeAsociado? Estado { get; set; } = null!;

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual TbUsuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<TbActaAsistencium> TbActaAsistencia { get; } = new List<TbActaAsistencium>();

    public virtual ICollection<TbActum> TbActas { get; } = new List<TbActum>();

    public virtual ICollection<TbCategoriaMovimiento> TbCategoriaMovimientos { get; } = new List<TbCategoriaMovimiento>();

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();

    public virtual ICollection<TbProyecto> TbProyectos { get; } = new List<TbProyecto>();
}
