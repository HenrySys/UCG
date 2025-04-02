using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCG.Models.ViewModels;

namespace UCG.Models.ValidationModels
{
    public class ProyectoViewModelValidator: AbstractValidator<ProyectoViewModel>
    {
        private readonly UcgdbContext _context;
        public ProyectoViewModelValidator(UcgdbContext context)
        {
            _context = context;

            RuleFor(x => x.IdProyecto)
                .NotNull().WithMessage("Debe de tener un id.")
                .GreaterThan(0).WithMessage("Debe seleccionar una id valido.")
                .MustAsync(async (id, cancellation) =>
                {
                    return !await _context.TbProyectos.AnyAsync(a => a.IdProyecto == id);
                })
                .WithMessage("Ya existe un proveedor con ese id.");

            RuleFor(x => x.IdAsociacion)
              .NotNull().WithMessage("Debe seleccionar una Asociación.")
              .GreaterThan(0).WithMessage("Debe seleccionar una Asociación valida.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociacions.AnyAsync(a => a.IdAsociacion == id);
               })
               .WithMessage("La Asociación seleccionada no existe.");

            RuleFor(x => x.IdActa)
                .NotNull().WithMessage("Debe seleccionar un Acta.")
                .GreaterThan(0).WithMessage("Debe seleccionar un Acta valido.")
                 .MustAsync(async (id, cancellation) =>
                 {
                     return await _context.TbActa.AnyAsync(a => a.IdActa == id);
                 })
               .WithMessage("El acta seleccionado no existe.");

            RuleFor(x => x.IdAsociado)
              .NotNull().WithMessage("Debe seleccionar un Asociado.")
              .GreaterThan(0).WithMessage("Debe seleccionar un Asociado valido.")
               .MustAsync(async (id, cancellation) =>
               {
                   return await _context.TbAsociados.AnyAsync(a => a.IdAsociado == id);
               })
               .WithMessage("El Asociado seleccionada no existe.");   


            RuleFor(x => x.Nombre)
                .NotNull().WithMessage("Debe ingresar un Nombre de Proyecto.")
                .NotEmpty().WithMessage("Debe ingresar un Nombre de Proyecto.")
                .MaximumLength(100).WithMessage("El Nombre de Proyecto no puede superar los 100 caracteres.")
                 .MustAsync(async (nombre, cancellation) =>
                 {
                     return !await _context.TbProyectos.AnyAsync(a => a.Nombre == nombre);
                 })
                .WithMessage("Ya existe un Proyecto con el nombre seleccionado.");


            RuleFor(x => x.Descripcion)
                .NotNull().WithMessage("Debe ingresar una Descripcion.")
                .NotEmpty().WithMessage("Debe ingresar una Descripcion.")
                .MaximumLength(100).WithMessage("La Descripcion no puede superar los 100 caracteres.");

            RuleFor(x => x.MontoTotalDestinado)
                .GreaterThanOrEqualTo(0).WithMessage("El Monto Total Destinado debe ser un valor positivo.");

        }
    }
}