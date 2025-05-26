using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbCliente;

namespace UCG.Models.ViewModels
{
    public class ClienteViewModel
    {

        public int IdCliente { get; set; }

        [Display(Name = "Asociación")]
        public int? IdAsociacion { get; set; }

        [Display(Name = "Apellido Primero")]
        public string? Apellido1 { get; set; }

        [Display(Name = "Apellido Segundo")]
        public string? Apellido2 { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Cedula")]
        public string? Cedula { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Direccion")]
        public string? Direccion { get; set; }

        [Display(Name = "Estado")]
        public EstadoDeCliente? Estado { get; set; }


    }
}
