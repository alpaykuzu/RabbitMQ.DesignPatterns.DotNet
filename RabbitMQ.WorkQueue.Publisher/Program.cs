using RabbitMQ.Client;
using System.Text;

//Bu tasarımda, publisher tarafından yayımlanmış bir mesajın birden fazla consumer arasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır.
//Böylece mesajların işlenmesi sürecinde tüm consumer'lar aynı iş yüküne ve eşit görev dağılımına sahip olacaktır.
//Örneğin, birden fazla consumer'ın aynı kuyruğa abone olduğu bir senaryoda, her mesaj yalnızca bir consumer tarafından alınır ve işlenir. Bu, mesajların sırayla ve adil bir şekilde dağıtılmasını sağlar.


//Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new Uri("url");

//Bağlantıyı Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

string queueName = "work-queue";
channel.QueueDeclare(
    queue: queueName,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

IBasicProperties properties = channel.CreateBasicProperties();
properties.Persistent = true; //Mesajların kalıcı olması için

for (int i = 0; i < 100; i++)
{
    Task.Delay(200);
    byte[] messageBody = Encoding.UTF8.GetBytes("Merhaba" + i);
    channel.BasicPublish(
        exchange: string.Empty, //Default (Direct) exchange kullanıyoruz, bu yüzden boş değer
        routingKey: queueName, //Mesajın gideceği kuyruk adı 
        body: messageBody,
        basicProperties: properties
        );
    Console.WriteLine($"Mesaj Gönderildi.");
}

Console.Read();