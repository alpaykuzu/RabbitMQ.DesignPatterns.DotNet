# RabbitMQ Design Patterns with .NET

Bu repository, RabbitMQ kullanarak temel mesajlaşma tasarımlarının .NET (C#) ile nasıl uygulanacağını gösteren örnek kodları içerir. Eğitim sürecimde öğrendiğim temel mesaj değişim modellerini (Message Exchange Patterns) kapsamaktadır.



## 🛠 Kullanılan Teknolojiler
*   **.NET 9**
*   **RabbitMQ.Client** (NuGet Paketi)
*   **C#**

## Uygulanan Senaryolar

### 1. Point-to-Point (P2P)
Mesajın doğrudan bir kuyruğa gönderildiği ve tek bir consumer tarafından işlendiği en temel modeldir. Bir işin yalnızca bir kez ve bir kişi tarafından yapılması gereken durumlar için idealdir.

### 2. Publish/Subscribe (Fanout)
Publisher mesajı bir `Exchange`'e gönderir ve bu exchange'e bağlı olan **tüm kuyruklara** mesaj iletilir. Bir olayın tüm abonelere duyurulması gereken senaryolarda (örn: haber sitesi aboneliği) kullanılır.

### 3. Request/Response (RPC)
Asenkron bir sistemde senkron bir işlem gibi yanıt beklenen modeldir. Publisher bir mesaj gönderir (`ReplyTo` adresiyle) ve Consumer'dan gelen yanıtı `CorrelationId` üzerinden takip ederek işlemin sonucunu öğrenir.

### 4. Work Queues (İş Kuyrukları)
Yoğun iş yüklerini birden fazla Consumer arasında paylaştırmak için kullanılır. `BasicQos` (Prefetch Count) ayarı ile mesajların consumerlar arasında adil (Fair Dispatch) şekilde dağıtılması sağlanır.

## Nasıl Çalıştırılır?

1.  Bilgisayarınızda RabbitMQ'nun kurulu olduğundan veya bir Cloud sunucu (örn: CloudAMQP) erişiminiz olduğundan emin olun.
2.  Projedeki `factory.Uri = new Uri("url");` satırlarındaki `"url"` kısmına kendi bağlantı adresinizi yazın.
3.  Önce ilgili senaryonun **Consumer** uygulamasını, ardından **Publisher** uygulamasını çalıştırın.
