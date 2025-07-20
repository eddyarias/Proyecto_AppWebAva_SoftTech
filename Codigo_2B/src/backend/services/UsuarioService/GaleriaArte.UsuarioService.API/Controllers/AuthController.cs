using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Application.Interfaces;
using GaleriaArte.UsuarioService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.UsuarioService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UsuarioLoginService _loginService;
    private readonly IUsuarioService _usuarioService;

    public AuthController(UsuarioLoginService loginService, IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
        _loginService = loginService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var result = await _loginService.LoginAsync(request);
            // Enviar refreshToken como cookie segura
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // usa HTTPS en producción
                SameSite = SameSiteMode.Strict, // o Lax si se necesita compatibilidad
                Expires = DateTime.UtcNow.AddDays(7)
            });

            Response.Cookies.Append("acces_token", result.TokenAcceso, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // usa HTTPS en producción
                SameSite = SameSiteMode.Strict, // o Lax si se necesita compatibilidad
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                accessToken = result.TokenAcceso,
                refreshToken = result.RefreshToken,
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
    }
    
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var resultado = await _usuarioService.RenovarTokenAsync(dto.RefreshToken);
        if (resultado == null)
        {
            return Unauthorized(new { mensaje = "Refresh token inválido o expirado." });
        }

        Response.Cookies.Append("refreshToken", resultado.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // usa HTTPS en producción
            SameSite = SameSiteMode.Strict, // o Lax si se necesita compatibilidad
            Expires = DateTime.UtcNow.AddDays(7)
        });

        Response.Cookies.Append("acces_token", resultado.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // usa HTTPS en producción
            SameSite = SameSiteMode.Strict, // o Lax si se necesita compatibilidad
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        return Ok();
    }
}