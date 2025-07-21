using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace GaleriaArteFrontend.Services
{
    public class JwtService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "jwt_token";

        public JwtService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // Guardar el token en localStorage
        public async Task GuardarTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        // Obtener el token de localStorage
        public async Task<string?> ObtenerTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
        }

        // Eliminar el token de localStorage
        public async Task EliminarTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }

        // Obtener el payload del JWT
        public JwtPayload? ObtenerPayload(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Payload;
        }

        // Obtener el payload del JWT desde localStorage
        public async Task<JwtPayload?> ObtenerPayloadAsync()
        {
            var token = await ObtenerTokenAsync();
            return ObtenerPayload(token);
        }

        // Obtener un claim específico del payload
        public string? ObtenerClaim(JwtPayload payload, string claimType)
        {
            if (payload == null) return null;
            if (payload.Claims == null) return null;
            var claim = payload.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
