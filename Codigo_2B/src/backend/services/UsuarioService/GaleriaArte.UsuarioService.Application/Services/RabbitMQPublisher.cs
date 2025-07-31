using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using GaleriaArte.UsuarioService.Application.Interfaces;
using GaleriaArte.UsuarioService.Application.DTOs;

namespace GaleriaArte.UsuarioService.Application.Services
{
    public class RabbitMQPublisher : IAuditoriaPublisher, IDisposable
    {
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private const string EXCHANGE_NAME = "auditoria_events";
        private const string ROUTING_KEY = "usuario.eventos";

        public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
        {
            _logger = logger;
            
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = configuration["RabbitMQ:UserName"] ?? "admin",
                    Password = configuration["RabbitMQ:Password"] ?? "admin123",
                    VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "galeria_arte"
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declarar el exchange
                _channel.ExchangeDeclare(
                    exchange: EXCHANGE_NAME,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                _logger.LogInformation("Conexión a RabbitMQ establecida correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar con RabbitMQ");
                // No lanzar excepción para evitar que la aplicación falle si RabbitMQ no está disponible
            }
        }

        public async Task PublishEventoAsync(EventoAuditoriaDto evento)
        {
            if (_channel == null)
            {
                _logger.LogWarning("No hay conexión disponible a RabbitMQ. Evento no enviado: {Evento}", evento.Evento);
                return;
            }

            try
            {
                var message = JsonSerializer.Serialize(evento);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = Guid.NewGuid().ToString();
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: EXCHANGE_NAME,
                    routingKey: ROUTING_KEY,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Evento de auditoría enviado: {Evento}", evento.Evento);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar evento de auditoría: {Evento}", evento.Evento);
                // No re-lanzar para evitar que falle la operación principal
            }
        }

        public async Task PublishUsuarioEventoAsync(string tipoEvento, string? usuarioId = null, string? rolId = null, string? ip = null, object? datos = null)
        {
            var evento = new UsuarioEventoAuditoria
            {
                Evento = tipoEvento,
                UsuarioId = usuarioId,
                RolId = rolId,
                Ip = ip,
                Datos = datos,
                Fecha = DateTime.UtcNow
            };

            await PublishEventoAsync(evento);
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar conexión RabbitMQ");
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
        }
    }
}