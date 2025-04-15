
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqp://guest:guest@localhost:5672");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "fanout-exchange", type: ExchangeType.Fanout);

Console.WriteLine("Kuyruk adını giriniz : ");

string queueName = Console.ReadLine();

channel.QueueDeclare(
    queue: queueName,
    exclusive: false);

channel.QueueBind(
    queue: queueName,
    exchange: "fanout-exchange",
    routingKey: string.Empty);

EventingBasicConsumer consumer = new(channel);

channel.BasicConsume(
    queue: queueName, 
    autoAck: true, 
    consumer);

consumer.Received += (sender, e) =>
    {
        string message = Encoding.UTF8.GetString(e.Body.Span);
        Console.WriteLine(message);
    };

Console.Read();