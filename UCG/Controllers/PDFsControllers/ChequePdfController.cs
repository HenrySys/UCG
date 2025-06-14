using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using UCG.Models;

namespace UCG.Controllers.PDFsControllers
{
    public class ChequePdfController : Controller
    {
        private readonly UcgdbContext _context;

        public ChequePdfController(UcgdbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> VerCheque(int id)
        {
            var cheque = await _context.TbCheques
                .Include(c => c.IdAsociacionNavigation)
                .Include(c => c.IdAsociadoAutorizaNavigation)
                .Include(c => c.IdCuentaNavigation)
                .FirstOrDefaultAsync(c => c.IdCheque == id);

            if (cheque == null)
                return NotFound("Cheque no encontrado");

            return new ViewAsPdf("ChequeIndividualPdf", cheque)
            {
                FileName = $"Cheque_{cheque.NumeroCheque}.pdf"
            };
        }
    }
}
