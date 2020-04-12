using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbirMQTK.Consumer
{
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

                    channel.QueueDeclare("rabbitSpeeding", durable: true, false, false, null); //1.message durability
                    #region comment
                    //prefetchCount consumer her seferde mesajları tek tek alacak 1 mesaj işlemi bitmeden diğerini almayacak
                    //global consumer instance sayısına göre false olursa her instance  tek seferde prefetchCount kadar mesaj alır ,
                    //global true olursa tüm instancelar toplam olarak tek seferde prefetchCount kadar mesaj alır
                    //wont consume any messages before the current ones(prefetchCount) done successfully
                    //if global true all conusmer instances takes prefetchCount as 1 instance, if false every consumer instance takes prefetchCount by one's own 
                    #endregion
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);//fair dipatch

                    Console.WriteLine("waiting for messages");

                    //event-in ctor we specify listen to which channel
                    var consumer = new EventingBasicConsumer(channel);

                    #region comment
                    //autoAck specifies that recieved message will be deleted or not after receiving it
                    //when false after consuming message succesfully, consumer will decide to delete or keep the message 
                    //mesajı aldıktan sonra brokera mesaj başarıyla işlendikten sonra silineceği bilgisini biz vereceğizzz
                    #endregion
                    channel.BasicConsume("rabbitSpeeding", autoAck: false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var messagByte = ea.Body;
                        var message = Encoding.UTF8.GetString(messagByte);
                        Console.WriteLine($"Message has been received : \"{message}\" ");

                        //simulating message process
                        int simulatedProcessTimeForEveryMessage = int.Parse(GetMessage(args));
                        Thread.Sleep(simulatedProcessTimeForEveryMessage);
                        Console.WriteLine("Message has been processed");

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
