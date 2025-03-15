using System;
using System.Collections.Generic;

namespace UCG.Models;

public partial class TbAsociado
{
    public int IdAsociado { get; set; }

    public int? IdAsociacion { get; set; }

    public int? IdUsuario { get; set; }

    public string Nacionalidad { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string? Sexo { get; set; }

    public string? EstadoCivil { get; set; }

    public string Telefono { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual TbAsociacion? IdAsociacionNavigation { get; set; }

    public virtual TbUsuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<TbActum> TbActa { get; } = new List<TbActum>();

    public virtual ICollection<TbActaAsistencium> TbActaAsistencia { get; } = new List<TbActaAsistencium>();

    public virtual ICollection<TbCategoriaMovimiento> TbCategoriaMovimientos { get; } = new List<TbCategoriaMovimiento>();

    public virtual ICollection<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; } = new List<TbMiembroJuntaDirectiva>();

    public virtual ICollection<TbMovimiento> TbMovimientos { get; } = new List<TbMovimiento>();

    public virtual ICollection<TbProyecto> TbProyectos { get; } = new List<TbProyecto>();
}
