
using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.UsuarioService.API.Controllers;

[ApiController]
[Route("api/recuperacion")]
public class RecuperacionController : ControllerBase
{

    private readonly IRecuperacionService _service;
    public RecuperacionController(IRecuperacionService service)
    {
        _service = service;
    }



    [HttpPost("solicitar")]
    public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarRecuperacionDto dto)
    {
        var resultado = await _service.SolicitarRecuperacionAsync(dto.Correo);
        if (resultado)
            return Ok(new { mensaje = "Instrucciones enviadas al correo." });
        return NotFound(new { mensaje = "Correo no registrado." });
    }

    [HttpPost("restablecer")]
    public async Task<IActionResult> Restablecer([FromBody] RestablecerPasswordDto dto)
    {
        var resultado = await _service.RestablecerPasswordAsync(dto.Token, dto.NuevaPassword);
        if (resultado)
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        return BadRequest(new { mensaje = "Token inválido o expirado." });
    }
}