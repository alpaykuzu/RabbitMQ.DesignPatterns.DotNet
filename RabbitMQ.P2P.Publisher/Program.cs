using RabbitMQ.Client;
using System.Text;

//Bu tasarımda, bir publisher ilgili mesajı direkt bir kuyruğa gönderir ve bu mesaj kuyruğu işleyen bir consumer tarafından tüketilir.
//Eğer ki senaryo gereği bir mesajın bir tüketici tarafından işlenmesi gerekiyorsa bu yaklaşım kullanılır.


//Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new Uri("url");

//Bağlantıyı Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

string queueName = "p2p-queue";

channel.QueueDeclare(
    queue: queueName,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

byte[] messageBody = Encoding.UTF8.GetBytes("Merhaba");
channel.BasicPublish(
    exchange: string.Empty,
    routingKey: queueName,
    body: messageBody
    );

Console.Read();