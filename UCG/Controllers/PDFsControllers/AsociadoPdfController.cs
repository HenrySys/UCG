using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using UCG.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace UCG.Controllers.PDFsControllers
{
    public class AsociadoPdfController : Controller
    {
        private readonly UcgdbContext _context;
        private string rol => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        public AsociadoPdfController(UcgdbContext context)
        {
            _context = context;
        }

        // Acción para generar el PDF
        public async Task<IActionResult> Index()
        {
            // Obtienes las asociaciones directamente desde la base de datos
            var asociados = await _context.TbAsociados.ToListAsync();

            // Devuelves el PDF con el modelo pasado a la vista
            return new ViewAsPdf("IndexAsociadosPdf", asociados);
        }
    }
}
