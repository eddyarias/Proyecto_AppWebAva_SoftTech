using Microsoft.EntityFrameworkCore;
using GaleriaArte.AuditoriaService.Domain.Entities;

namespace GaleriaArte.AuditoriaService.Infrastructure.Data
{
    public class AuditoriaDbContext : DbContext
    {
        public AuditoriaDbContext(DbContextOptions<AuditoriaDbContext> options) : base(options) { }

        public DbSet<LogEvento> LogsEventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("auditoria");

            modelBuilder.Entity<LogEvento>(entity =>
            {
                entity.ToTable("logs_eventos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Microservicio).HasColumnName("microservicio").IsRequired();
                entity.Property(e => e.Evento).HasColumnName("evento").IsRequired();
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.RolId).HasColumnName("rol_id");
                entity.Property(e => e.Ip).HasColumnName("ip");
                entity.Property(e => e.Datos).HasColumnName("datos").HasColumnType("jsonb");
                entity.Property(e => e.Fecha).HasColumnName("fecha").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}