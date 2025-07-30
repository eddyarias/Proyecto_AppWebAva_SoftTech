using Microsoft.EntityFrameworkCore;
using GaleriaArte.AuditoriaService.Infrastructure.Data;
using GaleriaArte.AuditoriaService.Infrastructure.Repositories;
using GaleriaArte.AuditoriaService.Application.Services;
using GaleriaArte.AuditoriaService.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de base de datos PostgreSQL
builder.Services.AddDbContext<AuditoriaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositorios
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

// RabbitMQ Consumer como servicio en background
builder.Services.AddHostedService<RabbitMQConsumer>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();