using GaleriaArteFrontend.Models;
using System.Text.Json;
using System.Net.Http.Headers;

namespace GaleriaArteFrontend.Services
{
    public class AuditoriaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuditoriaService> _logger;
        private readonly JwtService _jwtService;
        private readonly string _baseUrl = "";

        public AuditoriaService(HttpClient httpClient, ILogger<AuditoriaService> logger, JwtService jwtService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jwtService = jwtService;
        }

        public async Task<List<LogEvento>> ObtenerPorMicroservicioAsync(string microservicio, int limite = 100)
        {
            try
            {
                await AddJwtHeaderAsync();
                var response = await _httpClient.GetAsync($"{_baseUrl}/auditoria/microservicio/{microservicio}?limite={limite}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<LogEvento>>(json, options) ?? new List<LogEvento>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por microservicio: {Microservicio}", microservicio);
                return new List<LogEvento>();
            }
        }

        public async Task<List<LogEvento>> ObtenerPorUsuarioAsync(string usuarioId, int limite = 100)
        {
            try
            {
                await AddJwtHeaderAsync();
                var response = await _httpClient.GetAsync($"{_baseUrl}/auditoria/usuario/{usuarioId}?limite={limite}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<LogEvento>>(json, options) ?? new List<LogEvento>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por usuario: {UsuarioId}", usuarioId);
                return new List<LogEvento>();
            }
        }

        public async Task<List<LogEvento>> ObtenerConFiltrosAsync(FiltrosAuditoria filtros)
        {
            try
            {
                // Determinar qué endpoint usar basado en los filtros
                if (!string.IsNullOrEmpty(filtros.Microservicio))
                {
                    var resultados = await ObtenerPorMicroservicioAsync(filtros.Microservicio, filtros.Limite);
                    return resultados.Take(filtros.Limite).ToList();
                }
                else if (!string.IsNullOrEmpty(filtros.UsuarioId))
                {
                    var resultados = await ObtenerPorUsuarioAsync(filtros.UsuarioId, filtros.Limite);
                    return resultados.Take(filtros.Limite).ToList();
                }
                else
                {
                    // Si no hay filtros específicos, retornar lista vacía
                    return new List<LogEvento>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs con filtros");
                return new List<LogEvento>();
            }
        }

        private async Task AddJwtHeaderAsync()
        {
            var token = await _jwtService.ObtenerTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}