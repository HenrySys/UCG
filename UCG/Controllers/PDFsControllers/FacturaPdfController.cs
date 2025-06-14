using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UCG.Models;

namespace UCG.Controllers.PDFsControllers
{
    public class FacturaPdfController : Controller
    {
        private readonly UcgdbContext _context;

        public FacturaPdfController(UcgdbContext context)
        {
            _context = context;
        }

        // Acción para generar un PDF individual
        public async Task<IActionResult> DetalleFacturaPdf(int id)
        {
            var factura = await _context.TbFacturas
                .Include(f => f.IdAsociacionNavigation)
                .Include(f => f.IdAsociadoNavigation)
                .Include(f => f.IdColaboradorNavigation)
                .Include(f => f.IdProveedorNavigation)
                .Include(f => f.IdConceptoAsociacionNavigation)
                    .ThenInclude(ca => ca.IdConceptoNavigation)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null)
            {
                return NotFound("Factura no encontrada.");
            }

            return new ViewAsPdf("FacturaIndividualPdf", factura)
            {
                FileName = $"Factura_{factura.NumeroFactura}.pdf"
            };
        }
    }
}
