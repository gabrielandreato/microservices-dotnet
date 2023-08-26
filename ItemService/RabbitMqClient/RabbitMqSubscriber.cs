using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestauranteService.EventProcessor;

namespace ItemService.RabbitMqClient; 

public class RabbitMqSubscriber: BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly string _nomeDaFila;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private IProcessaEvento _processaEvento;

    public RabbitMqSubscriber(IConfiguration configuration, IProcessaEvento processaEvento) {
        _configuration = configuration;
        _connection = new ConnectionFactory() {
                HostName = _configuration["RabbitMqHost"], 
                Port = int.Parse(_configuration["RabbitMqPort"])
            }.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _nomeDaFila = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _nomeDaFila, exchange: "trigger", routingKey: "");
        _processaEvento = processaEvento;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        EventingBasicConsumer? consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (ModuleHandle, ea) => {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            _processaEvento.Processa(message);
        };
        _channel.BasicConsume(queue: _nomeDaFila, autoAck: true, consumer: consumer);
        
        return Task.CompletedTask;
    }
}