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

    public virtual DbSet<TbActividad> TbActividads { get; set; }

    public virtual DbSet<TbActum> TbActa { get; set; }

    public virtual DbSet<TbAcuerdo> TbAcuerdos { get; set; }

    public virtual DbSet<TbAsociacion> TbAsociacions { get; set; }

    public virtual DbSet<TbAsociado> TbAsociados { get; set; }

    public virtual DbSet<TbCheque> TbCheques { get; set; }

    public virtual DbSet<TbCliente> TbClientes { get; set; }

    public virtual DbSet<TbColaborador> TbColaboradors { get; set; }

    public virtual DbSet<TbConceptoAsociacion> TbConceptoAsociacions { get; set; }

    public virtual DbSet<TbConceptoMovimiento> TbConceptoMovimientos { get; set; }

    public virtual DbSet<TbCuentum> TbCuenta { get; set; }

    public virtual DbSet<TbDetalleChequeFactura> TbDetalleChequeFacturas { get; set; }

    public virtual DbSet<TbDetalleFactura> TbDetalleFacturas { get; set; }

    public virtual DbSet<TbDocumentoIngreso> TbDocumentoIngresos { get; set; }

    public virtual DbSet<TbFactura> TbFacturas { get; set; }

    public virtual DbSet<TbFinancistum> TbFinancista { get; set; }

    public virtual DbSet<TbFolio> TbFolios { get; set; }

    public virtual DbSet<TbFondosRecaudadosActividad> TbFondosRecaudadosActividads { get; set; }

    public virtual DbSet<TbJuntaDirectiva> TbJuntaDirectivas { get; set; }

    public virtual DbSet<TbMiembroJuntaDirectiva> TbMiembroJuntaDirectivas { get; set; }

    public virtual DbSet<TbMovimientoEgreso> TbMovimientoEgresos { get; set; }

    public virtual DbSet<TbMovimientoIngreso> TbMovimientoIngresos { get; set; }

    public virtual DbSet<TbProveedor> TbProveedors { get; set; }

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

        modelBuilder.Entity<TbActividad>(entity =>
        {
            entity.HasKey(e => e.IdActividad).HasName("PRIMARY");

            entity.ToTable("tb_actividad");

            entity.HasIndex(e => e.IdActa, "fk_tb_actividad_tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_actividad_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_actividad_tb_asociado");

            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.Lugar)
                .HasMaxLength(150)
                .HasColumnName("lugar");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Observaciones)
                .HasColumnType("text")
                .HasColumnName("observaciones");
            entity.Property(e => e.Razon)
                .HasMaxLength(255)
                .HasColumnName("razon");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbActividads)
                .HasForeignKey(d => d.IdActa)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_actividad_tb_acta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbActividads)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_actividad_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbActividads)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_actividad_tb_asociado");
        });

        modelBuilder.Entity<TbActum>(entity =>
        {
            entity.HasKey(e => e.IdActa).HasName("PRIMARY");

            entity.ToTable("tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_acta_tb_asociacion");

            entity.HasIndex(e => e.IdFolio, "fk_tb_acta_tb_folio");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_cuenta_tb_miembro_junta_directiva_asociado");

            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(320)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasColumnType("enum('Cerrado','Inactivo','EnProceso')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaSesion).HasColumnName("fecha_sesion");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.IdFolio).HasColumnName("id_folio");
            entity.Property(e => e.NumeroActa)
                .HasMaxLength(30)
                .HasColumnName("numero_acta");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('Ordinario','Extraordinaria')")
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbActa)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_acta_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbActa)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_acta_tb_asociado");

            entity.HasOne(d => d.IdFolioNavigation).WithMany(p => p.TbActa)
                .HasForeignKey(d => d.IdFolio)
                .HasConstraintName("fk_tb_acta_tb_folio");
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
                .HasPrecision(15, 2)
                .HasColumnName("monto_acuerdo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroAcuerdo)
                .HasMaxLength(100)
                .HasColumnName("numero_acuerdo");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('Compra','Pago','Ordinario')")
                .HasColumnName("tipo");

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
                .HasColumnType("enum('Soltero','Casado','Divorciado','Viudo','UnionLibre')")
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

        modelBuilder.Entity<TbCheque>(entity =>
        {
            entity.HasKey(e => e.IdCheque).HasName("PRIMARY");

            entity.ToTable("tb_cheque");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_cheque_tb_asociacion");

            entity.HasIndex(e => e.IdAsociadoAutoriza, "fk_tb_cheque_tb_asociado");

            entity.HasIndex(e => e.IdCuenta, "fk_tb_cheque_tb_cuenta");

            entity.HasIndex(e => e.NumeroCheque, "numero_cheque").IsUnique();

            entity.Property(e => e.IdCheque).HasColumnName("id_cheque");
            entity.Property(e => e.Beneficiario)
                .HasMaxLength(100)
                .HasColumnName("beneficiario");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Emitido'")
                .HasColumnType("enum('Emitido','UsadoParcial','UsadoTotal','Anulado','Vencido')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAnulacion).HasColumnName("fecha_anulacion");
            entity.Property(e => e.FechaCobro).HasColumnName("fecha_cobro");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociadoAutoriza).HasColumnName("id_asociado_autoriza");
            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");
            entity.Property(e => e.MontoRestante)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("monto_restante");
            entity.Property(e => e.NumeroCheque)
                .HasMaxLength(20)
                .HasColumnName("numero_cheque");
            entity.Property(e => e.Observaciones)
                .HasColumnType("text")
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbCheques)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_cheque_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoAutorizaNavigation).WithMany(p => p.TbCheques)
                .HasForeignKey(d => d.IdAsociadoAutoriza)
                .HasConstraintName("fk_tb_cheque_tb_asociado");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.TbCheques)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_cheque_tb_cuenta");
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

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbClientes)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_cliente_tb_asociacion");
        });

        modelBuilder.Entity<TbColaborador>(entity =>
        {
            entity.HasKey(e => e.IdColaborador).HasName("PRIMARY");

            entity.ToTable("tb_colaborador");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_colaborador_tb_asociacion");

            entity.Property(e => e.IdColaborador).HasColumnName("id_colaborador");
            entity.Property(e => e.Cedula)
                .HasMaxLength(20)
                .HasColumnName("cedula");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Observaciones)
                .HasColumnType("text")
                .HasColumnName("observaciones");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(100)
                .HasColumnName("apellido_1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(100)
                .HasColumnName("apellido_2");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbColaboradors)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_colaborador_tb_asociacion");
        });

        modelBuilder.Entity<TbConceptoAsociacion>(entity =>
        {
            entity.HasKey(e => e.IdConceptoAsociacion).HasName("PRIMARY");

            entity.ToTable("tb_concepto_asociacion");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_concepto_asociacion_tb_asociacion");

            entity.HasIndex(e => e.IdConcepto, "fk_tb_concepto_asociacion_tb_concepto");

            entity.Property(e => e.IdConceptoAsociacion).HasColumnName("id_concepto_asociacion");
            entity.Property(e => e.DescripcionPersonalizada)
                .HasMaxLength(100)
                .HasColumnName("descripcion_personalizada");
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
            entity.Property(e => e.TipoEmisorEgreso)
                .HasColumnType("enum('proveedor','colaborador','asociado')")
                .HasColumnName("tipo_emisor_egreso");
            entity.Property(e => e.TipoMovimiento)
                .HasColumnType("enum('Ingreso','Egreso')")
                .HasColumnName("tipoMovimiento");
            entity.Property(e => e.TipoOrigenIngreso)
                .HasColumnType("enum('donante','actividad','financista')")
                .HasColumnName("tipo_origen_ingreso");
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
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(30)
                .HasColumnName("numero_cuenta");
            entity.Property(e => e.Saldo)
                .HasPrecision(15, 2)
                .HasColumnName("saldo");
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

        modelBuilder.Entity<TbDetalleChequeFactura>(entity =>
        {
            entity.HasKey(e => e.IdDetalleChequeFactura).HasName("PRIMARY");

            entity.ToTable("tb_detalle_cheque_factura");

            entity.HasIndex(e => e.IdAcuerdo, "fk_tb_detalle_cheque_factura_tb_acuerdo");

            entity.HasIndex(e => e.IdCheque, "fk_tb_detalle_cheque_factura_tb_cheque");

            entity.HasIndex(e => e.IdFactura, "fk_tb_detalle_cheque_factura_tb_factura");

            entity.HasIndex(e => e.IdMovimientoEgreso, "fk_tb_detalle_cheque_factura_tb_movimiento_egreso");

            entity.Property(e => e.IdDetalleChequeFactura).HasColumnName("id_detalle_cheque_factura");
            entity.Property(e => e.IdAcuerdo).HasColumnName("id_acuerdo");
            entity.Property(e => e.IdCheque).HasColumnName("id_cheque");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.IdMovimientoEgreso).HasColumnName("id_movimiento_egreso");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");
            entity.Property(e => e.Observacion)
                .HasColumnType("text")
                .HasColumnName("observacion");

            entity.HasOne(d => d.IdAcuerdoNavigation).WithMany(p => p.TbDetalleChequeFacturas)
                .HasForeignKey(d => d.IdAcuerdo)
                .HasConstraintName("fk_tb_detalle_cheque_factura_tb_acuerdo");

            entity.HasOne(d => d.IdChequeNavigation).WithMany(p => p.TbDetalleChequeFacturas)
                .HasForeignKey(d => d.IdCheque)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_detalle_cheque_factura_tb_cheque");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.TbDetalleChequeFacturas)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_detalle_cheque_factura_tb_factura");

            entity.HasOne(d => d.IdMovimientoEgresoNavigation).WithMany(p => p.TbDetalleChequeFacturas)
                .HasForeignKey(d => d.IdMovimientoEgreso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_detalle_cheque_factura_tb_movimiento_egreso");
        });

        modelBuilder.Entity<TbDetalleFactura>(entity =>
        {
            entity.HasKey(e => e.IdDetalleFactura).HasName("PRIMARY");

            entity.ToTable("tb_detalle_factura");

            entity.HasIndex(e => e.IdFactura, "fk_tb_detalle_factura");

            entity.Property(e => e.IdDetalleFactura).HasColumnName("id_detalle_factura");
            entity.Property(e => e.BaseImponible)
                .HasPrecision(15, 2)
                .HasColumnName("base_imponible");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(15, 2)
                .HasColumnName("descuento");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.MontoIva)
                .HasPrecision(15, 2)
                .HasColumnName("monto_iva");
            entity.Property(e => e.PorcentajeDescuento)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje_descuento");
            entity.Property(e => e.PorcentajeIva)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje_iva");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(15, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.TotalLinea)
                .HasPrecision(15, 2)
                .HasColumnName("total_linea");
            entity.Property(e => e.Unidad)
                .HasMaxLength(50)
                .HasColumnName("unidad");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.TbDetalleFacturas)
                .HasForeignKey(d => d.IdFactura)
                .HasConstraintName("fk_tb_detalle_factura");
        });

        modelBuilder.Entity<TbDocumentoIngreso>(entity =>
        {
            entity.HasKey(e => e.IdDocumentoIngreso).HasName("PRIMARY");

            entity.ToTable("tb_documento_ingreso");

            entity.HasIndex(e => e.IdActividad, "fk_tb_documento_ingreso_tb_actividad");

            entity.HasIndex(e => e.IdCliente, "fk_tb_documento_ingreso_tb_cliente");

            entity.HasIndex(e => e.IdConceptoAsociacion, "fk_tb_documento_ingreso_tb_concepto_asociacion");

            entity.HasIndex(e => e.IdCuenta, "fk_tb_documento_ingreso_tb_cuenta");

            entity.HasIndex(e => e.IdFinancista, "fk_tb_documento_ingreso_tb_financista");

            entity.HasIndex(e => e.IdMovimientoIngreso, "fk_tb_documento_ingreso_tb_movimiento_ingreso");

            entity.Property(e => e.IdDocumentoIngreso).HasColumnName("id_documento_ingreso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaComprobante).HasColumnName("fecha_comprobante");
            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdConceptoAsociacion).HasColumnName("id_concepto_asociacion");
            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.IdFinancista).HasColumnName("id_financista");
            entity.Property(e => e.IdMovimientoIngreso).HasColumnName("id_movimiento_ingreso");
            entity.Property(e => e.MetodoPago)
                .HasColumnType("enum('Efectivo','Sinpe','Transferencia')")
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");
            entity.Property(e => e.NumComprobante)
                .HasMaxLength(12)
                .HasColumnName("num_comprobante");

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.TbDocumentoIngresos)
                .HasForeignKey(d => d.IdActividad)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_documento_ingreso_tb_actividad");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.TbDocumentoIngresos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_documento_ingreso_tb_cliente");

            entity.HasOne(d => d.IdConceptoAsociacionNavigation).WithMany(p => p.TbDocumentoIngresos)
                .HasForeignKey(d => d.IdConceptoAsociacion)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_documento_ingreso_tb_concepto_asociacion");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.TbDocumentoIngresos)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_documento_ingreso_tb_cuenta");

            entity.HasOne(d => d.IdFinancistaNavigation).WithMany(p => p.TbDocumentoIngresos)
                .HasForeignKey(d => d.IdFinancista)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_documento_ingreso_tb_financista");

            entity.HasOne(d => d.IdMovimientoIngresoNavigation).WithMany(p => p.TbDocumentoIngresos)
                .HasForeignKey(d => d.IdMovimientoIngreso)
                .HasConstraintName("fk_tb_documento_ingreso_tb_movimiento_ingreso");
        });

        modelBuilder.Entity<TbFactura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PRIMARY");

            entity.ToTable("tb_factura");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_factura_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_factura_tb_asociado");

            entity.HasIndex(e => e.IdColaborador, "fk_tb_factura_tb_colaborador");

            entity.HasIndex(e => e.IdProveedor, "fk_tb_factura_tb_proveedor");

            entity.HasIndex(e => e.IdConceptoAsociacion, "fk_tb_movimiento_egreso_tb_concepto_asociacion");

            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.ArchivoUrl)
                .HasMaxLength(255)
                .HasColumnName("archivo_url");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Pendiente'")
                .HasColumnType("enum('Pendiente','Pagada','Rechazada')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.FechaSubida)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_subida");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.IdColaborador).HasColumnName("id_colaborador");
            entity.Property(e => e.IdConceptoAsociacion).HasColumnName("id_concepto_asociacion");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.MontoTotal)
                .HasPrecision(15, 2)
                .HasColumnName("monto_total");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(255)
                .HasColumnName("nombre_archivo");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .HasColumnName("numero_factura");
            entity.Property(e => e.Subtotal)
                .HasPrecision(15, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.TotalIva)
                .HasPrecision(15, 2)
                .HasColumnName("total_iva");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbFacturas)
                .HasForeignKey(d => d.IdAsociacion)
                .HasConstraintName("fk_tb_factura_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbFacturas)
                .HasForeignKey(d => d.IdAsociado)
                .HasConstraintName("fk_tb_factura_tb_asociado");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.TbFacturas)
                .HasForeignKey(d => d.IdColaborador)
                .HasConstraintName("fk_tb_factura_tb_colaborador");

            entity.HasOne(d => d.IdConceptoAsociacionNavigation).WithMany(p => p.TbFacturas)
                .HasForeignKey(d => d.IdConceptoAsociacion)
                .HasConstraintName("fk_tb_movimiento_egreso_tb_concepto_asociacion");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.TbFacturas)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("fk_tb_factura_tb_proveedor");
        });

        modelBuilder.Entity<TbFinancistum>(entity =>
        {
            entity.HasKey(e => e.IdFinancista).HasName("PRIMARY");

            entity.ToTable("tb_financista");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_financista_tb_asociacion");

            entity.Property(e => e.IdFinancista).HasColumnName("id_financista");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.SitioWeb)
                .HasMaxLength(150)
                .HasColumnName("sitio_web");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoEntidad)
                .HasColumnType("enum('publica','privada')")
                .HasColumnName("tipo_entidad");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbFinancista)
                .HasForeignKey(d => d.IdAsociacion)
                .HasConstraintName("fk_tb_financista_tb_asociacion");
        });

        modelBuilder.Entity<TbFolio>(entity =>
        {
            entity.HasKey(e => e.IdFolio).HasName("PRIMARY");

            entity.ToTable("tb_folio");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_folio_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_folio_tb_asociado");

            entity.Property(e => e.IdFolio).HasColumnName("id_folio");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Anulado','Cerrado')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.NumeroFolio)
                .HasMaxLength(100)
                .HasColumnName("numero_folio");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbFolios)
                .HasForeignKey(d => d.IdAsociacion)
                .HasConstraintName("fk_tb_folio_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbFolios)
                .HasForeignKey(d => d.IdAsociado)
                .HasConstraintName("fk_tb_folio_tb_asociado");
        });

        modelBuilder.Entity<TbFondosRecaudadosActividad>(entity =>
        {
            entity.HasKey(e => e.IdFondosRecaudadosActividad).HasName("PRIMARY");

            entity.ToTable("tb_fondos_recaudados_actividad");

            entity.HasIndex(e => e.IdActividad, "fk_tb_fondos_recaudados_actividad");

            entity.Property(e => e.IdFondosRecaudadosActividad).HasColumnName("id_fondos_recaudados_actividad");
            entity.Property(e => e.Detalle)
                .HasColumnType("text")
                .HasColumnName("detalle");
            entity.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");
            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.TbFondosRecaudadosActividads)
                .HasForeignKey(d => d.IdActividad)
                .HasConstraintName("fk_tb_fondos_recaudados_actividad");
        });

        modelBuilder.Entity<TbJuntaDirectiva>(entity =>
        {
            entity.HasKey(e => e.IdJuntaDirectiva).HasName("PRIMARY");

            entity.ToTable("tb_junta_directiva");

            entity.HasIndex(e => e.IdActa, "fk_tb_junta_directiva_tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_junta_directiva_tb_asociacion");

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

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbJuntaDirectivas)
                .HasForeignKey(d => d.IdAsociacion)
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

        modelBuilder.Entity<TbMovimientoEgreso>(entity =>
        {
            entity.HasKey(e => e.IdMovimientoEgreso).HasName("PRIMARY");

            entity.ToTable("tb_movimiento_egreso");

            entity.HasIndex(e => e.IdActa, "fk_tb_movimiento_egreso_tb_acta");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_movimiento_egreso_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_movimiento_egreso_tb_asociado");

            entity.Property(e => e.IdMovimientoEgreso).HasColumnName("id_movimiento_egreso");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.IdActa).HasColumnName("id_acta");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");

            entity.HasOne(d => d.IdActaNavigation).WithMany(p => p.TbMovimientoEgresos)
                .HasForeignKey(d => d.IdActa)
                .HasConstraintName("fk_tb_movimiento_egreso_tb_acta");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbMovimientoEgresos)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_movimiento_egreso_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbMovimientoEgresos)
                .HasForeignKey(d => d.IdAsociado)
                .HasConstraintName("fk_tb_movimiento_egreso_tb_asociado");
        });

        modelBuilder.Entity<TbMovimientoIngreso>(entity =>
        {
            entity.HasKey(e => e.IdMovimientoIngreso).HasName("PRIMARY");

            entity.ToTable("tb_movimiento_ingreso");

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_movimiento_ingreso_tb_asociacion");

            entity.HasIndex(e => e.IdAsociado, "fk_tb_movimiento_ingreso_tb_asociado");

            entity.Property(e => e.IdMovimientoIngreso).HasColumnName("id_movimiento_ingreso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.IdAsociado).HasColumnName("id_asociado");
            entity.Property(e => e.MontoTotalIngresado)
                .HasPrecision(15, 2)
                .HasColumnName("monto_total_ingresado");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbMovimientoIngresos)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_movimiento_ingreso_tb_asociacion");

            entity.HasOne(d => d.IdAsociadoNavigation).WithMany(p => p.TbMovimientoIngresos)
                .HasForeignKey(d => d.IdAsociado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tb_movimiento_ingreso_tb_asociado");
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
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
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

            entity.HasIndex(e => e.IdAsociacion, "fk_tb_usuario_tb_asociacion");

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
            entity.Property(e => e.IdAsociacion).HasColumnName("id_asociacion");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.Rol)
                .HasDefaultValueSql("'Admin'")
                .HasColumnType("enum('Admin','root')")
                .HasColumnName("rol");

            entity.HasOne(d => d.IdAsociacionNavigation).WithMany(p => p.TbUsuarios)
                .HasForeignKey(d => d.IdAsociacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_tb_usuario_tb_asociacion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
