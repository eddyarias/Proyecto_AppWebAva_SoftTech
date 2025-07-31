using Microsoft.AspNetCore.Mvc;
using GaleriaArte.AuditoriaService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GaleriaArte.AuditoriaService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly ILogger<AuditoriaController> _logger;

        public AuditoriaController(IAuditoriaRepository auditoriaRepository, ILogger<AuditoriaController> logger)
        {
            _auditoriaRepository = auditoriaRepository;
            _logger = logger;
        }

        [HttpGet("microservicio/{microservicio}")]
        [Authorize (Roles="f50fdbe5-2e16-4e91-9e7b-a39219d57031")]
        public async Task<IActionResult> ObtenerLogsPorMicroservicio(string microservicio, [FromQuery] int limite = 100)
        {
            try
            {
                var logs = await _auditoriaRepository.ObtenerLogsPorMicroservicioAsync(microservicio, limite);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por microservicio: {Microservicio}", microservicio);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        [Authorize (Roles="f50fdbe5-2e16-4e91-9e7b-a39219d57031")]
        public async Task<IActionResult> ObtenerLogsPorUsuario(string usuarioId, [FromQuery] int limite = 100)
        {
            try
            {
                var logs = await _auditoriaRepository.ObtenerLogsPorUsuarioAsync(usuarioId, limite);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por usuario: {UsuarioId}", usuarioId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("fecha")]
        [Authorize (Roles="f50fdbe5-2e16-4e91-9e7b-a39219d57031")]
        public async Task<IActionResult> ObtenerLogsPorFecha([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha fin");
                }

                var logs = await _auditoriaRepository.ObtenerLogsPorFechaAsync(fechaInicio, fechaFin);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por fecha: {FechaInicio} - {FechaFin}", fechaInicio, fechaFin);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}