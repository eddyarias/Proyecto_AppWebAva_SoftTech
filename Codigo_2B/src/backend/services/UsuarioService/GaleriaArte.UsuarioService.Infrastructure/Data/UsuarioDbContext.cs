using GaleriaArte.UsuarioService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GaleriaArte.UsuarioService.Infrastructure.Data;

public class UsuarioDbContext : DbContext
{
    public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        // Configuración de UsuarioRol
        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.ToTable("usuarios_roles");
            
            // Configurar clave primaria compuesta
            entity.HasKey(ur => new { ur.UsuarioId, ur.RolId });

            // Mapear nombres de columnas
            entity.Property(ur => ur.UsuarioId).HasColumnName("usuario_id");
            entity.Property(ur => ur.RolId).HasColumnName("rol_id");

            // Configurar relación con Usuario
            entity.HasOne(ur => ur.Usuario)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UsuarioId)
                .HasConstraintName("fk_usuarios_roles_usuario");

            // Configurar relación con Rol
            entity.HasOne(ur => ur.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(ur => ur.RolId)
                .HasConstraintName("fk_usuarios_roles_rol");
        });
    }
}