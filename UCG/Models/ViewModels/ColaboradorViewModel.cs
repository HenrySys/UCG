using System.ComponentModel.DataAnnotations;
namespace UCG.Models.ViewModels
{
    public class ColaboradorViewModel
    {
        public int IdColaborador { get; set; }

        [Display(Name = "Asociación")]
        public int? IdAsociacion { get; set; }

        [Display(Name= "Nombre")]
        public string Nombre { get; set; } = null!;

        [Display(Name= "Cedula")]
        public string? Cedula { get; set; }

        [Display(Name= "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name= "Correo")]
        public string? Correo { get; set; }
        
        [Display(Name= "Observaciones")]
        public string? Observaciones { get; set; }
    }
}
