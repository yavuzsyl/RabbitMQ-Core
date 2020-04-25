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
                    channel.ExchangeDeclare("logs", durable:true, type: ExchangeType.Fanout);

                    string message = GetMessage(args);//mesajlar konsoldan gönderiliyor

                    for (int i = 1; i < 11; i++)
                    {
                        var messageAsByte = Encoding.UTF8.GetBytes($"{i}-{message}");
                        
                        //keeps safe messages when instance down
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //2nd overload => if we dont provide exchange default shit gets in business first parameter, routingkey aka queue?
                        //fanout exchange => we dont provide routingKey cuz we want to send messages to all subscribers
                        channel.BasicPublish("logs", "", properties, messageAsByte);//message in queue

                        Console.WriteLine($"Message has been sent : {i}-{message}");
                    }

                }
            }
            //Console.WriteLine("Press the key mah man");
            //Console.ReadLine();
        }
        //send arguements through console ex => dotnet run "message"
        private static string GetMessage(string[] args)
        {
            return args[0];
        }
    }
}
