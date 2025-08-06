var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.SetIsOriginAllowed(_ => true) // 👈 Permite cualquier origen
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