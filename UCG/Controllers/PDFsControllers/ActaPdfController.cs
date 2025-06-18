using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using UCG.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UCG.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace UCG.Controllers.PDFsControllers
{
    public class ActaPdfController : Controller
    {
        private readonly UcgdbContext _context;

        public ActaPdfController(UcgdbContext context)
        {
            _context = context;
        }

        // Acción para generar el PDF
        public async Task<IActionResult> Index(int id)
        {
            var acta = await _context.TbActa
                .Include(a => a.IdAsociacionNavigation)
                .Include(a => a.IdAsociadoNavigation)
                .Include(a => a.IdFolioNavigation)
                .Include(a => a.TbActaAsistencias)
                    .ThenInclude(x => x.IdAsociadoNavigation)
                .Include(a => a.TbAcuerdos)
                .FirstOrDefaultAsync(a => a.IdActa == id);

            if (acta == null)
                return NotFound();

            return new ViewAsPdf("ActaIndividualPdf", acta)
            {
                FileName = $"Acta_{acta.NumeroActa ?? "sin_numero"}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                ContentDisposition = Rotativa.AspNetCore.Options.ContentDisposition.Inline
            };
        }

        [HttpGet]
        public async Task<IActionResult> ConstruccionDeActaPdf(int id)
        {
            var acta = await _context.TbActa
                .Include(a => a.IdAsociacionNavigation)
                .Include(a => a.IdAsociadoNavigation)
                .Include(a => a.IdFolioNavigation)
                .Include(a => a.TbActaAsistencias).ThenInclude(x => x.IdAsociadoNavigation)
                .Include(a => a.TbAcuerdos)
                .FirstOrDefaultAsync(a => a.IdActa == id);

            if (acta == null) return NotFound();

            return View(new ActaPdfConstruccionViewModel { Acta = acta });
        }

        [HttpPost]
        public async Task<IActionResult> GenerarPdfConAgenda(ActaPdfConstruccionViewModel model)
        {
            var acta = await _context.TbActa
                .Include(a => a.IdAsociacionNavigation)
                .Include(a => a.IdAsociadoNavigation)
                .Include(a => a.IdFolioNavigation)
                .Include(a => a.TbActaAsistencias).ThenInclude(x => x.IdAsociadoNavigation)
                .Include(a => a.TbAcuerdos)
                .FirstOrDefaultAsync(a => a.IdActa == model.Acta.IdActa);

            if (acta == null) return NotFound();

            // ► Normalizamos a HTML por si viene texto plano
            var agenda = string.IsNullOrWhiteSpace(model.AgendaHtml)
                ? ""
                : (model.AgendaHtml.Trim().StartsWith("<")
                    ? model.AgendaHtml
                    : $"<ol>{string.Join("", model.AgendaHtml.Split('\n').Select(p => $"<li>{p.Trim()}</li>"))}</ol>");


            // ► Crear el PDF y pasar la agenda por ViewData
            var pdf = new ViewAsPdf("ActaIndividualPdf", acta)
            {
                FileName = $"Acta_{acta.NumeroActa ?? "sin_numero"}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                ContentDisposition = Rotativa.AspNetCore.Options.ContentDisposition.Inline,
                ViewData = new ViewDataDictionary<TbActum>(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: new ModelStateDictionary())
                {
                    Model = acta,
                    ["AgendaHtml"] = agenda
                }
            };


            return pdf;
        }

    }
}
