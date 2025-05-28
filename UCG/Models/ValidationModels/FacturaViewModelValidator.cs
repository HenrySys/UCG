using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class FacturaViewModelValidator : AbstractValidator<FacturaViewModel>
    {
        private readonly UcgdbContext _context;

        public FacturaViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.NumeroFactura)
                .NotNull().WithMessage("El número de factura es obligatorio.")
                .NotEmpty().WithMessage("Debe ingresar el número de factura.")
                .MaximumLength(50).WithMessage("El número de factura no puede superar los 50 caracteres.")
                .MustAsync(async (model, numero, cancellation) =>
                    !await _context.TbFacturas
                        .AnyAsync(f => f.NumeroFactura == numero &&
                                       f.IdFactura != model.IdFactura &&
                                       f.IdAsociacion == model.IdAsociacion))
                .WithMessage("Ya existe una factura con ese número en esta asociación.");

            RuleFor(x => x.FechaTextoEmision)
                .NotEmpty().WithMessage("Debe ingresar la fecha de emisión.")
                .Must(SerFechaValida).WithMessage("La fecha de emisión no es válida (formato requerido: dd/MM/yyyy).");

            //RuleFor(x => x.FechaTextoSubida)
            //    .NotEmpty().WithMessage("Debe ingresar la fecha de subida.")
            //    .Must(SerFechaValida).WithMessage("La fecha de subida no es válida (formato requerido: dd/MM/yyyy).");

            RuleFor(x => x.IdAsociacion)
                .NotNull().WithMessage("Debe seleccionar una asociación.")
                .GreaterThan(0).WithMessage("La asociación seleccionada no es válida.")
                .MustAsync(async (id, _) =>
                    await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id))
                .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdConceptoAsociacion)
                .NotNull().WithMessage("Debe seleccionar un concepto de asociación.")
                .GreaterThan(0).WithMessage("Debe seleccionar un concepto válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbConceptoAsociacions
                        .AnyAsync(c => c.IdConceptoAsociacion == id && c.IdAsociacion == model.IdAsociacion))
                .WithMessage("El concepto no pertenece a la asociación seleccionada.");

            RuleFor(x => x.IdAsociado)
                .GreaterThan(0).When(x => x.TipoEmisor == "asociado")
                .WithMessage("Debe seleccionar un asociado válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbAsociados
                        .AnyAsync(a => a.IdAsociado == id && a.IdAsociacion == model.IdAsociacion))
                .When(x => x.TipoEmisor == "asociado")
                .WithMessage("El asociado no pertenece a la asociación seleccionada.");

            RuleFor(x => x.IdColaborador)
                .GreaterThan(0).When(x => x.TipoEmisor == "colaborador")
                .WithMessage("Debe seleccionar un colaborador válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbColaboradors
                        .AnyAsync(c => c.IdColaborador == id && c.IdAsociacion == model.IdAsociacion))
                .When(x => x.TipoEmisor == "colaborador")
                .WithMessage("El colaborador no pertenece a la asociación seleccionada.");

            RuleFor(x => x.IdProveedor)
                .GreaterThan(0).When(x => x.TipoEmisor == "proveedor")
                .WithMessage("Debe seleccionar un proveedor válido.")
                .MustAsync(async (model, id, _) =>
                    await _context.TbProveedors
                        .AnyAsync(p => p.IdProveedor == id && p.IdAsociacion == model.IdAsociacion))
                .When(x => x.TipoEmisor == "proveedor")
                .WithMessage("El proveedor no pertenece a la asociación seleccionada.");


            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("La descripción no puede ser nula.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.MontoTotal)
                .NotNull().WithMessage("Debe ingresar un monto.")
                .GreaterThan(0).WithMessage("El monto total debe ser mayor a ₡0.")
                .LessThanOrEqualTo(9999999999).WithMessage("El monto total no puede superar ₡9,999,999,999.");

            //RuleFor(x => x.ArchivoUrl)
            //    .NotNull().WithMessage("La ruta del archivo no puede ser nula.")
            //    .MaximumLength(300).WithMessage("La ruta del archivo no puede superar los 300 caracteres.");

            //RuleFor(x => x.NombreArchivo)
            //    .NotNull().WithMessage("El nombre del archivo no puede ser nulo.")
            //    .MaximumLength(150).WithMessage("El nombre del archivo no puede superar los 150 caracteres.");
        }

        private bool SerFechaValida(string? fecha)
        {
            return DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
