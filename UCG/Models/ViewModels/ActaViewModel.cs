
using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbActum;
namespace UCG.Models.ViewModels
{
    public class ActaViewModel
    {
        public int IdActa { get; set; }

        [Display(Name = "Fecha de Acta")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido.")]
        public DateOnly FechaSesion { get; set; }
        [Display(Name = "NUmero de acta")]
        public string? NumeroActa { get; set; }
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }
        [Display(Name = "Estado")]
        public EstadoDeActa? Estado { get; set; }
        [Display(Name = "Monto total acordado")]
        public decimal? MontoTotalAcordado { get; set; }
        [Display(Name = "Asociado")]
        public int? IdAsociado { get; set; }

        public List<ActaAsistenciaViewModel> ActaAsistencia { get; set; } = new List<ActaAsistenciaViewModel>();

    }
}
