using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace ApiGateway.Services
{
    public class CircuitBreakerService
    {
        private readonly ILogger<CircuitBreakerService> _logger;

        public CircuitBreakerService(ILogger<CircuitBreakerService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Política de reintentos con backoff exponencial
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Maneja HttpRequestException y 5XX, 408
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            "Reintento {RetryCount} para {ServiceName} en {Delay}ms. Razón: {Reason}",
                            retryCount,
                            context.OperationKey,
                            timespan.TotalMilliseconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    });
        }

        /// <summary>
        /// Política de Circuit Breaker
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger, string serviceName)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3, // 3 fallos consecutivos
                    durationOfBreak: TimeSpan.FromSeconds(30), // 30 segundos abierto
                    onBreak: (exception, duration) =>
                    {
                        logger.LogError(
                            "🔴 Circuit Breaker ABIERTO para {ServiceName} por {Duration}s. Razón: {Reason}",
                            serviceName,
                            duration.TotalSeconds,
                            exception.Exception?.Message ?? exception.Result?.StatusCode.ToString());
                    },
                    onReset: () =>
                    {
                        logger.LogInformation(
                            "🟢 Circuit Breaker CERRADO para {ServiceName}. Servicio restaurado.",
                            serviceName);
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogWarning(
                            "🟡 Circuit Breaker SEMI-ABIERTO para {ServiceName}. Probando conectividad...",
                            serviceName);
                    });
        }

        /// <summary>
        /// Política de Timeout
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(ILogger logger, int timeoutSeconds = 10)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                timeout: TimeSpan.FromSeconds(timeoutSeconds),
                onTimeoutAsync: (context, timespan, task) =>
                {
                    logger.LogError(
                        "⏰ Timeout para {ServiceName} después de {Timeout}s",
                        context.OperationKey,
                        timespan.TotalSeconds);
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        /// Combina todas las políticas en una pipeline
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy(ILogger logger, string serviceName)
        {
            var timeout = GetTimeoutPolicy(logger, 15);
            var circuitBreaker = GetCircuitBreakerPolicy(logger, serviceName);
            var retry = GetRetryPolicy(logger);

            // Pipeline: Timeout -> Circuit Breaker -> Retry
            return Policy.WrapAsync(timeout, circuitBreaker, retry);
        }
    }
}
