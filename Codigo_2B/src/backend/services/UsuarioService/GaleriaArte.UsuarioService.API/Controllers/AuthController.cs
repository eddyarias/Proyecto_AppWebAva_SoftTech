using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.UsuarioService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UsuarioLoginService _loginService;

    public AuthController(UsuarioLoginService loginService)
    {
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
                mensaje = "Inicio de sesión exitoso."
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
    }
}