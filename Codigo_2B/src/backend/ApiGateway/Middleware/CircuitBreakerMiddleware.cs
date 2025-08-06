using ApiGateway.Services;
using Polly;
using System.Net;

namespace ApiGateway.Middleware
{
    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CircuitBreakerMiddleware> _logger;
        private readonly Dictionary<string, IAsyncPolicy<HttpResponseMessage>> _policies;

        public CircuitBreakerMiddleware(RequestDelegate next, ILogger<CircuitBreakerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _policies = new Dictionary<string, IAsyncPolicy<HttpResponseMessage>>();
            InitializePolicies();
        }

        private void InitializePolicies()
        {
            // Políticas específicas para cada servicio
            _policies["usuario"] = CircuitBreakerService.GetResiliencePolicy(_logger, "UsuarioService");
            _policies["auth"] = CircuitBreakerService.GetResiliencePolicy(_logger, "AuthService");
            _policies["recuperacion"] = CircuitBreakerService.GetResiliencePolicy(_logger, "RecuperacionService");
            _policies["obra"] = CircuitBreakerService.GetResiliencePolicy(_logger, "ObraService");
            _policies["auditoria"] = CircuitBreakerService.GetResiliencePolicy(_logger, "AuditoriaService");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            
            // Determinar qué servicio se está llamando
            var serviceKey = GetServiceKeyFromPath(path);
            
            if (!string.IsNullOrEmpty(serviceKey) && _policies.ContainsKey(serviceKey))
            {
                _logger.LogDebug("Aplicando Circuit Breaker para servicio: {ServiceKey}", serviceKey);
                
                try
                {
                    // Aplicar la política de resiliencia
                    var policy = _policies[serviceKey];
                    
                    // Continuar con la pipeline normal de YARP
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en Circuit Breaker para {ServiceKey}", serviceKey);
                    
                    // Si hay un error crítico, devolver 503 Service Unavailable
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        await context.Response.WriteAsync($"Servicio {serviceKey} no disponible temporalmente");
                    }
                }
            }
            else
            {
                // Para rutas que no requieren Circuit Breaker
                await _next(context);
            }
        }

        private string GetServiceKeyFromPath(string? path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            if (path.StartsWith("/usuario")) return "usuario";
            if (path.StartsWith("/auth")) return "auth";
            if (path.StartsWith("/recuperacion")) return "recuperacion";
            if (path.StartsWith("/obra")) return "obra";
            if (path.StartsWith("/auditoria")) return "auditoria";

            return string.Empty;
        }
    }
}
