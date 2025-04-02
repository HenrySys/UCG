using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbAsociacion;

namespace UCG.Models.ViewModels
{
    public class AsociacionViewModel
    {
        public int IdAsociacion { get; set; }

        [Display(Name = "Cedula Juridica")]
        public string? CedulaJuridica { get; set; }

        [Display(Name = "Codigo Registro")]
        public string? CodigoRegistro { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Fecha Costitucion")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido.")]
        public DateOnly FechaConstitucion { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Fax")]
        public string? Fax { get; set; }

        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Provincia")]
        public string? Provincia { get; set; }

        [Display(Name = "Canton")]
        public string? Canton { get; set; }

        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }

        [Display(Name = "Pueblo")]
        public string? Pueblo { get; set; }

        [Display(Name = "Direccion")]
        public string? Direccion { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; } = null!;

        [Display(Name = "Estado")]
        public EstadoDeAsociacion Estado { get; set; }
    }
}
