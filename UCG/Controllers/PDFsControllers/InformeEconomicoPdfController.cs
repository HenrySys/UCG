using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using UCG.Models.ViewModels;
using UCG.Models;
using Microsoft.EntityFrameworkCore; 


public class InformeEconomicoPdfController : Controller
{
    private readonly UcgdbContext _context;

    public InformeEconomicoPdfController(UcgdbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> InformEcoPdf()
    {
        var model = await GenerarModeloInforme();
        if (model == null)
        {
            TempData["ErrorMessage"] = "Asociación no encontrada.";
            return RedirectToAction("Index", "Home");
        }

        return View("InformEcoPdf", model);
    }

    public async Task<IActionResult> Index()
    {
        var model = await GenerarModeloInforme();
        if (model == null)
            return RedirectToAction("Index", "Home");

        return new ViewAsPdf("InformEcoPdf", model)
        {
            FileName = "InformeEconomicoPdf.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
        };
    }

    private async Task<InformeEconomicoViewModel?> GenerarModeloInforme()
    {
        var idAsociacion = ObtenerIdAsociacion();

        if (idAsociacion == null)
            return null;

        var asociacion = await _context.TbAsociacions
       .FirstOrDefaultAsync(a => a.IdAsociacion == idAsociacion);

        if (asociacion == null)
            return null;



        var ingresos = await _context.TbMovimientoIngresos
            .Where(m => m.IdAsociacion == idAsociacion)
            .ToListAsync();

        var egresos = await _context.TbMovimientoEgresos
            .Where(m => m.IdAsociacion == idAsociacion)
            .ToListAsync();

        var totalIngresos = ingresos.Sum(m => m.MontoTotalIngresado ?? 0);
        var totalEgresos = egresos.Sum(m => m.Monto );
        var saldoInicial = 0m;
        var saldoFinal = saldoInicial + totalIngresos - totalEgresos;

        return new InformeEconomicoViewModel
        {
            Asociacion = asociacion,
            Ingresos = ingresos,
            Egresos = egresos,
            TotalIngresos = totalIngresos,
            TotalEgresos = totalEgresos,
            SaldoInicial = saldoInicial,
            SaldoFinal = saldoFinal,
            TotalIngresosConSaldo = totalIngresos + saldoInicial,
            TotalEgresosConSaldo = totalEgresos + saldoFinal
        };
    }

    private int? ObtenerIdAsociacion()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdAsociacion");
        if (claim != null && int.TryParse(claim.Value, out int idAsociacion))
            return idAsociacion;
        return null;
    }
}
