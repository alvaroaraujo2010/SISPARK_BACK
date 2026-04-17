using Microsoft.EntityFrameworkCore;
using sispark_api.Domain.Entities;

namespace sispark_api.Infrastructure.Data;

public class SisparkDbContext(DbContextOptions<SisparkDbContext> options) : DbContext(options)
{
    public DbSet<Estado> Estados => Set<Estado>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<TipoVehiculo> TiposVehiculo => Set<TipoVehiculo>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<TipoServicio> TiposServicio => Set<TipoServicio>();
    public DbSet<Tarifa> Tarifas => Set<Tarifa>();
    public DbSet<Celda> Celdas => Set<Celda>();
    public DbSet<Mensualidad> Mensualidades => Set<Mensualidad>();
    public DbSet<RegistroParqueo> RegistrosParqueo => Set<RegistroParqueo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.ToTable("estados");
            entity.HasKey(x => x.IdEstado);
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.Modulo).HasColumnName("modulo").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(150);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.IdRol);
            entity.Property(x => x.IdRol).HasColumnName("id_rol");
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(150);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(x => x.IdUsuario);
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Apellido).HasColumnName("apellido").HasMaxLength(100).IsRequired();
            entity.Property(x => x.UsuarioLogin).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(120);
            entity.Property(x => x.Contrasena).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.IdRol).HasColumnName("id_rol");
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(x => x.UltimoAcceso).HasColumnName("ultimo_acceso");

            entity.HasOne(x => x.Rol).WithMany().HasForeignKey(x => x.IdRol);
            entity.HasOne(x => x.Estado).WithMany().HasForeignKey(x => x.IdEstado);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(x => x.IdCliente);
            entity.Property(x => x.IdCliente).HasColumnName("id_cliente");
            entity.Property(x => x.TipoDocumento).HasColumnName("tipo_documento").HasMaxLength(20).IsRequired();
            entity.Property(x => x.NumeroDocumento).HasColumnName("numero_documento").HasMaxLength(30).IsRequired();
            entity.Property(x => x.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Correo).HasColumnName("correo").HasMaxLength(120);
            entity.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(30);
            entity.Property(x => x.Direccion).HasColumnName("direccion").HasMaxLength(180);
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(x => x.IdUsuarioCreacion).HasColumnName("id_usuario_creacion");
            entity.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");
            entity.Property(x => x.IdUsuarioActualizacion).HasColumnName("id_usuario_actualizacion");
        });

        modelBuilder.Entity<TipoVehiculo>(entity =>
        {
            entity.ToTable("tipos_vehiculo");
            entity.HasKey(x => x.IdTipoVehiculo);
            entity.Property(x => x.IdTipoVehiculo).HasColumnName("id_tipo_vehiculo");
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(150);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.ToTable("vehiculos");
            entity.HasKey(x => x.IdVehiculo);
            entity.Property(x => x.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(x => x.Placa).HasColumnName("placa").HasMaxLength(15).IsRequired();
            entity.Property(x => x.Marca).HasColumnName("marca").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Modelo).HasColumnName("modelo").HasMaxLength(50);
            entity.Property(x => x.Color).HasColumnName("color").HasMaxLength(30);
            entity.Property(x => x.IdTipoVehiculo).HasColumnName("id_tipo_vehiculo");
            entity.Property(x => x.IdCliente).HasColumnName("id_cliente");
            entity.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(255);
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(x => x.IdUsuarioCreacion).HasColumnName("id_usuario_creacion");
            entity.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");
            entity.Property(x => x.IdUsuarioActualizacion).HasColumnName("id_usuario_actualizacion");

            entity.HasOne(x => x.TipoVehiculo).WithMany().HasForeignKey(x => x.IdTipoVehiculo);
            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.IdCliente);
        });

        modelBuilder.Entity<TipoServicio>(entity =>
        {
            entity.ToTable("tipos_servicio");
            entity.HasKey(x => x.IdTipoServicio);
            entity.Property(x => x.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(150);
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.ToTable("tarifas");
            entity.HasKey(x => x.IdTarifa);
            entity.Property(x => x.IdTarifa).HasColumnName("id_tarifa");
            entity.Property(x => x.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(x => x.IdTipoVehiculo).HasColumnName("id_tipo_vehiculo");
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
            entity.Property(x => x.Valor).HasColumnName("valor").HasColumnType("decimal(10,2)");
            entity.Property(x => x.FraccionMinutos).HasColumnName("fraccion_minutos");
            entity.Property(x => x.FechaInicioVigencia).HasColumnName("fecha_inicio_vigencia");
            entity.Property(x => x.FechaFinVigencia).HasColumnName("fecha_fin_vigencia");
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");

            entity.HasOne(x => x.TipoServicio).WithMany().HasForeignKey(x => x.IdTipoServicio);
            entity.HasOne(x => x.TipoVehiculo).WithMany().HasForeignKey(x => x.IdTipoVehiculo);
        });

        modelBuilder.Entity<Celda>(entity =>
        {
            entity.ToTable("celdas");
            entity.HasKey(x => x.IdCelda);
            entity.Property(x => x.IdCelda).HasColumnName("id_celda");
            entity.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Ubicacion).HasColumnName("ubicacion").HasMaxLength(100);
            entity.Property(x => x.IdTipoVehiculo).HasColumnName("id_tipo_vehiculo");
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(150);
        });

        modelBuilder.Entity<Mensualidad>(entity =>
        {
            entity.ToTable("mensualidades");
            entity.HasKey(x => x.IdMensualidad);
            entity.Property(x => x.IdMensualidad).HasColumnName("id_mensualidad");
            entity.Property(x => x.IdCliente).HasColumnName("id_cliente");
            entity.Property(x => x.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(x => x.IdTarifa).HasColumnName("id_tarifa");
            entity.Property(x => x.IdCeldaFija).HasColumnName("id_celda_fija");
            entity.Property(x => x.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(x => x.FechaFin).HasColumnName("fecha_fin");
            entity.Property(x => x.Valor).HasColumnName("valor").HasColumnType("decimal(10,2)");
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(255);

            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.IdCliente);
            entity.HasOne(x => x.Vehiculo).WithMany().HasForeignKey(x => x.IdVehiculo);
            entity.HasOne(x => x.Tarifa).WithMany().HasForeignKey(x => x.IdTarifa);
            entity.HasOne(x => x.CeldaFija).WithMany().HasForeignKey(x => x.IdCeldaFija);
        });

        modelBuilder.Entity<RegistroParqueo>(entity =>
        {
            entity.ToTable("registros_parqueo");
            entity.HasKey(x => x.IdRegistro);
            entity.Property(x => x.IdRegistro).HasColumnName("id_registro");
            entity.Property(x => x.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(x => x.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(x => x.IdTarifa).HasColumnName("id_tarifa");
            entity.Property(x => x.IdCelda).HasColumnName("id_celda");
            entity.Property(x => x.FechaIngreso).HasColumnName("fecha_ingreso");
            entity.Property(x => x.FechaSalida).HasColumnName("fecha_salida");
            entity.Property(x => x.MinutosConsumidos).HasColumnName("minutos_consumidos");
            entity.Property(x => x.ValorCalculado).HasColumnName("valor_calculado").HasColumnType("decimal(10,2)");
            entity.Property(x => x.ValorFinal).HasColumnName("valor_final").HasColumnType("decimal(10,2)");
            entity.Property(x => x.IdUsuarioIngreso).HasColumnName("id_usuario_ingreso");
            entity.Property(x => x.IdUsuarioSalida).HasColumnName("id_usuario_salida");
            entity.Property(x => x.IdEstado).HasColumnName("id_estado");
            entity.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(255);

            entity.HasOne(x => x.Vehiculo).WithMany().HasForeignKey(x => x.IdVehiculo);
            entity.HasOne(x => x.TipoServicio).WithMany().HasForeignKey(x => x.IdTipoServicio);
            entity.HasOne(x => x.Tarifa).WithMany().HasForeignKey(x => x.IdTarifa);
            entity.HasOne(x => x.Celda).WithMany().HasForeignKey(x => x.IdCelda);
            entity.HasOne(x => x.UsuarioIngreso).WithMany().HasForeignKey(x => x.IdUsuarioIngreso);
            entity.HasOne(x => x.UsuarioSalida).WithMany().HasForeignKey(x => x.IdUsuarioSalida);
        });
    }
}
