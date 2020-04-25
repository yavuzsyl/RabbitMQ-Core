using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace RabbirMQTK.Consumer
{
    //publisher tarafından gönderilen critical ve error routing_key dinlenecek
    public enum LogLevel
    {
        Critical = 1,
        Error = 2
    }

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://jpvhygyn:y-3JUFmkusd-FmLbcKmPCyhapFV2Mx88@orangutan.rmq.cloudamqp.com/jpvhygyn");
            //factory.HostName = "localhost";


            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare("DirectExchange", type: ExchangeType.Direct, durable: true);

                    var queueName = channel.QueueDeclare().QueueName;

                    foreach (var item in Enum.GetNames(typeof(LogLevel)))
                    {
                        //tek bir queue exchange e belirtilen routing keyler için bind edilecek
                        channel.QueueBind(queue: queueName, "DirectExchange", routingKey: item);
                    }
       
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine("waiting for logs");

                    var consumer = new EventingBasicConsumer(channel);

                    #region comment
                    //autoAck specifies that recieved message will be deleted or not after receiving it
                    //when false after consuming message succesfully, consumer will decide to delete or keep the message 
                    //mesajı aldıktan sonra brokera mesaj başarıyla işlendikten sonra silineceği bilgisini biz vereceğizzz
                    #endregion
                    channel.BasicConsume(queueName, autoAck: false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var log = ea.Body;
                        var logb = Encoding.UTF8.GetString(log);
                        Console.WriteLine($"log has been received : \"{logb}\" ");

                        //simulating message process
                        int simulatedProcessTimeForEveryMessage = int.Parse(GetMessage(args));
                        Thread.Sleep(simulatedProcessTimeForEveryMessage);
                        Console.WriteLine("logs has been processed");

                        File.AppendAllText("logs_critical_error.txt", $"{logb}\n");
                       
                        //provding information to broker that it can delete the message now
                        channel.BasicAck(ea.DeliveryTag, multiple: false);//2.message acknowledgment


                    };
                    Console.WriteLine("Press the key mah man");
                    Console.ReadLine();
                }
            }
        }
        private static string GetMessage(string[] args)
        {
            return args[0];
        }
    }
}
