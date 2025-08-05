using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using GaleriaArte.MensajeriaService.Application.Services;
using GaleriaArte.MensajeriaService.Domain.DTOs;
using GaleriaArte.MensajeriaService.Infrastructure.Hubs;

namespace GaleriaArte.MensajeriaService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MensajeriaController : ControllerBase
    {
        private readonly IMensajeriaService _mensajeriaService;
        private readonly IConexionService _conexionService;
        private readonly IHubContext<MensajeriaHub> _hubContext;
        private readonly ILogger<MensajeriaController> _logger;

        public MensajeriaController(
            IMensajeriaService mensajeriaService,
            IConexionService conexionService,
            IHubContext<MensajeriaHub> hubContext,
            ILogger<MensajeriaController> logger)
        {
            _mensajeriaService = mensajeriaService;
            _conexionService = conexionService;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las conversaciones del usuario autenticado
        /// </summary>
        [HttpGet("conversaciones")]
        public async Task<ActionResult<List<ConversacionDto>>> ObtenerConversaciones()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var conversaciones = await _mensajeriaService.ObtenerConversacionesAsync(usuarioId);
                return Ok(conversaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversaciones");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los mensajes de una conversación específica
        /// </summary>
        [HttpGet("conversaciones/{conversacionId}/mensajes")]
        public async Task<ActionResult<List<MensajeDto>>> ObtenerMensajes(
            int conversacionId, 
            [FromQuery] int pagina = 1, 
            [FromQuery] int tamanoPagina = 50)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var mensajes = await _mensajeriaService.ObtenerMensajesAsync(conversacionId, usuarioId, pagina, tamanoPagina);
                return Ok(mensajes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensajes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Envía un mensaje
        /// </summary>
        [HttpPost("enviar")]
        public async Task<ActionResult<MensajeDto>> EnviarMensaje([FromBody] EnviarMensajeDto enviarMensajeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var mensaje = await _mensajeriaService.EnviarMensajeAsync(usuarioId, enviarMensajeDto);
                if (mensaje == null)
                {
                    return BadRequest("No se pudo enviar el mensaje");
                }

                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea o obtiene una conversación entre dos usuarios
        /// </summary>
        [HttpPost("conversaciones")]
        public async Task<ActionResult<ConversacionDto>> CrearConversacion([FromBody] CrearConversacionDto crearConversacionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var conversacion = await _mensajeriaService.CrearOObtenerConversacionAsync(usuarioId, crearConversacionDto.OtroUsuarioId);
                if (conversacion == null)
                {
                    return BadRequest("No se pudo crear la conversación");
                }

                return Ok(conversacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear conversación");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Marca mensajes como leídos
        /// </summary>
        [HttpPut("conversaciones/{conversacionId}/marcar-leidos")]
        public async Task<ActionResult> MarcarMensajesComoLeidos(int conversacionId)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                await _mensajeriaService.MarcarMensajesComoLeidosAsync(conversacionId, usuarioId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar mensajes como leídos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el conteo de mensajes no leídos del usuario
        /// </summary>
        [HttpGet("mensajes-no-leidos/contador")]
        public async Task<ActionResult<int>> ContarMensajesNoLeidos()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var contador = await _mensajeriaService.ContarMensajesNoLeidosAsync(usuarioId);
                return Ok(contador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar mensajes no leídos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un usuario está conectado
        /// </summary>
        [HttpGet("usuarios/{usuarioId}/estado-conexion")]
        public async Task<ActionResult<bool>> VerificarEstadoConexion(string usuarioId)
        {
            try
            {
                var estaConectado = await _conexionService.EstaConectadoAsync(usuarioId);
                return Ok(estaConectado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar estado de conexión");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene información de una conversación específica
        /// </summary>
        [HttpGet("conversaciones/{conversacionId}")]
        public async Task<ActionResult<ConversacionDto>> ObtenerConversacion(int conversacionId)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var conversacion = await _mensajeriaService.ObtenerConversacionAsync(conversacionId, usuarioId);
                if (conversacion == null)
                {
                    return NotFound("Conversación no encontrada");
                }

                return Ok(conversacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversación");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si existe una conversación entre dos usuarios
        /// </summary>
        [HttpGet("conversaciones/existe/{otroUsuarioId}")]
        public async Task<ActionResult<bool>> ExisteConversacion(string otroUsuarioId)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var existe = await _mensajeriaService.ExisteConversacionAsync(usuarioId, otroUsuarioId);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de conversación");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private string? ObtenerUsuarioId()
        {
            return User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                   User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   User?.FindFirst("sub")?.Value;
        }
    }
}
