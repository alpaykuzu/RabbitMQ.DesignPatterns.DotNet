using RabbitMQ.Client;
using System.Text;

//Bu tasarımda, publisher mesajı bir exchange'e gönderir ve böylece mesaj bu exchange'e bind edilmiş olan tüm kuyruklara yönlendirilir.
//Bu tasarım, bir mesajın birçok tüketici tarafından işlenmesi gerektiği durumlarda kullanışlıdır.
//Örneğin, bir haber sitesi yeni bir haber yayınladığında, bu haberi tüm abonelerine göndermek isteyebilir. Bu durumda, haberler için bir exchange oluşturulur ve her abonelik için bir kuyruk oluşturularak bu exchange'e bind edilir. Böylece, yeni bir haber yayınlandığında, bu haber exchange'e gönderilir ve otomatik olarak tüm abonelerin kuyruklarına yönlendirilir.


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

for (int i = 0; i < 100; i++)
{
    Task.Delay(200);
    byte[] messageBody = Encoding.UTF8.GetBytes("Merhaba" + i);
    channel.BasicPublish(
        exchange: exchangeName,
        routingKey: string.Empty, //fanout kullandığımız için boş değer
        body: messageBody
        );
    Console.WriteLine($"Mesaj Gönderildi.");
}

Console.Read();