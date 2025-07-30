using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using GaleriaArte.AuditoriaService.Application.DTOs;
using GaleriaArte.AuditoriaService.Domain.Interfaces;
using GaleriaArte.AuditoriaService.Domain.Entities;

namespace GaleriaArte.AuditoriaService.Application.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IModel? _channel;

        private const string EXCHANGE_NAME = "auditoria_events";
        private const string QUEUE_NAME = "auditoria_queue";
        private const string ROUTING_KEY = "usuario.eventos";

        public RabbitMQConsumer(
            IServiceProvider serviceProvider,
            ILogger<RabbitMQConsumer> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken); // Esperar a que RabbitMQ esté listo

            InitializeRabbitMQ();

            stoppingToken.Register(() =>
            {
                _channel?.Close();
                _connection?.Close();
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMQ:UserName"] ?? "admin",
                    Password = _configuration["RabbitMQ:Password"] ?? "admin123",
                    VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "galeria_arte"
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: EXCHANGE_NAME,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                _channel.QueueDeclare(
                    queue: QUEUE_NAME,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueBind(
                    queue: QUEUE_NAME,
                    exchange: EXCHANGE_NAME,
                    routingKey: ROUTING_KEY);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    await ProcessMessage(ea);
                };

                _channel.BasicConsume(
                    queue: QUEUE_NAME,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("Consumer de auditoría iniciado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar RabbitMQ Consumer");
                throw;
            }
        }

        private async Task ProcessMessage(BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var eventoDto = JsonSerializer.Deserialize<EventoAuditoriaDto>(message);
                
                if (eventoDto != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IAuditoriaRepository>();

                    var logEvento = new LogEvento
                    {
                        Microservicio = eventoDto.Microservicio,
                        Evento = eventoDto.Evento,
                        UsuarioId = eventoDto.UsuarioId,
                        RolId = eventoDto.RolId,
                        Ip = eventoDto.Ip,
                        Datos = eventoDto.DatosJson,
                        Fecha = eventoDto.Fecha
                    };

                    await repository.CrearLogEventoAsync(logEvento);
                    
                    _logger.LogInformation("Evento de auditoría procesado: {Evento} - {Microservicio}", 
                        eventoDto.Evento, eventoDto.Microservicio);

                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar mensaje de auditoría");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}