
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqp://guest:guest@localhost:5672");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "topic-exchange", 
    type: ExchangeType.Topic);

Console.Write("Dinlenecek topic formatını belirtiniz : ");

string topic = Console.ReadLine();

string queueName = channel.QueueDeclare().QueueName; //Random kuyruk adı, manuel de verilebilir.

channel.QueueBind(
    queue: queueName,
    exchange: "topic-exchange",
    routingKey: topic);

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