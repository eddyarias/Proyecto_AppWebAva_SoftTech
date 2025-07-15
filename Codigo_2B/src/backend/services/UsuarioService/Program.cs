using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// ✅ Agregar servicios de controladores
builder.Services.AddControllers();

// Si usas Swagger:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// ✅ Middleware para redireccionar solicitudes a los controladores
app.UseRouting();

app.UseAuthorization(); // o .UseAuthentication() si ya lo usas

app.MapControllers(); // <--- ESTA LÍNEA ES NECESARIA

app.Run();