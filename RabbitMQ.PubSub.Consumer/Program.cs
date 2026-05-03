using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


//Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new Uri("url");

//Bağlantıyı Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

string exchangeName = "pub-sub-exchange";
channel.ExchangeDeclare(
    exchange: exchangeName,
    type: ExchangeType.Fanout,
    durable: false,
    autoDelete: false
    );

string queueName = channel.QueueDeclare().QueueName; //Rastgele bir kuyruk ismi oluşturur

channel.QueueBind(
    queue: queueName,
    exchange: exchangeName,
    routingKey: string.Empty //fanout kullandığımız için boş değer
    );

channel.BasicQos(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false
    );//Bu ayar, bir tüketicinin aynı anda yalnızca bir mesaj işlemesine izin verir. Bu, mesajların sırayla işlenmesini sağlar ve tüketicinin aşırı yüklenmesini önler.

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: queueName,
    autoAck: false,
    consumer: consumer
    );

consumer.Received += (sender, e) => 
{
    string message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine($"Gelen Mesaj: {message}");
    channel.BasicAck(e.DeliveryTag, false);
};


Console.Read();