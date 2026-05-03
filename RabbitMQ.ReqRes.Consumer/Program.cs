using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new Uri("url");

//Bağlantıyı Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

string requestQueueName = "request-queue";
channel.QueueDeclare(
    queue: requestQueueName,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: requestQueueName,
    autoAck: true,
    consumer: consumer
    );

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine($"Gelen Mesaj: {message}");

    //Yanıt Gönderme
    byte[] responseMessageBody = Encoding.UTF8.GetBytes($"Response for {message}");

    IBasicProperties responseProperties = channel.CreateBasicProperties();
    responseProperties.CorrelationId = e.BasicProperties.CorrelationId; //Yanıt mesajına, gelen request mesajının correlation ID'si eklenir.
    
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: e.BasicProperties.ReplyTo, //Yanıt mesajı, gelen request mesajının ReplyTo özelliğinde belirtilen kuyruk adına gönderilir.
        basicProperties: responseProperties,
        body: responseMessageBody
        );
};

Console.Read();