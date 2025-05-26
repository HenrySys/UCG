using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
namespace UCG.Controllers.PDFsControllers
{
    public class InformeEconomicoPdfController : Controller
    {
        public ActionResult InformEcoPdf()
        {
            ViewBag.EsPdf = true;
            return View("InformEcoPdf");
        }

        public ActionResult Index()
        {
            
            return new ViewAsPdf("InformEcoPdf")
            {
                FileName = "InformeEconomicoPdf.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
    }
}
