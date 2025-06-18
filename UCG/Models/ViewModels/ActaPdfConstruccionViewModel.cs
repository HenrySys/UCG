using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCG.Models.ViewModels
{
    public class ActaPdfConstruccionViewModel
    {
        public TbActum Acta { get; set; } = null!;
        public string AgendaHtml { get; set; } = "";
    }
}
