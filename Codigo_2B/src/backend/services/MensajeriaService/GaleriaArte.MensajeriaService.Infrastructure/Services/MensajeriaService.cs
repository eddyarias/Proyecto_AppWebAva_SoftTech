using Microsoft.EntityFrameworkCore;
using GaleriaArte.MensajeriaService.Application.Services;
using GaleriaArte.MensajeriaService.Domain.Entities;
using GaleriaArte.MensajeriaService.Domain.DTOs;
using GaleriaArte.MensajeriaService.Infrastructure.Data;

namespace GaleriaArte.MensajeriaService.Infrastructure.Services
{
    public class MensajeriaService : IMensajeriaService
    {
        private readonly MensajeriaDbContext _context;
        private readonly ILogger<MensajeriaService> _logger;

        public MensajeriaService(MensajeriaDbContext context, ILogger<MensajeriaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MensajeDto?> EnviarMensajeAsync(string emisorId, EnviarMensajeDto enviarMensajeDto)
        {
            try
            {
                var conversacion = await CrearOObtenerConversacionAsync(emisorId, enviarMensajeDto.ReceptorId);
                if (conversacion == null)
                {
                    _logger.LogError($"No se pudo crear o encontrar conversación entre {emisorId} y {enviarMensajeDto.ReceptorId}");
                    return null;
                }

                var mensaje = new Mensaje
                {
                    ConversacionId = conversacion.Id,
                    EmisorId = emisorId,
                    ReceptorId = enviarMensajeDto.ReceptorId,
                    Contenido = enviarMensajeDto.Contenido,
                    FechaEnvio = DateTime.UtcNow,
                    Leido = false
                };

                _context.Mensajes.Add(mensaje);
                
                // Actualizar última actividad de la conversación
                var conversacionEntity = await _context.Conversaciones.FindAsync(conversacion.Id);
                if (conversacionEntity != null)
                {
                    conversacionEntity.UltimaActividad = DateTime.UtcNow;
                    conversacionEntity.UltimoMensaje = enviarMensajeDto.Contenido;
                }

                await _context.SaveChangesAsync();

                return new MensajeDto
                {
                    Id = mensaje.Id,
                    ConversacionId = mensaje.ConversacionId,
                    EmisorId = mensaje.EmisorId,
                    ReceptorId = mensaje.ReceptorId,
                    Contenido = mensaje.Contenido,
                    FechaEnvio = mensaje.FechaEnvio,
                    Leido = mensaje.Leido
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje");
                return null;
            }
        }

        public async Task<List<ConversacionDto>> ObtenerConversacionesAsync(string usuarioId)
        {
            try
            {
                var conversaciones = await _context.Conversaciones
                    .Where(c => c.Usuario1Id == usuarioId || c.Usuario2Id == usuarioId)
                    .OrderByDescending(c => c.UltimaActividad)
                    .Select(c => new ConversacionDto
                    {
                        Id = c.Id,
                        OtroUsuarioId = c.Usuario1Id == usuarioId ? c.Usuario2Id : c.Usuario1Id,
                        UltimoMensaje = c.UltimoMensaje,
                        UltimaActividad = c.UltimaActividad,
                        MensajesNoLeidos = _context.Mensajes.Count(m => 
                            m.ConversacionId == c.Id && 
                            m.ReceptorId == usuarioId && 
                            !m.Leido),
                        FechaCreacion = c.FechaCreacion
                    })
                    .ToListAsync();

                return conversaciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversaciones");
                return new List<ConversacionDto>();
            }
        }

        public async Task<List<MensajeDto>> ObtenerMensajesAsync(int conversacionId, string usuarioId, int pagina = 1, int tamanoPagina = 50)
        {
            try
            {
                // Verificar que el usuario tenga acceso a la conversación
                var conversacion = await _context.Conversaciones
                    .FirstOrDefaultAsync(c => c.Id == conversacionId && 
                                            (c.Usuario1Id == usuarioId || c.Usuario2Id == usuarioId));

                if (conversacion == null)
                {
                    _logger.LogWarning($"Usuario {usuarioId} intentó acceder a conversación {conversacionId} sin permisos");
                    return new List<MensajeDto>();
                }

                var mensajes = await _context.Mensajes
                    .Where(m => m.ConversacionId == conversacionId)
                    .OrderByDescending(m => m.FechaEnvio)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .Select(m => new MensajeDto
                    {
                        Id = m.Id,
                        ConversacionId = m.ConversacionId,
                        EmisorId = m.EmisorId,
                        ReceptorId = m.ReceptorId,
                        Contenido = m.Contenido,
                        FechaEnvio = m.FechaEnvio,
                        Leido = m.Leido
                    })
                    .ToListAsync();

                // Devolver en orden cronológico (más antiguos primero)
                return mensajes.OrderBy(m => m.FechaEnvio).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensajes");
                return new List<MensajeDto>();
            }
        }

        public async Task<ConversacionDto?> ObtenerConversacionAsync(int conversacionId, string usuarioId)
        {
            try
            {
                var conversacion = await _context.Conversaciones
                    .Where(c => c.Id == conversacionId && 
                              (c.Usuario1Id == usuarioId || c.Usuario2Id == usuarioId))
                    .Select(c => new ConversacionDto
                    {
                        Id = c.Id,
                        OtroUsuarioId = c.Usuario1Id == usuarioId ? c.Usuario2Id : c.Usuario1Id,
                        UltimoMensaje = c.UltimoMensaje,
                        UltimaActividad = c.UltimaActividad,
                        MensajesNoLeidos = _context.Mensajes.Count(m => 
                            m.ConversacionId == c.Id && 
                            m.ReceptorId == usuarioId && 
                            !m.Leido),
                        FechaCreacion = c.FechaCreacion
                    })
                    .FirstOrDefaultAsync();

                return conversacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversación");
                return null;
            }
        }

        public async Task<ConversacionDto?> CrearOObtenerConversacionAsync(string usuario1Id, string usuario2Id)
        {
            try
            {
                // Buscar conversación existente
                var conversacionExistente = await _context.Conversaciones
                    .FirstOrDefaultAsync(c => 
                        (c.Usuario1Id == usuario1Id && c.Usuario2Id == usuario2Id) ||
                        (c.Usuario1Id == usuario2Id && c.Usuario2Id == usuario1Id));

                if (conversacionExistente != null)
                {
                    return new ConversacionDto
                    {
                        Id = conversacionExistente.Id,
                        OtroUsuarioId = conversacionExistente.Usuario1Id == usuario1Id ? 
                                       conversacionExistente.Usuario2Id : conversacionExistente.Usuario1Id,
                        UltimoMensaje = conversacionExistente.UltimoMensaje,
                        UltimaActividad = conversacionExistente.UltimaActividad,
                        FechaCreacion = conversacionExistente.FechaCreacion
                    };
                }

                // Crear nueva conversación
                var nuevaConversacion = new Conversacion
                {
                    Usuario1Id = usuario1Id,
                    Usuario2Id = usuario2Id,
                    FechaCreacion = DateTime.UtcNow,
                    UltimaActividad = DateTime.UtcNow,
                    UltimoMensaje = ""
                };

                _context.Conversaciones.Add(nuevaConversacion);
                await _context.SaveChangesAsync();

                return new ConversacionDto
                {
                    Id = nuevaConversacion.Id,
                    OtroUsuarioId = usuario2Id,
                    UltimoMensaje = nuevaConversacion.UltimoMensaje,
                    UltimaActividad = nuevaConversacion.UltimaActividad,
                    FechaCreacion = nuevaConversacion.FechaCreacion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear u obtener conversación");
                return null;
            }
        }

        public async Task MarcarMensajesComoLeidosAsync(int conversacionId, string usuarioId)
        {
            try
            {
                var mensajesNoLeidos = await _context.Mensajes
                    .Where(m => m.ConversacionId == conversacionId && 
                              m.ReceptorId == usuarioId && 
                              !m.Leido)
                    .ToListAsync();

                foreach (var mensaje in mensajesNoLeidos)
                {
                    mensaje.Leido = true;
                    mensaje.FechaLectura = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar mensajes como leídos");
            }
        }

        public async Task<List<string>> ObtenerContactosAsync(string usuarioId)
        {
            try
            {
                var contactos = await _context.Conversaciones
                    .Where(c => c.Usuario1Id == usuarioId || c.Usuario2Id == usuarioId)
                    .Select(c => c.Usuario1Id == usuarioId ? c.Usuario2Id : c.Usuario1Id)
                    .Distinct()
                    .ToListAsync();

                return contactos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contactos");
                return new List<string>();
            }
        }

        public async Task<int> ContarMensajesNoLeidosAsync(string usuarioId)
        {
            try
            {
                return await _context.Mensajes
                    .CountAsync(m => m.ReceptorId == usuarioId && !m.Leido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar mensajes no leídos");
                return 0;
            }
        }

        public async Task<bool> ExisteConversacionAsync(string usuario1Id, string usuario2Id)
        {
            try
            {
                return await _context.Conversaciones
                    .AnyAsync(c => 
                        (c.Usuario1Id == usuario1Id && c.Usuario2Id == usuario2Id) ||
                        (c.Usuario1Id == usuario2Id && c.Usuario2Id == usuario1Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de conversación");
                return false;
            }
        }
    }
}
