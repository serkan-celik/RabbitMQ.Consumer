﻿
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqp://guest:guest@localhost:5672");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "header-exchange", 
    type: ExchangeType.Headers);

Console.Write("Lütfen header value'sunu giriniz : ");

string value = Console.ReadLine();

string queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(
    queue: queueName,
    exchange: "header-exchange",
    routingKey: string.Empty,
    new Dictionary<string, object>
    {
        ["no"] = value
    });

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