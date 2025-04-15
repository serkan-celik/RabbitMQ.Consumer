
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//RabbitMQ Cloud Bağlantı Adresi Oluşturma
ConnectionFactory factory = new();
factory.Uri = new("amqp://guest:guest@localhost:5672");

//Bağlantıyı Aktifleştirme
using IConnection connection = factory.CreateConnection();

//Bağlantıda Kanal Açma
using IModel channel = connection.CreateModel();

//Kanalda Queue(Kuyruk) Oluşturma
//Consumer'da da kuyruk, publisher'daki ile birebir aynı yapılandırmada tanımlanmalıdır!
channel.QueueDeclare(queue: "example-queue", exclusive: false);

//Queue(Kuyruk)'ye Mesaj Okuma
EventingBasicConsumer consumer = new(channel);
//autoAck parametresi kuyruktan bir mesaj alındığında o mesajın kuyruktan fiziksel olarak silinip silinmemesine dair bir davranış sergiler. 
//"example-queue" isimli kuyruğa mesaj geldiğinde consume et demektir.
channel.BasicConsume(queue: "example-queue", autoAck: true, consumer);

//channel.BasicQos(0, 10, false); //mesajı işleyen consumerlar bağımsız olarak max 1 Byte'lık 10 adet mesaj işleyebilir.

//channel.BasicQos(0,100, true); //tüm consumer'lar aynı anda 10 adet sınırsız KB boyutunda mesaj işleyebilir.(Adil Dağıtım)

//consumer'a ne zaman ki mesaj geldiğinde mesajı yakalayabilmesi için dinlemesi gerekmektedir.
consumer.Received += (sender, e) =>
    {
        //Kuyruğa gelen mesajın işlendiği yerdir!
        //e.Body: Kuyruktaki mesajın verisini bütünsel olarak getirecektir.
        //e.Body.Span veya e.Body.ToArray() : Kuyruktaki mesajın byte verisini getirecektir.

        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
        //channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
    };

Console.Read();