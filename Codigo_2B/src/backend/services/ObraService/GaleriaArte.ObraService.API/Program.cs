using GaleriaArte.ObraService.Application.Interfaces;
using GaleriaArte.ObraService.Application.Services;
using GaleriaArte.ObraService.Domain.Interfaces;
using GaleriaArte.ObraService.Infrastructure.Data;
using GaleriaArte.ObraService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

// CONFIGURACIÓN GLOBAL PARA POSTGRESQL Y UTC
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Configuracion para pruebas de autenticación con Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Galeria Arte Obra API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' seguido de tu token JWT. Ejemplo: Bearer eyJhbGciOiJIUzI1..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication("ManualScheme")
    .AddScheme<AuthenticationSchemeOptions, ManualAuthenticationHandler>("ManualScheme", options => { });

// Database configuration
builder.Services.AddDbContext<ObraDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Register services and repositories
builder.Services.AddScoped<ObraRepository>();
builder.Services.AddScoped<IObraService, ObraService>();
builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<IDigitalSignatureService, DigitalSignatureService>();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
// Configurar la autenticacion y autorizacion para las URLs
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();
