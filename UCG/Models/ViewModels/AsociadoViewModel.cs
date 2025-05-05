using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbAsociado;

namespace UCG.Models.ViewModels
{
    public class AsociadoViewModel
    {
        [Display(Name = "Fecha de nacimiento")]
        public string? FechaTexto { get; set; } 

        public int IdAsociado { get; set; }

        [Display(Name = "Asociación")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Usuario")]
        public int? IdUsuario { get; set; }

        [Display(Name = "Nacionalidad")]
        public NacionalidadDeAsociado? Nacionalidad { get; set; }

        [Display(Name = "Cedula")]
        public string? Cedula { get; set; }

        [Display(Name = "Apellido Primero")]
        public string? Apellido1 { get; set; }

        [Display(Name = "Apellido Segundo")]
        public string? Apellido2 { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        public DateOnly FechaNacimiento { get; set; }

        [Display(Name = "Sexo")]
        public SexoDeAsociado? Sexo { get; set; }

        [Display(Name = "Estado Civil")]
        public EstadoCivilDeAsociado? EstadoCivil { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Direccion")]
        public string? Direccion { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeAsociado Estado { get; set; }

    }
}
