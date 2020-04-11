using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbirMQTK.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();//to connect to rmq instance
            factory.Uri = new Uri("amqp://jpvhygyn:y-3JUFmkusd-FmLbcKmPCyhapFV2Mx88@orangutan.rmq.cloudamqp.com/jpvhygyn");//amqp url aka connection string

            //connection
            using (var connection = factory.CreateConnection())
            {
                //chanel
                using (var channel = connection.CreateModel())
                {
                    //queue-durable false in memory true in disk?-exclusive true only one channel false any channel-autodelete to delete queue after all shit s done
                    channel.QueueDeclare("rabbitWalking", false, false, false, null);

                    string message = "Rabbit has started walking tellem";
                    var messageAsByte = Encoding.UTF8.GetBytes(message);

                    //2nd overload => if we dont provide exchange default shit gets in business first parameter, routingkey aka queue?
                    channel.BasicPublish(string.Empty, routingKey: "rabbitWalking", null, messageAsByte);//message in queue

                    Console.WriteLine("Message has been sent");

                }
            }
            Console.WriteLine("Press the key mah man");
            Console.ReadLine();
        }
    }
}
