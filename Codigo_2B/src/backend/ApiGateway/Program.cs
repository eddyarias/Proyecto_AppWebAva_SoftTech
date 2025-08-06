using ApiGateway.Hubs;
using ApiGateway.Services;
using ApiGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuración de logging mejorado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5000", "http://localhost:5001") // 👈 Pon el dominio exacto
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // 👈 Esto permite cookies
    });
});

// Registrar el CircuitBreakerService
builder.Services.AddScoped<CircuitBreakerService>();

// Agregar controladores para endpoints de salud
builder.Services.AddControllers();

// Agregar SignalR
builder.Services.AddSignalR();

// Configurar YARP con métricas y logging mejorado
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Configurar health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Usar CORS
app.UseCors("AllowFrontend");

// 🔧 Agregar middleware de Circuit Breaker ANTES del proxy
app.UseMiddleware<CircuitBreakerMiddleware>();

// Health checks endpoint
app.MapHealthChecks("/health");

// Mapear controladores
app.MapControllers();

// Configurar SignalR Hub
app.MapHub<ChatHub>("/chatHub");

// YARP Reverse Proxy (debe ir al final)
app.MapReverseProxy();

app.Run();