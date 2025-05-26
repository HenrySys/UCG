using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;
using System.Globalization;

namespace UCG.Models.ValidationModels
{
    public class FolioViewModelValidator : AbstractValidator<FolioViewModel>
    {
        private readonly UcgdbContext _context;

        public FolioViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una Asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar una Asociación válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un Asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Asociado válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbAsociados
                        .AnyAsync(a => a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .WithMessage("El asociado no existe o no pertenece a la asociación seleccionada.");

            RuleFor(x => x.FechaTextoEmision)
                .NotEmpty().WithMessage("Debe ingresar la fecha de emisión.")
                .Must(SerFechaValida).WithMessage("La fecha de emisión no tiene un formato válido (dd/MM/yyyy).");

            RuleFor(x => x.FechaTextoCierre)
                .NotEmpty().WithMessage("Debe ingresar la fecha de cierre.")
                .Must(SerFechaValida).WithMessage("La fecha de cierre no tiene un formato válido (dd/MM/yyyy).");

            //RuleFor(x => new { x.FechaTextoEmision, x.FechaTextoCierre })
            //    .Must(fechas =>
            //    {
            //        if (!DateTime.TryParseExact(fechas.FechaTextoEmision, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var emision))
            //            return false;
            //        if (!DateTime.TryParseExact(fechas.FechaTextoCierre, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var cierre))
            //            return false;
            //        return emision <= cierre;
            //    })
            //    .WithMessage("La fecha de emisión debe ser anterior o igual a la fecha de cierre.");

            RuleFor(x => x.NumeroFolio)
                .NotNull().WithMessage("Debe ingresar el número de folio.")
                .NotEmpty().WithMessage("Debe ingresar el número de folio.")
                .MaximumLength(100).WithMessage("El número de folio no puede superar los 100 caracteres.")
                .MustAsync(async (model, numeroFolio, _) =>
                    !await _context.TbFolios
                        .AnyAsync(f => f.NumeroFolio == numeroFolio && f.IdFolio != model.IdFolio))
                .WithMessage("Ya existe un folio con ese número.");

            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("La descripción no puede ser nula.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            //RuleFor(x => x.Estado)
            //    .IsInEnum().WithMessage("Debe seleccionar un estado válido.");
        }

        private bool SerFechaValida(string? FechaTexto)
        {
            return DateTime.TryParseExact(FechaTexto, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var fecha);
        }


    }
}
