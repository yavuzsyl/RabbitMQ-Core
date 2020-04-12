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
            //factory.HostName = "localhost";

            //connection
            using (var connection = factory.CreateConnection())
            {
                //channel
                using (var channel = connection.CreateModel())
                {
                    #region comment
                    //queue-durable false in memory true in disk?-exclusive true only one channel false any channel-autodelete to delete queue after all shit s done
                    //with durable true messages wont be lost when instance restarted 
                    #endregion
                    channel.QueueDeclare("rabbitSpeeding", durable: true, false, false, null);//1.message durability


                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var messageAsByte = Encoding.UTF8.GetBytes($"{i}-{message}");
                        
                        //keeps safe messages when instance down
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //2nd overload => if we dont provide exchange default shit gets in business first parameter, routingkey aka queue?
                        channel.BasicPublish(string.Empty, routingKey: "rabbitSpeeding", properties, messageAsByte);//message in queue

                        Console.WriteLine($"Message has been sent : {i}-{message}");
                    }

                }
            }
            Console.WriteLine("Press the key mah man");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args[0];
        }
    }
}
