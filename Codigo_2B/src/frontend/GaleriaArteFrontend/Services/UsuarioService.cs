using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using GaleriaArteFrontend.Models;
using Microsoft.JSInterop;

namespace GaleriaArteFrontend.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly JwtService _jwtService;

        public UsuarioService(HttpClient httpClient, JwtService jwtService)
        {
            _httpClient = httpClient;
            _jwtService = jwtService;
        }

        private async Task AddJwtHeaderAsync()
        {
            var token = await _jwtService.ObtenerTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<UsuarioListItem>> ListarUsuariosAsync()
        {
            try
            {
                await AddJwtHeaderAsync();
                var response = await _httpClient.GetAsync("usuario/listar");

                if (response.IsSuccessStatusCode)
                {
                    var usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioListItem>>();
                    return usuarios ?? new List<UsuarioListItem>();
                }
                else
                {
                    throw new Exception("Error al obtener la lista de usuarios");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error de conexión: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CambiarEstadoUsuarioAsync(Guid usuarioId, bool nuevoEstado)
        {
            try
            {
                await AddJwtHeaderAsync();
                
                var dto = new
                {
                    UsuarioId = usuarioId,
                    NuevoEstado = nuevoEstado
                };

                var response = await _httpClient.PatchAsJsonAsync("usuario/cambiar-estado", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = result.GetProperty("mensaje").GetString() ?? "Estado cambiado correctamente";
                    
                    return new ApiResponse { Exito = true, Mensaje = mensaje };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        var mensaje = errorResult.GetProperty("mensaje").GetString() ?? "Error al cambiar estado del usuario";
                        return new ApiResponse { Exito = false, Mensaje = mensaje };
                    }
                    catch
                    {
                        return new ApiResponse { Exito = false, Mensaje = "Error al cambiar estado del usuario" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Exito = false, Mensaje = $"Error de conexión: {ex.Message}" };
            }
        }

        public async Task<UsuarioListItem?> ObtenerUsuarioActualAsync()
        {
            try
            {
                await AddJwtHeaderAsync();
                var response = await _httpClient.GetAsync("usuario/perfil");

                if (response.IsSuccessStatusCode)
                {
                    var usuario = await response.Content.ReadFromJsonAsync<UsuarioListItem>();
                    return usuario;
                }
                else
                {
                    // Si no existe endpoint de perfil, intentar extraer del token y buscar en la lista
                    var payload = await _jwtService.ObtenerPayloadAsync();
                    if (payload != null)
                    {
                        var nickname = _jwtService.ObtenerClaim(payload, "nickname");
                        if (!string.IsNullOrEmpty(nickname))
                        {
                            var usuarios = await ListarUsuariosAsync();
                            return usuarios.FirstOrDefault(u => u.Nickname == nickname);
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo usuario actual: {ex.Message}");
                // Fallback: intentar extraer del token
                try
                {
                    var payload = await _jwtService.ObtenerPayloadAsync();
                    if (payload != null)
                    {
                        var nickname = _jwtService.ObtenerClaim(payload, "nickname");
                        if (!string.IsNullOrEmpty(nickname))
                        {
                            var usuarios = await ListarUsuariosAsync();
                            return usuarios.FirstOrDefault(u => u.Nickname == nickname);
                        }
                    }
                }
                catch (Exception fallbackEx)
                {
                    Console.WriteLine($"Error en fallback: {fallbackEx.Message}");
                }
                return null;
            }
        }
    }
}