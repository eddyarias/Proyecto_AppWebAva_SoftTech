using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.ObraService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
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
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new { mensaje = "Autenticado correctamente", claims });
    }
}