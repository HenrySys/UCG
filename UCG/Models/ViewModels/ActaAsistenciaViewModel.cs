using System.ComponentModel.DataAnnotations;
using static UCG.Models.TbActaAsistencium;

namespace UCG.Models.ViewModels
{
    public class ActaAsistenciaViewModel{

        public int IdActaAsistencia { get; set; }
        
        [Display(Name = "Asociado")]
        public int IdAsociado { get; set; }

        [Display(Name = "Acta")]
        public int? IdActa { get; set; }

        [Display(Name = "Fecha de Asistencia")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido.")]
        public DateOnly Fecha { get; set; }

    }
}
