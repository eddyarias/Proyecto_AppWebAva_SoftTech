using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Application.Interfaces;
using GaleriaArte.UsuarioService.Application.Services;
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
    public async Task<IActionResult> Registrar([FromBody] UsuarioDto dto)
    {
        var result = await _usuarioService.RegistrarUsuarioAsync(dto);
        return Ok(result);
    }
}