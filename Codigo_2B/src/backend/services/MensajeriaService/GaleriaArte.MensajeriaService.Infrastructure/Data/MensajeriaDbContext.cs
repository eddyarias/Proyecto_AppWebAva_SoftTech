using Microsoft.EntityFrameworkCore;
using GaleriaArte.MensajeriaService.Domain.Entities;

namespace GaleriaArte.MensajeriaService.Infrastructure.Data
{
    public class MensajeriaDbContext : DbContext
    {
        public MensajeriaDbContext(DbContextOptions<MensajeriaDbContext> options) : base(options) { }

        public DbSet<Conversacion> Conversaciones { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<UsuarioConexion> UsuariosConexiones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración global para DateTime UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                }
            }

            modelBuilder.HasDefaultSchema("mensajeria");

            // Configuración de Conversacion
            modelBuilder.Entity<Conversacion>(entity =>
            {
                entity.ToTable("conversaciones");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Usuario1Id).HasColumnName("usuario1_id").HasMaxLength(36);
                entity.Property(e => e.Usuario2Id).HasColumnName("usuario2_id").HasMaxLength(36);
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.UltimaActividad).HasColumnName("ultima_actividad");
                entity.Property(e => e.EstaActivaUsuario1).HasColumnName("esta_activa_usuario1");
                entity.Property(e => e.EstaActivaUsuario2).HasColumnName("esta_activa_usuario2");

                // Índices únicos para evitar conversaciones duplicadas
                entity.HasIndex(e => new { e.Usuario1Id, e.Usuario2Id }).IsUnique().HasDatabaseName("ix_conversaciones_usuarios");
                entity.HasIndex(e => e.UltimaActividad).HasDatabaseName("ix_conversaciones_ultima_actividad");
            });

            // Configuración de Mensaje
            modelBuilder.Entity<Mensaje>(entity =>
            {
                entity.ToTable("mensajes");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ConversacionId).HasColumnName("conversacion_id");
                entity.Property(e => e.EmisorId).HasColumnName("emisor_id").HasMaxLength(36);
                entity.Property(e => e.ReceptorId).HasColumnName("receptor_id").HasMaxLength(36);
                entity.Property(e => e.Contenido).HasColumnName("contenido").HasMaxLength(1000);
                entity.Property(e => e.FechaEnvio).HasColumnName("fecha_envio");
                entity.Property(e => e.FechaLeido).HasColumnName("fecha_leido");
                entity.Property(e => e.EsLeido).HasColumnName("es_leido");
                entity.Property(e => e.EsEliminado).HasColumnName("es_eliminado");

                // Relación con Conversacion
                entity.HasOne(e => e.Conversacion)
                      .WithMany(c => c.Mensajes)
                      .HasForeignKey(e => e.ConversacionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.ConversacionId).HasDatabaseName("ix_mensajes_conversacion_id");
                entity.HasIndex(e => e.FechaEnvio).HasDatabaseName("ix_mensajes_fecha_envio");
                entity.HasIndex(e => new { e.ReceptorId, e.EsLeido }).HasDatabaseName("ix_mensajes_receptor_leido");
            });

            // Configuración de UsuarioConexion
            modelBuilder.Entity<UsuarioConexion>(entity =>
            {
                entity.ToTable("usuarios_conexiones");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id").HasMaxLength(36);
                entity.Property(e => e.ConnectionId).HasColumnName("connection_id").HasMaxLength(100);
                entity.Property(e => e.FechaConexion).HasColumnName("fecha_conexion");
                entity.Property(e => e.UltimaActividad).HasColumnName("ultima_actividad");
                entity.Property(e => e.EstaConectado).HasColumnName("esta_conectado");
                entity.Property(e => e.EstaEscribiendo).HasColumnName("esta_escribiendo");
                entity.Property(e => e.EscribiendoEnConversacion).HasColumnName("escribiendo_en_conversacion").HasMaxLength(10);

                // Índices
                entity.HasIndex(e => e.UsuarioId).HasDatabaseName("ix_usuarios_conexiones_usuario_id");
                entity.HasIndex(e => e.ConnectionId).IsUnique().HasDatabaseName("ix_usuarios_conexiones_connection_id");
                entity.HasIndex(e => e.EstaConectado).HasDatabaseName("ix_usuarios_conexiones_esta_conectado");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
