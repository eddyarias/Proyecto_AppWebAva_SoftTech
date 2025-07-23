using System.Net.Http.Json;
using System.Text.Json;
using GaleriaArteFrontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GaleriaArteFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private Usuario? _usuarioActual;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        public Usuario? UsuarioActual => _usuarioActual;
        public bool EstaAutenticado => _usuarioActual != null;

        public event Action? OnAuthStateChanged;

        public async Task<ApiResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = result.GetProperty("mensaje").GetString() ?? "Login exitoso";
                    string? accessToken = null;
                    if (result.TryGetProperty("accessToken", out var tokenProp))
                        accessToken = tokenProp.GetString();

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt_token", accessToken);
                    }

                    // Obtener datos reales del usuario desde el JWT
                    string nickname = request.Identificador;
                    string correo = request.Identificador.Contains("@") ? request.Identificador : "";
                    string rol = "";
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        var jwtService = new JwtService(_jsRuntime);
                        var payload = jwtService.ObtenerPayload(accessToken);
                        // El claim de rol es el ID, usar el mismo claim que en el backend
                        rol = jwtService.ObtenerClaim(payload, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") ?? "";
                        // Si el nickname está en el token, úsalo
                        var nickFromToken = jwtService.ObtenerClaim(payload, "nickname");
                        if (!string.IsNullOrEmpty(nickFromToken)) nickname = nickFromToken;
                        var correoFromToken = jwtService.ObtenerClaim(payload, "email");
                        if (!string.IsNullOrEmpty(correoFromToken)) correo = correoFromToken;
                    }

                    _usuarioActual = new Usuario
                    {
                        Nickname = nickname,
                        Correo = correo,
                        Rol = rol
                    };

                    // Guardar estado en localStorage
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "isAuthenticated", "true");
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userNickname", _usuarioActual.Nickname);

                    OnAuthStateChanged?.Invoke();

                    return new ApiResponse { Exito = true, Mensaje = mensaje };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    var mensaje = errorResult.GetProperty("mensaje").GetString() ?? "Error de autenticación";

                    return new ApiResponse { Exito = false, Mensaje = mensaje };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Exito = false, Mensaje = $"Error de conexión: {ex.Message}" };
            }
        }

        public async Task<ApiResponse> RegistrarAsync(RegistroRequest request)
        {
            try
            {
                var usuarioDto = new
                {
                    Nickname = request.Nickname,
                    Correo = request.Correo,
                    Contraseña = request.Contraseña,
                    Rol = request.Rol
                };

                var response = await _httpClient.PostAsJsonAsync("usuario/registrar", usuarioDto);

                // Imprimir el response en consola

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Exito = true, Mensaje = "Registro exitoso" };
                }
                else
                {
                    return new ApiResponse { Exito = false, Mensaje = "Nickname/correo ya en uso" };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Exito = false, Mensaje = $"Hubo un error intente de nuevo" };
            }
        }

        public async Task<ApiResponse> SolicitarRecuperacionAsync(SolicitarRecuperacionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("recuperacion/solicitar", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = result.GetProperty("mensaje").GetString() ?? "Instrucciones enviadas";

                    return new ApiResponse { Exito = true, Mensaje = mensaje };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    var mensaje = errorResult.GetProperty("mensaje").GetString() ?? "Error al solicitar recuperación";

                    return new ApiResponse { Exito = false, Mensaje = mensaje };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Exito = false, Mensaje = $"Error de conexión: {ex.Message}" };
            }
        }

        public async Task<ApiResponse> RestablecerPasswordAsync(RestablecerPasswordRequest request)
        {
            try
            {
                var dto = new
                {
                    Token = request.Token,
                    NuevaPassword = request.NuevaPassword
                };

                var response = await _httpClient.PostAsJsonAsync("recuperacion/restablecer", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = result.GetProperty("mensaje").GetString() ?? "Contraseña restablecida";

                    return new ApiResponse { Exito = true, Mensaje = mensaje };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    var mensaje = errorResult.GetProperty("mensaje").GetString() ?? "Error al restablecer contraseña";

                    return new ApiResponse { Exito = false, Mensaje = mensaje };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Exito = false, Mensaje = $"Error de conexión: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            _usuarioActual = null;

            // Limpiar localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "isAuthenticated");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userNickname");

            // Eliminar JWT
            var jwtService = new JwtService(_jsRuntime);
            await jwtService.EliminarTokenAsync();

            OnAuthStateChanged?.Invoke();

            _navigationManager.NavigateTo("/login");
        }

        public async Task<bool> CheckAuthStateAsync()
        {
            try
            {
                var jwtService = new JwtService(_jsRuntime);
                var token = await jwtService.ObtenerTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    var payload = jwtService.ObtenerPayload(token);
                    var rol = jwtService.ObtenerClaim(payload, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") ?? "";
                    var nickname = jwtService.ObtenerClaim(payload, "nickname") ?? "";
                    var correo = jwtService.ObtenerClaim(payload, "email") ?? "";
                    _usuarioActual = new Usuario
                    {
                        Nickname = nickname,
                        Correo = correo,
                        Rol = rol
                    };
                    OnAuthStateChanged?.Invoke();
                    return true;
                }
                _usuarioActual = null;
                return false;
            }
            catch
            {
                _usuarioActual = null;
                return false;
            }
        }
    }
}
