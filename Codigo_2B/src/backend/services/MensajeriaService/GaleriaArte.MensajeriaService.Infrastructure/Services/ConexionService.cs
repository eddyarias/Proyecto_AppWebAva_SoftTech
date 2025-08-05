using GaleriaArte.MensajeriaService.Application.Services;
using Microsoft.Extensions.Caching.Memory;

namespace GaleriaArte.MensajeriaService.Infrastructure.Services
{
    public class ConexionService : IConexionService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<ConexionService> _logger;
        private readonly TimeSpan _tiempoExpiracionEscritura = TimeSpan.FromMinutes(2);

        public ConexionService(IMemoryCache cache, ILogger<ConexionService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task RegistrarConexionAsync(string usuarioId, string connectionId)
        {
            try
            {
                var cacheKey = $"conexiones_{usuarioId}";
                var conexiones = _cache.Get<List<string>>(cacheKey) ?? new List<string>();
                
                if (!conexiones.Contains(connectionId))
                {
                    conexiones.Add(connectionId);
                    _cache.Set(cacheKey, conexiones, TimeSpan.FromHours(24));
                }

                _logger.LogInformation($"Conexión registrada: Usuario {usuarioId}, ConnectionId {connectionId}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar conexión");
            }
        }

        public async Task RegistrarDesconexionAsync(string connectionId)
        {
            try
            {
                // Buscar en todas las conexiones activas
                var todasLasClaves = GetAllCacheKeys().Where(k => k.StartsWith("conexiones_"));
                
                foreach (var clave in todasLasClaves)
                {
                    var conexiones = _cache.Get<List<string>>(clave);
                    if (conexiones != null && conexiones.Contains(connectionId))
                    {
                        conexiones.Remove(connectionId);
                        if (conexiones.Any())
                        {
                            _cache.Set(clave, conexiones, TimeSpan.FromHours(24));
                        }
                        else
                        {
                            _cache.Remove(clave);
                        }
                        break;
                    }
                }

                _logger.LogInformation($"Conexión removida: ConnectionId {connectionId}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar desconexión");
            }
        }

        public async Task<bool> EstaConectadoAsync(string usuarioId)
        {
            try
            {
                var cacheKey = $"conexiones_{usuarioId}";
                var conexiones = _cache.Get<List<string>>(cacheKey);
                return conexiones != null && conexiones.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar conexión");
                return false;
            }
        }

        public async Task<List<string>> ObtenerConexionesAsync(string usuarioId)
        {
            try
            {
                var cacheKey = $"conexiones_{usuarioId}";
                return _cache.Get<List<string>>(cacheKey) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conexiones");
                return new List<string>();
            }
        }

        public async Task IniciarEscrituraAsync(string usuarioId, string conversacionId)
        {
            try
            {
                var cacheKey = $"escribiendo_{usuarioId}";
                var escritura = new
                {
                    ConversacionId = conversacionId,
                    Timestamp = DateTime.UtcNow
                };
                
                _cache.Set(cacheKey, escritura, _tiempoExpiracionEscritura);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar escritura");
            }
        }

        public async Task DetenerEscrituraAsync(string usuarioId)
        {
            try
            {
                var cacheKey = $"escribiendo_{usuarioId}";
                _cache.Remove(cacheKey);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detener escritura");
            }
        }

        public async Task<bool> EstaEscribiendoAsync(string usuarioId, string conversacionId)
        {
            try
            {
                var cacheKey = $"escribiendo_{usuarioId}";
                var escritura = _cache.Get(cacheKey);
                
                if (escritura != null)
                {
                    var escrituraObj = escritura as dynamic;
                    return escrituraObj?.ConversacionId == conversacionId;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar escritura");
                return false;
            }
        }

        public async Task LimpiarConexionesAsync()
        {
            try
            {
                var todasLasClaves = GetAllCacheKeys()
                    .Where(k => k.StartsWith("conexiones_") || k.StartsWith("escribiendo_"))
                    .ToList();
                
                foreach (var clave in todasLasClaves)
                {
                    _cache.Remove(clave);
                }

                _logger.LogInformation("Cache de conexiones limpiado");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar conexiones");
            }
        }

        private IEnumerable<string> GetAllCacheKeys()
        {
            // Esta es una implementación simplificada
            // En un entorno de producción, podrías usar Redis o una implementación más robusta
            try
            {
                var field = typeof(MemoryCache).GetField("_coherentState",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    var coherentState = field.GetValue(_cache);
                    var entriesCollection = coherentState?.GetType()
                        .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (entriesCollection != null)
                    {
                        var entries = (IDictionary)entriesCollection.GetValue(coherentState);
                        return entries?.Keys.Cast<object>().Select(k => k.ToString()) ?? Enumerable.Empty<string>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener claves del cache");
            }
            
            return Enumerable.Empty<string>();
        }
    }
}
