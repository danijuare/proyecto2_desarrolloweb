using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccionesReparacion> AccionesReparacions { get; set; }

    public virtual DbSet<CategoriasRepuesto> CategoriasRepuestos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public virtual DbSet<Diagnostico> Diagnosticos { get; set; }

    public virtual DbSet<EstadosAccion> EstadosAccions { get; set; }

    public virtual DbSet<EstadosOrden> EstadosOrdens { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Mecanico> Mecanicos { get; set; }

    public virtual DbSet<MovimientosInventario> MovimientosInventarios { get; set; }

    public virtual DbSet<OrdenesTrabajo> OrdenesTrabajos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Repuesto> Repuestos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TokensSesion> TokensSesions { get; set; }

    public virtual DbSet<UsoRepuesto> UsoRepuestos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosPermiso> UsuariosPermisos { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=proyecto_2_desarrollo_web;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.11-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AccionesReparacion>(entity =>
        {
            entity.HasKey(e => e.IdAccion).HasName("PRIMARY");

            entity.Property(e => e.CostoManoObra).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.OrdenEjecucion).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.IdDiagnosticoNavigation).WithMany(p => p.AccionesReparacions).HasConstraintName("acciones_reparacion_ibfk_1");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.AccionesReparacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acciones_reparacion_ibfk_2");

            entity.HasOne(d => d.IdUsuarioAutorizaNavigation).WithMany(p => p.AccionesReparacions).HasConstraintName("acciones_reparacion_ibfk_3");

            entity.HasMany(d => d.IdAccionPrerequisitos).WithMany(p => p.IdAccions)
                .UsingEntity<Dictionary<string, object>>(
                    "DependenciasAccion",
                    r => r.HasOne<AccionesReparacion>().WithMany()
                        .HasForeignKey("IdAccionPrerequisito")
                        .HasConstraintName("dependencias_accion_ibfk_2"),
                    l => l.HasOne<AccionesReparacion>().WithMany()
                        .HasForeignKey("IdAccion")
                        .HasConstraintName("dependencias_accion_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdAccion", "IdAccionPrerequisito")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("dependencias_accion");
                        j.HasIndex(new[] { "IdAccionPrerequisito" }, "id_accion_prerequisito");
                        j.IndexerProperty<int>("IdAccion")
                            .HasColumnType("int(11)")
                            .HasColumnName("id_accion");
                        j.IndexerProperty<int>("IdAccionPrerequisito")
                            .HasColumnType("int(11)")
                            .HasColumnName("id_accion_prerequisito");
                    });

            entity.HasMany(d => d.IdAccions).WithMany(p => p.IdAccionPrerequisitos)
                .UsingEntity<Dictionary<string, object>>(
                    "DependenciasAccion",
                    r => r.HasOne<AccionesReparacion>().WithMany()
                        .HasForeignKey("IdAccion")
                        .HasConstraintName("dependencias_accion_ibfk_1"),
                    l => l.HasOne<AccionesReparacion>().WithMany()
                        .HasForeignKey("IdAccionPrerequisito")
                        .HasConstraintName("dependencias_accion_ibfk_2"),
                    j =>
                    {
                        j.HasKey("IdAccion", "IdAccionPrerequisito")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("dependencias_accion");
                        j.HasIndex(new[] { "IdAccionPrerequisito" }, "id_accion_prerequisito");
                        j.IndexerProperty<int>("IdAccion")
                            .HasColumnType("int(11)")
                            .HasColumnName("id_accion");
                        j.IndexerProperty<int>("IdAccionPrerequisito")
                            .HasColumnType("int(11)")
                            .HasColumnName("id_accion_prerequisito");
                    });
        });

        modelBuilder.Entity<CategoriasRepuesto>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PRIMARY");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PRIMARY");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PRIMARY");

            entity.Property(e => e.Cantidad).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFacturas).HasConstraintName("detalle_factura_ibfk_1");
        });

        modelBuilder.Entity<Diagnostico>(entity =>
        {
            entity.HasKey(e => e.IdDiagnostico).HasName("PRIMARY");

            entity.Property(e => e.FechaDiagnostico).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdMecanicoNavigation).WithMany(p => p.Diagnosticos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("diagnosticos_ibfk_2");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Diagnosticos).HasConstraintName("diagnosticos_ibfk_1");
        });

        modelBuilder.Entity<EstadosAccion>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PRIMARY");
        });

        modelBuilder.Entity<EstadosOrden>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PRIMARY");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PRIMARY");

            entity.Property(e => e.EstadoPago).HasDefaultValueSql("'pendiente'");
            entity.Property(e => e.FechaEmision).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdOrdenNavigation).WithOne(p => p.Factura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("facturas_ibfk_1");
        });

        modelBuilder.Entity<Mecanico>(entity =>
        {
            entity.HasKey(e => e.IdMecanico).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Mecanico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mecanicos_ibfk_1");
        });

        modelBuilder.Entity<MovimientosInventario>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PRIMARY");

            entity.Property(e => e.Fecha).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdRepuestoNavigation).WithMany(p => p.MovimientosInventarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("movimientos_inventario_ibfk_1");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.MovimientosInventarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("movimientos_inventario_ibfk_2");
        });

        modelBuilder.Entity<OrdenesTrabajo>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PRIMARY");

            entity.Property(e => e.FechaIngreso).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.OrdenesTrabajos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordenes_trabajo_ibfk_3");

            entity.HasOne(d => d.IdMecanicoNavigation).WithMany(p => p.OrdenesTrabajos).HasConstraintName("ordenes_trabajo_ibfk_2");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.OrdenesTrabajos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordenes_trabajo_ibfk_1");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PRIMARY");
        });

        modelBuilder.Entity<Repuesto>(entity =>
        {
            entity.HasKey(e => e.IdRepuesto).HasName("PRIMARY");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Repuestos).HasConstraintName("repuestos_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .HasConstraintName("roles_permisos_ibfk_2"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRol")
                        .HasConstraintName("roles_permisos_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("roles_permisos");
                        j.HasIndex(new[] { "IdPermiso" }, "id_permiso");
                        j.IndexerProperty<int>("IdRol")
                            .HasColumnType("int(11)")
                            .HasColumnName("id_rol");
                        j.IndexerProperty<int>("IdPermiso")
                            .HasColumnType("int(11)")
                            .HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<TokensSesion>(entity =>
        {
            entity.HasKey(e => e.IdToken).HasName("PRIMARY");

            entity.Property(e => e.FechaEmision).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TokensSesions).HasConstraintName("tokens_sesion_ibfk_1");
        });

        modelBuilder.Entity<UsoRepuesto>(entity =>
        {
            entity.HasKey(e => e.IdUso).HasName("PRIMARY");

            entity.Property(e => e.Fecha).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdAccionNavigation).WithMany(p => p.UsoRepuestos).HasConstraintName("uso_repuestos_ibfk_1");

            entity.HasOne(d => d.IdRepuestoNavigation).WithMany(p => p.UsoRepuestos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("uso_repuestos_ibfk_2");

            entity.HasOne(d => d.IdUsuarioAutorizaNavigation).WithMany(p => p.UsoRepuestos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("uso_repuestos_ibfk_3");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuarios_ibfk_1");
        });

        modelBuilder.Entity<UsuariosPermiso>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdPermiso })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.UsuariosPermisos).HasConstraintName("usuarios_permisos_ibfk_2");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuariosPermisos).HasConstraintName("usuarios_permisos_ibfk_1");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo).HasName("PRIMARY");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Vehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vehiculos_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
