using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace GaleriaArteFrontend.Services
{
    public class ChatService : IDisposable
    {
        private HubConnection? _hubConnection;
        private readonly IJSRuntime _jsRuntime;
        private readonly JwtService _jwtService;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        private string _cachedUserName = string.Empty;
        private DateTime _lastUserCheck = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

        public ChatService(IJSRuntime jsRuntime, JwtService jwtService, AuthService authService, IConfiguration configuration)
        {
            _jsRuntime = jsRuntime;
            _jwtService = jwtService;
            _authService = authService;
            _configuration = configuration;
        }

        public async Task StartAsync()
        {
            // Si ya hay una conexión activa, no crear otra
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                Console.WriteLine("SignalR ya está conectado, no se creará nueva conexión");
                return;
            }

            // Si hay una conexión previa en otro estado, cerrarla
            if (_hubConnection != null)
            {
                try
                {
                    await _hubConnection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error cerrando conexión previa: {ex.Message}");
                }
                _hubConnection = null;
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_configuration["ApiGateway:BaseUrl"] ?? "http://localhost:5000"}/chatHub") // URL del SignalR Hub en ApiGateway
                .Build();

            // Mensajes públicos (broadcast)
            _hubConnection.On<string, string>("broadcastMessage", (name, message) =>
            {
                Console.WriteLine($"Mensaje público recibido: {name}: {message}");
                OnMessageReceived?.Invoke(name, message);
            });

            // Mensajes privados
            _hubConnection.On<string, string, string>("privateMessage", (fromUser, toUser, message) =>
            {
                Console.WriteLine($"Mensaje privado recibido: {fromUser} -> {toUser}: {message}");
                OnPrivateMessageReceived?.Invoke(fromUser, toUser, message);
            });

            await _hubConnection.StartAsync();
            Console.WriteLine("Conexión SignalR establecida");

            // Registrar usuario en el hub para mensajes privados
            var userName = await GetUserNameAsync();
            Console.WriteLine($"Registrando usuario en hub: {userName}");
            
            if (_hubConnection?.State == HubConnectionState.Connected && !string.IsNullOrEmpty(userName) && userName != "Invitado")
            {
                await _hubConnection.SendAsync("RegisterUser", userName);
                Console.WriteLine($"Usuario {userName} registrado en hub para mensajes privados");
            }
            else
            {
                Console.WriteLine($"No se pudo registrar usuario. Estado conexión: {_hubConnection?.State}, Usuario: {userName}");
            }
        }

        public async Task SendMessageAsync(string name, string message)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("Send", name, message);
            }
        }

        public async Task SendPrivateMessageAsync(string fromUser, string toUser, string message)
        {
            Console.WriteLine($"Intentando enviar mensaje privado: {fromUser} -> {toUser}: {message}");
            
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.SendAsync("SendPrivateMessage", fromUser, toUser, message);
                    Console.WriteLine($"Mensaje privado enviado exitosamente");
                    
                    // Guardar mensaje privado enviado
                    await SavePrivateMessageAsync(fromUser, toUser, message, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando mensaje privado: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"No se puede enviar mensaje - Estado conexión: {_hubConnection?.State}");
            }
        }

        public event Action<string, string>? OnMessageReceived;
        public event Action<string, string, string>? OnPrivateMessageReceived;

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        // NO cargar historial al iniciar - solo mensajes nuevos desde la sesión actual
        public async Task SaveMessageAsync(string name, string message)
        {
            // Solo para referencia, no se carga historial al iniciar
            try
            {
                var history = new List<ChatMessage> { new ChatMessage { Name = name, Message = message, Timestamp = DateTime.Now } };
                var json = JsonSerializer.Serialize(history);
                await _jsRuntime.InvokeVoidAsync("console.log", $"Mensaje guardado: {name}: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando mensaje: {ex.Message}");
            }
        }
        public Task<List<ChatMessage>> GetChatHistoryAsync()
        {
            // Retornar lista vacía - no cargar historial previo
            return Task.FromResult(new List<ChatMessage>());
        }

        // Gestión de mensajes privados
        public async Task SavePrivateMessageAsync(string fromUser, string toUser, string message, bool isSent = false)
        {
            try
            {
                var privateMessages = await GetPrivateMessagesAsync();
                privateMessages.Add(new PrivateMessage 
                { 
                    FromUser = fromUser, 
                    ToUser = toUser, 
                    Message = message, 
                    Timestamp = DateTime.Now,
                    IsSent = isSent
                });

                // Mantener solo los últimos 200 mensajes privados
                if (privateMessages.Count > 200)
                {
                    privateMessages.RemoveAt(0);
                }

                var json = JsonSerializer.Serialize(privateMessages);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "privateMessages", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando mensaje privado: {ex.Message}");
            }
        }

        public async Task<List<PrivateMessage>> GetPrivateMessagesAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "privateMessages");
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<List<PrivateMessage>>(json) ?? new List<PrivateMessage>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo mensajes privados: {ex.Message}");
            }
            return new List<PrivateMessage>();
        }

        public async Task<List<PrivateMessage>> GetConversationAsync(string otherUser)
        {
            var currentUser = await GetUserNameAsync();
            var allPrivateMessages = await GetPrivateMessagesAsync();
            
            return allPrivateMessages
                .Where(m => (m.FromUser == currentUser && m.ToUser == otherUser) || 
                           (m.FromUser == otherUser && m.ToUser == currentUser))
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        public async Task<string> GetUserNameAsync()
        {
            // Usar caché para evitar verificaciones constantes
            if (!string.IsNullOrEmpty(_cachedUserName) && 
                DateTime.Now - _lastUserCheck < _cacheExpiry)
            {
                return _cachedUserName;
            }

            try
            {
                if (_authService.EstaAutenticado && _authService.UsuarioActual != null)
                {
                    _cachedUserName = _authService.UsuarioActual.Nickname;
                    _lastUserCheck = DateTime.Now;
                    return _cachedUserName;
                }
                else
                {
                    _cachedUserName = "Invitado";
                    _lastUserCheck = DateTime.Now;
                    return _cachedUserName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo usuario: {ex.Message}");
                _cachedUserName = "Invitado";
                _lastUserCheck = DateTime.Now;
                return _cachedUserName;
            }
        }

        public void ClearUserCache()
        {
            _cachedUserName = string.Empty;
            _lastUserCheck = DateTime.MinValue;
        }

        public async Task OnUserLoggedInAsync()
        {
            // Limpiar cache para forzar nueva obtención del usuario
            ClearUserCache();
            
            // Obtener nuevo username
            var userName = await GetUserNameAsync();
            Console.WriteLine($"Usuario logueado: {userName}");
            
            // Re-registrar en el hub si está conectado (sin crear nueva conexión)
            if (_hubConnection?.State == HubConnectionState.Connected && !string.IsNullOrEmpty(userName) && userName != "Invitado")
            {
                await _hubConnection.SendAsync("RegisterUser", userName);
                Console.WriteLine($"Usuario {userName} re-registrado en hub para mensajes privados");
            }

            // Disparar evento para que la UI pueda recargar mensajes privados
            OnUserChanged?.Invoke(userName);
        }

        public async Task ReconnectWithNewUserAsync()
        {
            // Método específico para reconectar cuando cambia el usuario
            var userName = await GetUserNameAsync();
            
            if (_hubConnection?.State == HubConnectionState.Connected && !string.IsNullOrEmpty(userName) && userName != "Invitado")
            {
                // Solo re-registrar el usuario sin crear nueva conexión
                await _hubConnection.SendAsync("RegisterUser", userName);
                Console.WriteLine($"Usuario {userName} re-registrado en hub existente");
            }
            else if (_hubConnection?.State != HubConnectionState.Connected)
            {
                // Si no hay conexión, crearla
                await StartAsync();
            }
        }

        public event Action<string>? OnUserChanged;

        public async Task<bool> IsUserLoggedInAsync()
        {
            return _authService.EstaAutenticado;
        }

        public async Task<string> GetUserInfoDebugAsync()
        {
            try
            {
                if (_authService.EstaAutenticado && _authService.UsuarioActual != null)
                {
                    var usuario = _authService.UsuarioActual;
                    return $"Usuario autenticado: Nickname={usuario.Nickname}, Rol={usuario.Rol}";
                }
                else
                {
                    return "Usuario no autenticado";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public class ChatMessage
        {
            public string Name { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        public class PrivateMessage
        {
            public string FromUser { get; set; } = string.Empty;
            public string ToUser { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public bool IsSent { get; set; } // true si fue enviado por el usuario actual
        }
    }
}
