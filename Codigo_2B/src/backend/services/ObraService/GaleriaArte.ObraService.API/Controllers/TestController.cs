using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.ObraService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObrasController : ControllerBase
{
    [HttpGet("publicas")]
    [AllowAnonymous]
    public IActionResult GetObrasPublicas()
    {
        return Ok(new { mensaje = "Esto es público" });
    }

    [HttpGet("protegidas")]
    [Authorize]
    public IActionResult GetObrasProtegidas()
    {
        var correo = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        return Ok(new { mensaje = "Autenticado correctamente", usuario = correo });
    }
}