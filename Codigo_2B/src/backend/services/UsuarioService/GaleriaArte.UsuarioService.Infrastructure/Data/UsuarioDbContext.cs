using GaleriaArte.UsuarioService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GaleriaArte.UsuarioService.Infrastructure.Data;

public class UsuarioDbContext : DbContext
{
    public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }
            }
        }

        modelBuilder.HasDefaultSchema("usuarios");

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nickname).HasColumnName("nickname");
            entity.Property(e => e.Correo).HasColumnName("correo");
            entity.Property(e => e.ContraseñaHash).HasColumnName("contraseña_hash");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExp).HasColumnName("refresh_token_exp");
            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.HasOne(e => e.Rol)
                .WithMany()
                .HasForeignKey(e => e.RolId)
                .HasConstraintName("fk_usuario_rol");
        });

        // Configuración de Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });
    }
}