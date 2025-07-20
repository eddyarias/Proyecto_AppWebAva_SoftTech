using System.Diagnostics;
using GaleriaArte.ObraService.API.Metrics;

namespace GaleriaArte.ObraService.API.Middleware
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public MetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var endpoint = context.Request.Path.Value ?? "unknown";

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode.ToString();

                // Record metrics
                CustomMetrics.RequestsTotal
                    .WithLabels(method, endpoint, statusCode)
                    .Inc();

                CustomMetrics.RequestDuration
                    .WithLabels(method, endpoint)
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }

    public static class MetricsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMetrics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MetricsMiddleware>();
        }
    }
}
