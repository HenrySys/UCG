using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;
using UCG.Models.ValidationModels;


namespace UCG.Models.ValidationModels
{
    public class MovimientoViewModelValidator : AbstractValidator<MovimientoViewModel>
    {
        private readonly UcgdbContext _context;

        public MovimientoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdMovimiento)
             .NotNull().WithMessage("Debe de tener un id.")
             .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
              .MustAsync(async (id, cancellation) =>
              {
                  return !await _context.TbMovimientos.AnyAsync(a => a.IdMovimiento == id);
              })
              .WithMessage("Ya existe un movimiento con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La asociación seleccionada no existe.");

            RuleFor(x => x.IdAsociado)
                .NotNull().WithMessage("Debe seleccionar un asociado.")
                .GreaterThan(0).WithMessage("Debe seleccionar un asociado valido.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
                 })
               .WithMessage("El asociado seleccionado no existe.");

            RuleFor(x => x.TipoMovimiento)
                .NotNull().WithMessage("Debe seleccionar un movimiento.")
                .IsInEnum().WithMessage("Debe seleccionar un tipo de movimiento valido.");

            RuleFor(x => x.IdConceptoMovimiento)
                .NotNull().WithMessage("Debe seleccionar un concepto.")
                .GreaterThan(0).WithMessage("Debe seleccionar un concepto de movimiento valido.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbConceptoMovimientos.AnyAsync(a => a.IdConceptoMovimiento == id);
                 })
               .WithMessage("El concepto seleccionado no existe.");

            RuleFor(x => x.IdCategoriaMovimiento)
                .NotNull().WithMessage("Debe seleccionar una categoría.")
                .GreaterThan(0).WithMessage("Debe seleccionar una categoría de movimiento valida.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbCategoriaMovimientos.AnyAsync(a => a.IdCategoriaMovimiento == id);
                })
               .WithMessage("La categoría seleccionada no existe.");

            RuleFor(x => x.FuenteFondo)
                .NotNull().WithMessage("Debe seleccionar una fuente de fondo.")
                .IsInEnum().WithMessage("Debe seleccionar una fuente de fondo valida.");

            RuleFor(x => x.MetodoPago)
                .NotNull().WithMessage("Debe seleccionar un metodo de pago.")
                .IsInEnum().WithMessage("Debe seleccionar un método de pago valido.");

            RuleFor(x => x.IdCuenta)
                .NotNull().WithMessage("Debe seleccionar una cuenta.")
                .GreaterThan(0).WithMessage("Debe seleccionar una cuenta valida.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbCuenta.AnyAsync(a => a.IdCuenta == id);
                })
               .WithMessage("La cuenta seleccionada no existe.");

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("Debe seleccionar un estado.")
                .IsInEnum().WithMessage("Debe seleccionar un estado valido.");

            RuleFor(x => x.SubtotalMovido)
                .GreaterThanOrEqualTo(0).WithMessage("El subtotal debe ser un valor positivo.");

            RuleFor(x => x.MontoTotalMovido)
                .GreaterThanOrEqualTo(0).WithMessage("El monto total debe ser un valor positivo.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("Debe ingresar una descripción.")
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.FechaMovimiento)
                .NotNull().WithMessage("Debe seleccionar una fecha.")
                .NotEmpty().WithMessage("Debe ingresar una fecha válida.");

            RuleFor(x => x.IdProveedor)
                .GreaterThan(0).WithMessage("Debe seleccionar un proveedor de movimiento valida.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.TbProveedors.AnyAsync(a => a.IdProveedor == id);
                })
               .WithMessage("El proveedor seleccionado no existe.");

            RuleFor(x => x.IdCliente)
              .GreaterThan(0).WithMessage("Debe seleccionar un id de cliente valido.")
              .MustAsync(async (id, cancellation) =>
              {
                  return await _context.TbClientes.AnyAsync(a => a.IdCliente == id);
              })
             .WithMessage("El cliente seleccionado no existe.");

            RuleFor(x => x.IdProyecto)
              .GreaterThan(0).WithMessage("Debe seleccionar un id de proyecto valido.")
              .MustAsync(async (id, cancellation) =>
              {
                  return await _context.TbProyectos.AnyAsync(a => a.IdProyecto == id);
              })
             .WithMessage("El proyecto seleccionado no existe.");

            RuleFor(x => x.IdActa)
             .GreaterThan(0).WithMessage("Debe seleccionar una acta valida.")
             .MustAsync(async (id, cancellation) =>
             {
                 return await _context.TbActa.AnyAsync(a => a.IdActa == id);
             })
            .WithMessage("La acta seleccionada no existe.");

            RuleFor(x => x.DetallesMovimiento)
           .NotEmpty().WithMessage("Debe agregar al menos un detalle.")
           .ForEach(detalle => detalle.SetValidator(new DetalleMovimientoViewModelValidator(context)));

        }
    }
}
