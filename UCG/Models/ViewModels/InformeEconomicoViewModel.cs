// Models/ViewModels/InformeEconomicoViewModel.cs
using System.Collections.Generic;

namespace UCG.Models.ViewModels
{
    public class InformeEconomicoViewModel
    {
        public List<TbMovimientoIngreso> Ingresos { get; set; } = new();
        public List<TbMovimientoEgreso> Egresos { get; set; } = new();
        public decimal TotalIngresos { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }
        public decimal TotalIngresosConSaldo { get; set; }
        public decimal TotalEgresosConSaldo { get; set; }

        public TbAsociacion? Asociacion { get; set; }
    }
}
