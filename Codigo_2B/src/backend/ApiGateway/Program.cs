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


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Usar CORS
app.UseCors("AllowFrontend");

app.MapReverseProxy();

app.Run();