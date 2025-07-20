using Microsoft.EntityFrameworkCore;
using GaleriaArte.ObraService.Domain.Entities;

namespace GaleriaArte.ObraService.Infrastructure.Data;

public class ObraDbContext : DbContext
{
    public ObraDbContext(DbContextOptions<ObraDbContext> options) : base(options) { }

    public DbSet<Obra> Obras => Set<Obra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("obras");

        modelBuilder.Entity<Obra>(entity =>
        {
            entity.ToTable("obras");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Titulo).HasColumnName("titulo").IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.ArchivoBase64).HasColumnName("archivo_base64").IsRequired();
            entity.Property(e => e.FirmaDigital).HasColumnName("firma_digital").IsRequired();
            entity.Property(e => e.ArtistaNickname).HasColumnName("artista_nickname").IsRequired();
            entity.Property(e => e.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();
            
            // 🔧 CONFIGURACIÓN ESPECÍFICA PARA DATETIME UTC
            entity.Property(e => e.FechaPublicacion)
                  .HasColumnName("fecha_publicacion")
                  .HasConversion(
                      v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                      v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
    }
}