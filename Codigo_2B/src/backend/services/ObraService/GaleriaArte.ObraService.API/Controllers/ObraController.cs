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
    public async Task<IActionResult> Crear([FromForm] CreateObraDto dto)
    {
        try
        {
            var result = await _obraService.CrearObraAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FileNotFoundException ex)
        {
            return BadRequest(new { error = $"Archivo no encontrado: {ex.Message}" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = $"Error de configuración: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var obra = await _obraService.ObtenerObraPorIdAsync(id);
            if (obra == null)
                return NotFound(new { message = "Obra no encontrada" });

            return Ok(obra);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("artista/{artistaNickname}")]
    public async Task<IActionResult> ObtenerPorArtista(string artistaNickname)
    {
        try
        {
            var obras = await _obraService.ObtenerObrasPorArtistaAsync(artistaNickname);
            return Ok(obras);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("activas")]
    public async Task<IActionResult> ObtenerObrasActivas([FromQuery] int limite = 10)
    {
        try
        {
            var obras = await _obraService.ObtenerObrasActivasAsync(limite);
            return Ok(obras);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    //[Authorize]
    public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateObraDto dto)
    {
        try
        {
            var resultado = await _obraService.ActualizarObraAsync(id, dto);
            if (!resultado)
                return NotFound(new { message = "Obra no encontrada" });

            return Ok(new { message = "Obra actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/ocultar")]
    //[Authorize]
    public async Task<IActionResult> Ocultar(int id)
    {
        try
        {
            var (success, message) = await _obraService.OcultarObraAsync(id);
            
            if (!success)
            {
                if (message == "Obra no encontrada")
                    return NotFound(new { message });
                
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/activar")]
    //[Authorize]
    public async Task<IActionResult> Activar(int id)
    {
        try
        {
            var (success, message) = await _obraService.ActivarObraAsync(id);
            
            if (!success)
            {
                if (message == "Obra no encontrada")
                    return NotFound(new { message });
                
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    //[Authorize]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var (success, message) = await _obraService.EliminarObraAsync(id);
            
            if (!success)
            {
                if (message == "Obra no encontrada")
                    return NotFound(new { message });
                
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/validar-firma")]
    public async Task<IActionResult> ValidarFirma(int id)
    {
        try
        {
            bool esValida = await _obraService.ValidarFirmaObraAsync(id);
            
            return Ok(new { 
                obraId = id,
                firmaValida = esValida,
                mensaje = esValida ? "La firma digital es válida" : "La firma digital no es válida"
            });
        }
        catch (FileNotFoundException ex)
        {
            return BadRequest(new { error = $"Archivo no encontrado: {ex.Message}" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = $"Error de configuración: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}