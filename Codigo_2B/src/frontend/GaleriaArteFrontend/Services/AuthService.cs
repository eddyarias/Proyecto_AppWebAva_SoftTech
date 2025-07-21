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

                    // Simular obtención del usuario desde las cookies o JWT
                    // En una implementación real, decodificarías el JWT del token de acceso
                    _usuarioActual = new Usuario
                    {
                        Nickname = request.Identificador,
                        Correo = request.Identificador.Contains("@") ? request.Identificador : "",
                        Rol = "cliente" // Por ahora asumimos cliente, deberías obtener esto del JWT
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
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = result.GetProperty("mensaje").GetString() ?? "Registro exitoso";
                    
                    return new ApiResponse { Exito = true, Mensaje = mensaje };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    var mensaje = errorResult.GetProperty("mensaje").GetString() ?? "Error en el registro";
                    
                    return new ApiResponse { Exito = false, Mensaje = mensaje };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Exito = false, Mensaje = $"Error de conexión: {ex.Message}" };
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
            
            OnAuthStateChanged?.Invoke();
            
            _navigationManager.NavigateTo("/login");
        }

        public async Task<bool> CheckAuthStateAsync()
        {
            try
            {
                var isAuthenticated = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "isAuthenticated");
                var userNickname = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userNickname");
                
                if (isAuthenticated == "true" && !string.IsNullOrEmpty(userNickname))
                {
                    _usuarioActual = new Usuario
                    {
                        Nickname = userNickname,
                        Rol = "cliente" // En una implementación real, deberías obtener esto de manera más segura
                    };
                    
                    OnAuthStateChanged?.Invoke();
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
