using Prometheus;

namespace GaleriaArte.ObraService.API.Metrics
{
    public static class CustomMetrics
    {
        // Total requests counter
        public static readonly Counter RequestsTotal = Prometheus.Metrics
            .CreateCounter(
                "obra_service_requests_total",
                "Total number of requests processed by ObraService",
                new[] { "method", "endpoint", "status_code" });

        // Request duration histogram
        public static readonly Histogram RequestDuration = Prometheus.Metrics
            .CreateHistogram(
                "obra_service_request_duration_seconds",
                "Duration of requests processed by ObraService in seconds",
                new[] { "method", "endpoint" });

        // Active users gauge
        public static readonly Gauge ActiveObras = Prometheus.Metrics
            .CreateGauge(
                "obra_service_active_obras",
                "Number of active obras in the system");

        // Database operations counter
        public static readonly Counter DatabaseOperations = Prometheus.Metrics
            .CreateCounter(
                "obra_service_database_operations_total",
                "Total number of database operations",
                new[] { "operation", "table" });

        // Business-specific metrics
        public static readonly Counter DigitalSignatureOperations = Prometheus.Metrics
            .CreateCounter(
                "obra_service_digital_signature_operations_total",
                "Total number of digital signature operations",
                new[] { "operation", "result" });

        // Database connections gauge
        public static readonly Gauge DatabaseConnections = Prometheus.Metrics
            .CreateGauge(
                "obra_service_database_connections",
                "Number of active database connections");
    }
}
