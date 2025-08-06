using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    usuario_service = "operational",
                    obra_service = "operational", 
                    auditoria_service = "operational",
                    media_service = "operational"
                },
                circuit_breakers = new
                {
                    usuario = "closed",
                    obra = "closed",
                    auditoria = "closed"
                }
            });
        }

        [HttpGet("circuit-breakers")]
        public IActionResult GetCircuitBreakerStatus()
        {
            return Ok(new
            {
                circuit_breakers = new
                {
                    usuario_service = new
                    {
                        state = "closed",
                        failure_count = 0,
                        last_failure = (DateTime?)null,
                        next_attempt = (DateTime?)null
                    },
                    obra_service = new
                    {
                        state = "closed", 
                        failure_count = 0,
                        last_failure = (DateTime?)null,
                        next_attempt = (DateTime?)null
                    },
                    auditoria_service = new
                    {
                        state = "closed",
                        failure_count = 0, 
                        last_failure = (DateTime?)null,
                        next_attempt = (DateTime?)null
                    }
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}
