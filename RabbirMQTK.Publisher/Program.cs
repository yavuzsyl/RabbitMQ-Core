using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbirMQTK.Publisher
{

    public enum LogLevel
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }

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

                    channel.ExchangeDeclare("DirectExchange", type: ExchangeType.Direct, durable: true);

                    Array logLevel = Enum.GetValues(typeof(LogLevel));


                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();

                        LogLevel log = (LogLevel)logLevel.GetValue(rnd.Next(logLevel.Length));

                        var messageAsByte = Encoding.UTF8.GetBytes($"LOG-{i}-{log}");

                        //keeps safe messages when instance down
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish("DirectExchange", routingKey: log.ToString(), properties, messageAsByte);//message in queue

                        Console.WriteLine($"LOG has been sent -{i}-{log}");
                    }

                }
                Console.WriteLine("Press the key mah man");
                Console.ReadLine();
            }
        }

    }
}
