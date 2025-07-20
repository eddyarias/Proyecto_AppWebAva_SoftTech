using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Application.Interfaces;
using GaleriaArte.UsuarioService.Application.Services;
using GaleriaArte.UsuarioService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.UsuarioService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;


    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }
    

    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] UsuarioDto dto)
    {
        var result = await _usuarioService.RegistrarUsuarioAsync(dto);
        return Ok(result);
    }

    [HttpPatch("cambiar-estado")]
    [Authorize]
    public async Task<IActionResult> CambiarEstadoUsuario([FromBody] CambiarEstadoUsuarioDto dto)
    {
        var resultado = await _usuarioService.CambiarEstadoUsuarioAsync(dto.UsuarioId, dto.NuevoEstado);

        if (resultado)
        {
            string mensaje = dto.NuevoEstado ? "Usuario activado correctamente." : "Usuario desactivado correctamente.";
            return Ok(new { mensaje });
        }

        return NotFound(new { mensaje = "Usuario no encontrado." });
    }

}