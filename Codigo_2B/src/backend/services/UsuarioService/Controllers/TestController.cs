using Microsoft.AspNetCore.Mvc;

namespace UsuarioService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("mensaje")]
        public IActionResult ObtenerMensaje()
        {
            return Ok(new { mensaje = "¡Hola desde UsuarioService a través del API Gateway!" });
        }
    }
}