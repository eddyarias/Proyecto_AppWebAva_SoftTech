using ApiGateway.Hubs;

var builder = WebApplication.CreateBuilder(args);

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

// Agregar SignalR
builder.Services.AddSignalR();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Usar CORS
app.UseCors("AllowFrontend");

// Configurar SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.MapReverseProxy();

app.Run();