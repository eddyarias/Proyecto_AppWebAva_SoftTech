using Prometheus;

namespace GaleriaArte.UsuarioService.API.Metrics
{
    public static class CustomMetrics
    {
        // Contador de requests por endpoint
        public static readonly Counter RequestsTotal = Prometheus.Metrics
            .CreateCounter("usuario_service_requests_total", "Total number of requests", new[] { "method", "endpoint", "status_code" });

        // Histograma de duración de requests
        public static readonly Histogram RequestDuration = Prometheus.Metrics
            .CreateHistogram("usuario_service_request_duration_seconds", "Duration of HTTP requests", new[] { "method", "endpoint" });

        // Gauge para usuarios activos
        public static readonly Gauge ActiveUsers = Prometheus.Metrics
            .CreateGauge("usuario_service_active_users", "Number of active users");

        // Contador de operaciones de base de datos
        public static readonly Counter DatabaseOperations = Prometheus.Metrics
            .CreateCounter("usuario_service_database_operations_total", "Total database operations", new[] { "operation", "success" });

        // Gauge para conexiones de base de datos activas
        public static readonly Gauge DatabaseConnections = Prometheus.Metrics
            .CreateGauge("usuario_service_database_connections", "Number of active database connections");

        // Contador de errores de autenticación
        public static readonly Counter AuthenticationErrors = Prometheus.Metrics
            .CreateCounter("usuario_service_authentication_errors_total", "Total authentication errors", new[] { "reason" });

        // Contador de usuarios registrados
        public static readonly Counter UsersRegistered = Prometheus.Metrics
            .CreateCounter("usuario_service_users_registered_total", "Total users registered");

        // Contador de logins exitosos
        public static readonly Counter SuccessfulLogins = Prometheus.Metrics
            .CreateCounter("usuario_service_successful_logins_total", "Total successful logins");
    }
}
