using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbirMQTK.Publisher
{
    //Critical.Error.Info.Warning
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

                    channel.ExchangeDeclare("TopicExchange", type: ExchangeType.Topic, durable: true);

                    Array logLevel = Enum.GetValues(typeof(LogLevel));


                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();

                        LogLevel log1 = (LogLevel)logLevel.GetValue(rnd.Next(logLevel.Length));
                        LogLevel log2 = (LogLevel)logLevel.GetValue(rnd.Next(logLevel.Length));
                        LogLevel log3 = (LogLevel)logLevel.GetValue(rnd.Next(logLevel.Length));

                        var messageAsByte = Encoding.UTF8.GetBytes($"LOG-{i}-{log1}-{log2}-{log3}");

                        //keeps safe messages when instance down
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        var logsRoutingKey = $"{log1}.{log2}.{log3}";

                        channel.BasicPublish("TopicExchange", routingKey: logsRoutingKey, properties, messageAsByte);//message in queue

                        Console.WriteLine($"LOG has been sent -{i}-{log1}-{log2}-{log3}");
                    }

                }
                Console.WriteLine("Press the key mah man");
                Console.ReadLine();
            }
        }

    }
}
