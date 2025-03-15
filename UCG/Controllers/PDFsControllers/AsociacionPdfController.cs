using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using UCG.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace UCG.Controllers.PDFsControllers
{
    public class AsociacionPdfController : Controller
    {
        private readonly UcgdbContext _context;

        public AsociacionPdfController(UcgdbContext context)
        {
            _context = context;
        }

        // Acción para generar el PDF
        public async Task<IActionResult> Index()
        {
            // Obtienes las asociaciones directamente desde la base de datos
            var asociaciones = await _context.TbAsociacions.ToListAsync();

            // Devuelves el PDF con el modelo pasado a la vista
            return new ViewAsPdf("IndexAsociacionesPdf", asociaciones);
        }
    }
}
