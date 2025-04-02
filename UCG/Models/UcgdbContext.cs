using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UCG.Models;

public partial class UcgdbContext : DbContext
{
    public UcgdbContext()
    {
    }

    public UcgdbContext(DbContextOptions<UcgdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbActaAsistencium> TbActaAsistencia { get; set; }

    public virtual DbSet<TbActum> TbActa { get; set; }

    public virtual DbSet<TbAcuerdo> TbAcuerdos { get; set; }

    public virtual DbSet<TbAsociacion> TbAsociacions { get; set; }

    public virtual DbSet<TbAsociado> TbAsociados { get; set; }

    public virtual DbSet<TbCategoriaMovimiento> TbCategoriaMovimientos { get; set; }

    public virtual DbSet<TbCliente> TbClientes { get; set; }

    public virtual DbSet<TbConceptoAsociacion> TbConceptoAsociacions { get; set; }

    public virtual DbSet<TbConceptoMovimiento> TbConceptoMovimientos { get; set; }

    public virtual DbSet<TbCuentum> TbCuenta { get; set; }

    public virtual DbSet<TbDetalleMovimiento> TbDetalleMovimientos { get; set; }

    public virtual DbSet<TbJuntaDirectiva> TbJuntaDirectivas { get; set; }

    public virtual DbSet<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; set; }

    public virtual DbSet<TbMovimiento> TbMovimientos { get; set; }

    public virtual DbSet<TbProveedor> TbProveedors { get; set; }

    public virtual DbSet<TbProyecto> TbProyectos { get; set; }

    public virtual DbSet<TbPuesto> TbPuestos { get; set; }

