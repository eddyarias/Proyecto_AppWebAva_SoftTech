using GaleriaArte.ObraService.Application.DTOs;
using GaleriaArte.ObraService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.ObraService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObraController : ControllerBase
{
    private readonly IObraService _obraService;

    public ObraController(IObraService obraService)
    {
        _obraService = obraService;
    }

    [HttpPost("crear")]
    //[Authorize]
    public async Task<IActionResult> Crear([FromBody] CreateObraDto dto)
    {
        var result = await _obraService.CrearObraAsync(dto);
        return Ok(result);
    }
}