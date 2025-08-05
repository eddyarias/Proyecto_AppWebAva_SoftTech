using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace GaleriaArteFrontend.Services
{
    public interface IMensajeriaService
    {
        event Action<MensajeDto>? OnMensajeRecibido;
        event Action<string>? OnUsuarioConectado;
        event Action<string>? OnUsuarioDesconectado;
        event Action<string, string>? OnUsuarioEscribiendo;
        event Action<string>? OnUsuarioDejoDeEscribir;
        event Action<int>? OnContadorActualizado;
        
        Task<bool> ConectarAsync(string token);
        Task DesconectarAsync();
        Task<bool> EnviarMensajeAsync(string destinatarioId, string contenido);
        Task<bool> MarcarComoLeidoAsync(int conversacionId);
        Task<bool> NotificarEscribiendoAsync(string destinatarioId);
        Task<bool> NotificarDejoDeEscribirAsync(string destinatarioId);
        Task<List<ConversacionDto>> ObtenerConversacionesAsync();
        Task<List<MensajeDto>> ObtenerMensajesAsync(int conversacionId, int pagina = 1, int tamaño = 50);
        Task<List<UsuarioDto>> BuscarUsuariosAsync(string termino);
        Task<ConversacionDto> IniciarConversacionAsync(string destinatarioId);
        bool EstaConectado { get; }
    }

    public class MensajeriaService : IMensajeriaService, IAsyncDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MensajeriaService> _logger;
        private HubConnection? _hubConnection;
        private readonly string _apiBaseUrl;

        public event Action<MensajeDto>? OnMensajeRecibido;
        public event Action<string>? OnUsuarioConectado;
        public event Action<string>? OnUsuarioDesconectado;
        public event Action<string, string>? OnUsuarioEscribiendo;
        public event Action<string>? OnUsuarioDejoDeEscribir;
        public event Action<int>? OnContadorActualizado;

        public bool EstaConectado => _hubConnection?.State == HubConnectionState.Connected;

        public MensajeriaService(HttpClient httpClient, ILogger<MensajeriaService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiBaseUrl = configuration["ApiSettings:MensajeriaUrl"] ?? "http://localhost:5002";
        }

        public async Task<bool> ConectarAsync(string token)
        {
            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.DisposeAsync();
                }

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{_apiBaseUrl}/mensajeriahub", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(token);
                    })
                    .WithAutomaticReconnect()
                    .Build();

                // Configurar eventos del hub
                ConfigurarEventosHub();

                await _hubConnection.StartAsync();
                _logger.LogInformation("Conectado al hub de mensajería");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar al hub de mensajería");
                return false;
            }
        }

        public async Task DesconectarAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _logger.LogInformation("Desconectado del hub de mensajería");
            }
        }

        public async Task<bool> EnviarMensajeAsync(string destinatarioId, string contenido)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("EnviarMensaje", destinatarioId, contenido);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje");
                return false;
            }
        }

        public async Task<bool> MarcarComoLeidoAsync(int conversacionId)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("MarcarComoLeido", conversacionId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar como leído");
                return false;
            }
        }

        public async Task<bool> NotificarEscribiendoAsync(string destinatarioId)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("NotificarEscribiendo", destinatarioId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar escribiendo");
                return false;
            }
        }

        public async Task<bool> NotificarDejoDeEscribirAsync(string destinatarioId)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("NotificarDejoDeEscribir", destinatarioId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar dejó de escribir");
                return false;
            }
        }

        public async Task<List<ConversacionDto>> ObtenerConversacionesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/mensajeria/conversaciones");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ConversacionDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<ConversacionDto>();
                }
                return new List<ConversacionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversaciones");
                return new List<ConversacionDto>();
            }
        }

        public async Task<List<MensajeDto>> ObtenerMensajesAsync(int conversacionId, int pagina = 1, int tamaño = 50)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/mensajeria/conversaciones/{conversacionId}/mensajes?pagina={pagina}&tamaño={tamaño}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<MensajeDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<MensajeDto>();
                }
                return new List<MensajeDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensajes");
                return new List<MensajeDto>();
            }
        }

        public async Task<List<UsuarioDto>> BuscarUsuariosAsync(string termino)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/mensajeria/usuarios/buscar?termino={Uri.EscapeDataString(termino)}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UsuarioDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<UsuarioDto>();
                }
                return new List<UsuarioDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuarios");
                return new List<UsuarioDto>();
            }
        }

        public async Task<ConversacionDto> IniciarConversacionAsync(string destinatarioId)
        {
            try
            {
                var requestBody = JsonSerializer.Serialize(new { DestinatarioId = destinatarioId });
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/mensajeria/conversaciones", content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ConversacionDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new ConversacionDto();
                }
                return new ConversacionDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar conversación");
                return new ConversacionDto();
            }
        }

        private void ConfigurarEventosHub()
        {
            if (_hubConnection == null) return;

            _hubConnection.On<string>("MensajeRecibido", (mensajeJson) =>
            {
                try
                {
                    var mensaje = JsonSerializer.Deserialize<MensajeDto>(mensajeJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (mensaje != null)
                    {
                        OnMensajeRecibido?.Invoke(mensaje);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar mensaje recibido");
                }
            });

            _hubConnection.On<string>("UsuarioConectado", (usuarioId) =>
            {
                OnUsuarioConectado?.Invoke(usuarioId);
            });

            _hubConnection.On<string>("UsuarioDesconectado", (usuarioId) =>
            {
                OnUsuarioDesconectado?.Invoke(usuarioId);
            });

            _hubConnection.On<string, string>("UsuarioEscribiendo", (usuarioId, conversacionId) =>
            {
                OnUsuarioEscribiendo?.Invoke(usuarioId, conversacionId);
            });

            _hubConnection.On<string>("UsuarioDejoDeEscribir", (usuarioId) =>
            {
                OnUsuarioDejoDeEscribir?.Invoke(usuarioId);
            });

            _hubConnection.On<int>("ContadorMensajesActualizado", (contador) =>
            {
                OnContadorActualizado?.Invoke(contador);
            });

            _hubConnection.Reconnecting += (exception) =>
            {
                _logger.LogWarning("Reconectando al hub de mensajería...");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                _logger.LogInformation("Reconectado al hub de mensajería");
                return Task.CompletedTask;
            };

            _hubConnection.Closed += (exception) =>
            {
                _logger.LogWarning("Conexión al hub de mensajería cerrada");
                return Task.CompletedTask;
            };
        }

        public async ValueTask DisposeAsync()
        {
            await DesconectarAsync();
        }
    }

    // DTOs para el frontend
    public class MensajeDto
    {
        public int Id { get; set; }
        public int ConversacionId { get; set; }
        public string EmisorId { get; set; } = string.Empty;
        public string EmisorNombre { get; set; } = string.Empty;
        public string EmisorAvatar { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
        public string TipoMensaje { get; set; } = "Texto";
    }

    public class ConversacionDto
    {
        public int Id { get; set; }
        public string ParticipanteId { get; set; } = string.Empty;
        public string ParticipanteNombre { get; set; } = string.Empty;
        public string ParticipanteAvatar { get; set; } = string.Empty;
        public bool ParticipanteEnLinea { get; set; }
        public DateTime? UltimaActividad { get; set; }
        public string UltimoMensaje { get; set; } = string.Empty;
        public DateTime? FechaUltimoMensaje { get; set; }
        public int MensajesNoLeidos { get; set; }
    }

    public class UsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public bool EnLinea { get; set; }
        public DateTime? UltimaActividad { get; set; }
        public string Rol { get; set; } = string.Empty;
    }
}