    public virtual DbSet<TbUsuario> TbUsuarios { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<TbActaAsistencium>(entity =>
        {
            entity.HasKey(e => e.IdActaAsistencia).HasName("PRIMARY");

            entity.ToTable("tb_acta_asistencia");

            entity.HasIndex(e => e.IdActa, "fk_tb_acta_asistencia_tb_acta");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_acta_asistencia_tb_asociado");

            entity.Property(e => e.IdActaAsistencia).HasColumnName("id_acta_asistencia");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbActaAsistencias)
                .HasForeignKey(d => d.IdActa)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_acta_asistencia_tb_acta");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbActaAsistencia)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_acta_asistencia_tb_asociado");
        });

        modelBuilder.Entity<TbActum>(entity =>
        {
            entity.HasKey(e => e.IdActa).HasName("PRIMARY");

            entity.ToTable("tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_acta_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_acta_tb_asociado");

            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(320)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasColumnType("enum('Cerrado','Inactivo','En Proceso')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaSesion).HasColumnName("fecha_sesion");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.MontoTotalAcordado)
                .HasPrecision(10, 2)
                .HasColumnName("monto_total_acordado");
            entity.Property(e => e.NumeroActa)
                .HasMaxLength(20)
                .HasColumnName("numero_acta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbActa)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_acta_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbActas)
                .HasForeignKey(d =>d.IdAsociado)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_acta_tb_asociado");
        });

        modelBuilder.Entity<TbAcuerdo>(entity =>
        {
            entity.HasKey(e => e.IdAcuerdo).HasName("PRIMARY");

            entity.ToTable("tb_acuerdo");

            entity.HasIndex(e => e.IdActa, "fk_tb_acuerdo_tb_acta");

            entity.Property(e => e.IdAcuerdo).HasColumnName("id_acuerdo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.MontoAcuerdo)
                .HasPrecision(10, 2)
                .HasColumnName("monto_acuerdo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroAcuerdo)
                .HasMaxLength(100)
                .HasColumnName("numero_acuerdo");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbAcuerdos)
                .HasForeignKey(d => d.IdActa)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_acuerdo_tb_acta");
        });

        modelBuilder.Entity<TbAsociacion>(entity =>
        {
            entity.HasKey(e => e.IdAsociacion).HasName("PRIMARY");

            entity.ToTable("tb_asociacion");

            entity.HasIndex(e => e.CedulaJuridica, "cedula_juridica").IsUnique();

            entity.HasIndex(e => e.CodigoRegistro, "codigo_registro").IsUnique();

            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.Canton)
                .HasMaxLength(50)
                .HasColumnName("canton");
            entity.Property(e => e.CedulaJuridica)
                .HasMaxLength(40)
                .HasColumnName("cedula_juridica");
            entity.Property(e => e.CodigoRegistro)
                .HasMaxLength(40)
                .HasColumnName("codigo_registro");
            entity.Property(e => e.Correo)
                .HasMaxLength(120)
                .HasColumnName("correo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Distrito)
                .HasMaxLength(50)
                .HasColumnName("distrito");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactio')")
                .HasColumnName("estado");
            entity.Property(e => e.Fax)
                .HasMaxLength(30)
                .HasColumnName("fax");
            entity.Property(e => e.FechaConstitucion).HasColumnName("fecha_constitucion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.Provincia)
                .HasMaxLength(50)
                .HasColumnName("provincia");
            entity.Property(e => e.Pueblo)
                .HasMaxLength(50)
                .HasColumnName("pueblo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(30)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<TbAsociado>(entity =>
        {
            entity.HasKey(e => e.IdAsociado).HasName("PRIMARY");

            entity.ToTable("tb_asociado");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_asociado_tb_asociacion");

            entity.HasIndex(e => e.IdUsuario, "fk_tb_asociado_tb_usuario");

            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(30)
                .HasColumnName("apellido_1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(30)
                .HasColumnName("apellido_2");
            entity.Property(e => e.Cedula)
                .HasMaxLength(12)
                .HasColumnName("cedula");
            entity.Property(e => e.Correo)
                .HasMaxLength(120)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("estado");
            entity.Property(e => e.EstadoCivil)
                .HasColumnType("enum('Soltero(a)','Casado(a)','Divorciado(a)','Viudo(a)','Union libre')")
                .HasColumnName("estado_civil");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nacionalidad)
                .HasColumnType("enum('Nacional','Residente','Extranjero')")
                .HasColumnName("nacionalidad");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
            entity.Property(e => e.Sexo)
                .HasColumnType("enum('Femenino','Masculino','Otro')")
                .HasColumnName("sexo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(25)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbAsociados)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_asociado_tb_asociacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TbAsociados)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_asociado_tb_usuario");
        });

        modelBuilder.Entity<TbCategoriaMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaMovimiento).HasName("PRIMARY");

            entity.ToTable("tb_categoria_movimiento");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_categoria_movimiento_tb_asociado");

            entity.HasIndex(e => e.IdConceptoAsociacion, "fk_tb_categoria_movimiento_tb_concepto_asociacion");

            entity.Property(e => e.IdCategoriaMovimiento).HasColumnName("id_categoria_movimiento");
            entity.Property(e => e.DescripcionCategoria)
                .HasMaxLength(100)
                .HasColumnName("descripcion_categoria");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.IdConceptoAsociacion).HasColumnName("id_concepto_asociacion");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(100)
                .HasColumnName("nombre_categoria");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbCategoriaMovimientos)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_categoria_movimiento_tb_asociado");

            entity.HasOne(d => d.IdConceptoAsociacionNavigation).WithMany(p => p.TbCategoriaMovimientos)
                .HasForeignKey(d => d.IdConceptoAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_categoria_movimiento_tb_concepto_asociacion");
        });

        modelBuilder.Entity<TbCliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PRIMARY");

            entity.ToTable("tb_cliente");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_cliente_tb_asociacion");

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .HasColumnName("apellido_1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .HasColumnName("apellido_2");
            entity.Property(e => e.Cedula)
                .HasMaxLength(50)
                .HasColumnName("cedula");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("estado");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(25)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoCliente)
                .HasColumnType("enum('Donante','Colaborador','Cliente')")
                .HasColumnName("tipo_cliente");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbClientes)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_cliente_tb_asociacion");
        });

        modelBuilder.Entity<TbConceptoAsociacion>(entity =>
        {
            entity.HasKey(e => e.IdConceptoAsociacion).HasName("PRIMARY");

            entity.ToTable("tb_concepto_asociacion");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_concepto_asociacion_tb_asociacion");

            entity.HasIndex(e => e.IdConcepto, "fk_tb_concepto_asociacion_tb_concepto");

            entity.Property(e => e.IdConceptoAsociacion).HasColumnName("id_concepto_asociacion");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdConcepto).HasColumnName("id_concepto");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbConceptoAsociacions)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_concepto_asociacion_tb_asociacion");

            entity.HasOne(d => d.IdConceptoNavigation).WithMany(p => p.TbConceptoAsociacions)
                .HasForeignKey(d => d.IdConcepto)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_concepto_asociacion_tb_concepto");
        });

        modelBuilder.Entity<TbConceptoMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdConceptoMovimiento).HasName("PRIMARY");

            entity.ToTable("tb_concepto_movimiento");

            entity.Property(e => e.IdConceptoMovimiento).HasColumnName("id_concepto_movimiento");
            entity.Property(e => e.Concepto)
                .HasMaxLength(100)
                .HasColumnName("concepto");
            entity.Property(e => e.TipoMovimiento)
                .HasColumnType("enum('Ingreso','Egreso')")
                .HasColumnName("tipoMovimiento");
        });

        modelBuilder.Entity<TbCuentum>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PRIMARY");

            entity.ToTable("tb_cuenta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_cuenta_id_asociacion");

            entity.HasIndex(e => e.NumeroCuenta, "numero_cuenta").IsUnique();

            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.Banco).HasColumnType("enum('BN','BP','BCR','BAC')");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("estado");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.NumeroCuenta).HasColumnName("numero_cuenta");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoCuenta)
                .HasColumnType("enum('Ahorro','Credito','Debito')")
                .HasColumnName("tipo_cuenta");
            entity.Property(e => e.TituloCuenta)
                .HasMaxLength(50)
                .HasColumnName("titulo_cuenta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbCuenta)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_cuenta_id_asociacion");
        });

        modelBuilder.Entity<TbDetalleMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdDetalleMovimiento).HasName("PRIMARY");

            entity.ToTable("tb_detalle_movimiento");

            entity.HasIndex(e => e.IdAcuerdo, "fk_tb_detalle_movimiento_tb_acuerdo");

            entity.HasIndex(e => e.IdMovimiento, "fk_tb_detalle_movimiento_tb_movimiento");

            entity.Property(e => e.IdDetalleMovimiento).HasColumnName("id_detalle_movimiento");
            entity.Property(e => e.Decripcion)
                .HasMaxLength(300)
                .HasColumnName("decripcion");
            entity.Property(e => e.IdAcuerdo).HasColumnName("id_acuerdo");
            entity.Property(e => e.IdInformeEconomico).HasColumnName("id_informe_economico");
            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.TipoMovimiento)
                .HasColumnType("enum('Ingresos','Egresos')")
                .HasColumnName("tipo_movimiento");

            entity.HasOne(d => d.IdAcuerdoNavigation).WithMany(p => p.TbDetalleMovimientos)
                .HasForeignKey(d => d.IdAcuerdo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_detalle_movimiento_tb_acuerdo");

            entity.HasOne(d => d.IdMovimientoNavigation).WithMany(p => p.TbDetalleMovimientos)
                .HasForeignKey(d => d.IdMovimiento)
                .HasConstraintName("fk_tb_detalle_movimiento_tb_movimiento");
        });

        modelBuilder.Entity<TbJuntaDirectiva>(entity =>
        {
            entity.HasKey(e => e.IdJuntaDirectiva).HasName("PRIMARY");

            entity.ToTable("tb_junta_directiva");

            entity.HasIndex(e => e.IdActa, "fk_tb_junta_directiva_tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "id_asociacion").IsUnique();

            entity.Property(e => e.IdJuntaDirectiva).HasColumnName("id_junta_directiva");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("estado");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PeriodoFin).HasColumnName("periodo_fin");
            entity.Property(e => e.PeriodoInicio).HasColumnName("periodo_inicio");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbJuntaDirectivas)
                .HasForeignKey(d => d.IdActa)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_junta_directiva_tb_acta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithOne(p => p.TbJuntaDirectiva)
                .HasForeignKey<TbJuntaDirectiva>(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_junta_directiva_tb_asociacion");
        });

        modelBuilder.Entity<TbMiembroJuntaDirectiva>(entity =>
        {
            entity.HasKey(e => e.IdMiembrosJuntaDirectiva).HasName("PRIMARY");

            entity.ToTable("tb_miembro_junta_directiva");

            entity.HasIndex(e => e.IdJuntaDirectiva, "fk_tb_miembro_junta_directiva_tb_junta_directiva");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_miembros_junta_directiva_tb_asociado");

            entity.HasIndex(e => e.IdPuesto, "fk_tb_miembros_junta_directiva_tb_puesto");

            entity.Property(e => e.IdMiembrosJuntaDirectiva).HasColumnName("id_miembros_junta_directiva");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("estado");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.IdJuntaDirectiva).HasColumnName("id_junta_directiva");
            entity.Property(e => e.IdPuesto).HasColumnName("id_puesto");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbMiembroJuntaDirectivas)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_miembros_junta_directiva_tb_asociado");

            entity.HasOne(d => d.IdJuntaDirectivaNavigation).WithMany(p => p.TbMiembroJuntaDirectivas)
                .HasForeignKey(d => d.IdJuntaDirectiva)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_miembro_junta_directiva_tb_junta_directiva");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.TbMiembroJuntaDirectivas)
                .HasForeignKey(d => d.IdPuesto)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_miembros_junta_directiva_tb_puesto");
        });

        modelBuilder.Entity<TbMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PRIMARY");

            entity.ToTable("tb_movimiento");

            entity.HasIndex(e => e.IdActa, "fk_tb_movimiento_tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_movimiento_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_movimiento_tb_asociado");

            entity.HasIndex(e => e.IdCategoriaMovimiento, "fk_tb_movimiento_tb_categoria_movimiento");

            entity.HasIndex(e => e.IdConcepto, "fk_tb_movimiento_tb_concepto_movimiento");

            entity.HasIndex(e => e.IdCuenta, "fk_tb_movimiento_tb_cuenta");

            entity.HasIndex(e => e.IdProveedor, "fk_tb_movimiento_tb_proveedor");

            entity.HasIndex(e => e.IdProyecto, "fk_tb_movimiento_tb_proyecto");

            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasColumnType("enum('Procesado','Inactivo','EnProceso')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");
            entity.Property(e => e.FuenteFondo)
                .HasColumnType("enum('FondosPropios','Aporte2Dinadeco')")
                .HasColumnName("fuente_fondo");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.IdCategoriaMovimiento).HasColumnName("id_categoria_movimiento");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdConcepto).HasColumnName("id_concepto");
            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");
            entity.Property(e => e.MetdodoPago)
                .HasColumnType("enum('Transferencia','SinpeMovil','Cheque','Efectivo')")
                .HasColumnName("metdodo_pago");
            entity.Property(e => e.MontoTotalMovido)
                .HasPrecision(10, 2)
                .HasColumnName("monto_total_movido");
            entity.Property(e => e.SubtotalMovido)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal_movido");
            entity.Property(e => e.TipoMovimiento)
                .HasColumnType("enum('Ingresos','Egresos')")
                .HasColumnName("tipo_movimiento");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdActa)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_tb_acta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdAsociacion)
                .HasConstraintName("fk_tb_movimiento_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_movimiento_tb_asociado");

            entity.HasOne(d => d.IdCategoriaMovimientoNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdCategoriaMovimiento)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_tb_categoria_movimiento");

            entity.HasOne(d => d.IdConceptoNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdConcepto)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_tb_concepto_movimiento");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_tb_cuenta");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_tb_proveedor");

            entity.HasOne(d => d.IdProyectoNavigation).WithMany(p => p.TbMovimientos)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_tb_proyecto");
        });

        modelBuilder.Entity<TbProveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PRIMARY");

            entity.ToTable("tb_proveedor");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_proveedor_tb_asociacion");

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.CedulaContacto)
                .HasMaxLength(50)
                .HasColumnName("cedula_contacto");
            entity.Property(e => e.CedulaJuridica)
                .HasMaxLength(50)
                .HasColumnName("cedula_juridica");
            entity.Property(e => e.Correo)
                .HasMaxLength(120)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(120)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactio')")
                .HasColumnName("estado");
            entity.Property(e => e.Fax)
                .HasMaxLength(60)
                .HasColumnName("fax");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.NombreContacto)
                .HasMaxLength(50)
                .HasColumnName("nombre_contacto");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(50)
                .HasColumnName("nombre_empresa");
            entity.Property(e => e.Telefono)
                .HasMaxLength(25)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoProveedor)
                .HasDefaultValueSql("'Juridico'")
                .HasColumnType("enum('Fisico','Juridico')")
                .HasColumnName("tipo_proveedor");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbProveedors)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_proveedor_tb_asociacion");
        });

        modelBuilder.Entity<TbProyecto>(entity =>
        {
            entity.HasKey(e => e.IdProyecto).HasName("PRIMARY");

            entity.ToTable("tb_proyecto");

            entity.HasIndex(e => e.IdActa, "fk_tb_proyecto_tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_proyecto_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_proyecto_tb_asociado");

            entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.MontoTotalDestinado)
                .HasPrecision(10, 2)
                .HasColumnName("monto_total_destinado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbProyectos)
                .HasForeignKey(d => d.IdActa)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_proyecto_tb_acta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbProyectos)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_proyecto_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbProyectos)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_proyecto_tb_asociado");
        });

        modelBuilder.Entity<TbPuesto>(entity =>
        {
            entity.HasKey(e => e.IdPuesto).HasName("PRIMARY");

            entity.ToTable("tb_puesto");

            entity.Property(e => e.IdPuesto).HasColumnName("id_puesto");
            entity.Property(e => e.Decripcion)
                .HasMaxLength(100)
                .HasColumnName("decripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TbUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("tb_usuario");

            entity.HasIndex(e => e.Correo, "correo").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "nombre_usuario").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(128)
                .HasColumnName("contraseña");
            entity.Property(e => e.Correo).HasColumnName("correo");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("estado");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.Rol)
                .HasDefaultValueSql("'Admin'")
                .HasColumnType("enum('Admin','root')")
                .HasColumnName("rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
