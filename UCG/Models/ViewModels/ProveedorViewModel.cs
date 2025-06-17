using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbProveedor;

namespace UCG.Models.ViewModels
{
    public class ProveedorViewModel
    {
        public int IdProveedor { get; set; }

        [Display(Name = "Asociación")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Tipo Proveedor")]
        public TipoDeProveedor? TipoProveedor { get; set; }

        [Display(Name = "Nombre Empresa")]
        public string? NombreEmpresa { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; } 

        [Display(Name = "Cedula Juridica")]
        public string? CedulaJuridica { get; set; }

        [Display(Name = "Nombre Contacto")]
        public string? NombreContacto { get; set; }

        [Display(Name = "Cedula Contacto")]
        public string? CedulaContacto { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Direccion")]
        public string? Direccion { get; set; }

        [Display(Name = "Fax")]
        public string? Fax { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeProveedor Estado { get; set; }

    }
}
