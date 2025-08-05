using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GaleriaArte.MensajeriaService.Application.Services;
using GaleriaArte.MensajeriaService.Domain.DTOs;

namespace GaleriaArte.MensajeriaService.Infrastructure.Hubs
{
    [Authorize]
    public class MensajeriaHub : Hub
    {
        private readonly IMensajeriaService _mensajeriaService;
        private readonly IConexionService _conexionService;
        private readonly ILogger<MensajeriaHub> _logger;

        public MensajeriaHub(
            IMensajeriaService mensajeriaService,
            IConexionService conexionService,
            ILogger<MensajeriaHub> logger)
        {
            _mensajeriaService = mensajeriaService;
            _conexionService = conexionService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var usuarioId = ObtenerUsuarioId();
            if (!string.IsNullOrEmpty(usuarioId))
            {
                await _conexionService.RegistrarConexionAsync(usuarioId, Context.ConnectionId);
                
                // Unirse a un grupo personal para recibir notificaciones
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{usuarioId}");
                
                // Notificar a contactos que el usuario se conectó
                await NotificarEstadoConexion(usuarioId, true);
                
                _logger.LogInformation($"Usuario {usuarioId} conectado con ConnectionId {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var usuarioId = ObtenerUsuarioId();
            if (!string.IsNullOrEmpty(usuarioId))
            {
                await _conexionService.RegistrarDesconexionAsync(Context.ConnectionId);
                
                // Notificar a contactos que el usuario se desconectó
                await NotificarEstadoConexion(usuarioId, false);
                
                _logger.LogInformation($"Usuario {usuarioId} desconectado. ConnectionId {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Enviar mensaje
        public async Task EnviarMensaje(EnviarMensajeDto enviarMensajeDto)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    await Clients.Caller.SendAsync("Error", "Usuario no autenticado");
                    return;
                }

                var mensaje = await _mensajeriaService.EnviarMensajeAsync(usuarioId, enviarMensajeDto);
                if (mensaje != null)
                {
                    // Enviar mensaje al emisor
                    await Clients.Group($"user_{usuarioId}").SendAsync("MensajeEnviado", mensaje);
                    
                    // Enviar mensaje al receptor
                    await Clients.Group($"user_{mensaje.ReceptorId}").SendAsync("MensajeRecibido", mensaje);
                    
                    _logger.LogInformation($"Mensaje enviado de {usuarioId} a {mensaje.ReceptorId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje");
                await Clients.Caller.SendAsync("Error", "Error al enviar el mensaje");
            }
        }

        // Marcar mensajes como leídos
        public async Task MarcarMensajesComoLeidos(int conversacionId)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    await Clients.Caller.SendAsync("Error", "Usuario no autenticado");
                    return;
                }

                await _mensajeriaService.MarcarMensajesComoLeidosAsync(conversacionId, usuarioId);
                
                // Obtener la conversación para notificar al otro usuario
                var conversacion = await _mensajeriaService.ObtenerConversacionAsync(conversacionId, usuarioId);
                if (conversacion != null)
                {
                    await Clients.Group($"user_{conversacion.OtroUsuarioId}")
                        .SendAsync("MensajesLeidos", new { ConversacionId = conversacionId, LectorId = usuarioId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar mensajes como leídos");
            }
        }

        // Indicar que el usuario está escribiendo
        public async Task EscribiendoMensaje(int conversacionId)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                    return;

                await _conexionService.IniciarEscrituraAsync(usuarioId, conversacionId.ToString());
                
                // Obtener la conversación para notificar al otro usuario
                var conversacion = await _mensajeriaService.ObtenerConversacionAsync(conversacionId, usuarioId);
                if (conversacion != null)
                {
                    await Clients.Group($"user_{conversacion.OtroUsuarioId}")
                        .SendAsync("UsuarioEscribiendo", new { ConversacionId = conversacionId, UsuarioId = usuarioId, EstaEscribiendo = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al indicar escritura");
            }
        }

        // Indicar que el usuario dejó de escribir
        public async Task DejoDeEscribir(int conversacionId)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                    return;

                await _conexionService.DetenerEscrituraAsync(usuarioId);
                
                // Obtener la conversación para notificar al otro usuario
                var conversacion = await _mensajeriaService.ObtenerConversacionAsync(conversacionId, usuarioId);
                if (conversacion != null)
                {
                    await Clients.Group($"user_{conversacion.OtroUsuarioId}")
                        .SendAsync("UsuarioEscribiendo", new { ConversacionId = conversacionId, UsuarioId = usuarioId, EstaEscribiendo = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detener indicador de escritura");
            }
        }

        // Obtener conversaciones del usuario
        public async Task ObtenerConversaciones()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    await Clients.Caller.SendAsync("Error", "Usuario no autenticado");
                    return;
                }

                var conversaciones = await _mensajeriaService.ObtenerConversacionesAsync(usuarioId);
                await Clients.Caller.SendAsync("ConversacionesActualizadas", conversaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversaciones");
                await Clients.Caller.SendAsync("Error", "Error al obtener conversaciones");
            }
        }

        // Obtener mensajes de una conversación
        public async Task ObtenerMensajes(int conversacionId, int pagina = 1, int tamanoPagina = 50)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    await Clients.Caller.SendAsync("Error", "Usuario no autenticado");
                    return;
                }

                var mensajes = await _mensajeriaService.ObtenerMensajesAsync(conversacionId, usuarioId, pagina, tamanoPagina);
                await Clients.Caller.SendAsync("MensajesActualizados", new { ConversacionId = conversacionId, Mensajes = mensajes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensajes");
                await Clients.Caller.SendAsync("Error", "Error al obtener mensajes");
            }
        }

        private async Task NotificarEstadoConexion(string usuarioId, bool estaConectado)
        {
            try
            {
                // Obtener contactos del usuario (usuarios con conversaciones activas)
                var contactos = await _mensajeriaService.ObtenerContactosAsync(usuarioId);
                
                foreach (var contacto in contactos)
                {
                    await Clients.Group($"user_{contacto}")
                        .SendAsync("EstadoConexionCambiado", new { UsuarioId = usuarioId, EstaConectado = estaConectado });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar estado de conexión");
            }
        }

        private string? ObtenerUsuarioId()
        {
            return Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                   Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   Context.User?.FindFirst("sub")?.Value;
        }
    }
}
