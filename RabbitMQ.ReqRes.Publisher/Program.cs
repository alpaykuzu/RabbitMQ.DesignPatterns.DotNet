using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Bu tasarımda, publisher bir request yapar gibi kuyruğa mesaj gönderir ve bu mesajı tüketen consumer'dan sonuca dair başka kuyruktan bir yanıt/response bekler.
//Örneğin, bir sipariş oluşturma işlemi yaparken, sipariş bilgilerini içeren bir mesaj gönderilir ve bu mesajı işleyen consumer, siparişin başarılı bir şekilde oluşturulduğuna dair bir yanıt mesajı gönderir. Publisher, bu yanıt mesajını alarak işlemin sonucunu öğrenir ve buna göre hareket eder. Bu tasarım, özellikle mikroservis mimarilerinde yaygın olarak kullanılır ve sistemler arasında asenkron iletişim sağlar.

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

string responseQueueName = "response-queue";
channel.QueueDeclare(
    queue: responseQueueName,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

string correlationId = Guid.NewGuid().ToString(); //Her bir request için benzersiz bir correlation ID oluşturulur. Bu ID, yanıt mesajının hangi request'e ait olduğunu belirlemek için kullanılır.

//Request Mesajı Gönderme
IBasicProperties properties = channel.CreateBasicProperties();
properties.ReplyTo = responseQueueName; //Consumer'ın yanıt göndereceği kuyruk
properties.CorrelationId = correlationId; //Request ile yanıt arasında ilişki kurmak için correlation ID eklenir.
for (int i = 0; i < 100; i++)
{
    Task.Delay(200);
    byte[] messageBody = Encoding.UTF8.GetBytes($"Request {i}");
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: requestQueueName,
        basicProperties: properties,
        body: messageBody
        );
}

//Yanıtları Dinleme
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: responseQueueName,
    autoAck: true,
    consumer: consumer
    );

consumer.Received += (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
    {
        string responseMessage = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"Response: {responseMessage}");
    }
};

Console.Read();