using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbirMQTK.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://jpvhygyn:y-3JUFmkusd-FmLbcKmPCyhapFV2Mx88@orangutan.rmq.cloudamqp.com/jpvhygyn");

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("rabbitWalking", false, false, false, null);

                    //event-in ctor we specify listen to which channel
                    var consumer = new EventingBasicConsumer(channel);

                    //autoAck specifies that recieved message will be deleted or not after receiving it
                    channel.BasicConsume("rabbitWalking",true, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var messagByte = ea.Body;
                        var message = Encoding.UTF8.GetString(messagByte);
                        Console.WriteLine($"Message has been received : \"{message}\" ");
                    };
                }
            }
            Console.WriteLine("Press the key mah man");
            Console.ReadLine();
        }
    }
}
