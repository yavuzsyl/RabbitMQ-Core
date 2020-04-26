using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQTK.Publisher;
using System;
using System.Collections.Generic;
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

                    channel.ExchangeDeclare("HeaderExchange", type: ExchangeType.Headers, durable: true);

                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format1", "pdf");
                    headers.Add("shape", "A4");

                    //keeps safe messages when instance down
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    //giden mesajın headerına key-value değerleri eklendi
                    properties.Headers = headers;

                    User user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = "noice@mail.com",
                        Name = "Noice man",
                        Password = "Noiceman123"
                    };

                    var userStr = JsonConvert.SerializeObject(user);

                    channel.BasicPublish("HeaderExchange", routingKey: string.Empty, properties, Encoding.UTF8.GetBytes(userStr));//message in queue

                    Console.WriteLine($"Header message sent");

                }
                Console.WriteLine("Press the key mah man");
                Console.ReadLine();
            }
        }

    }
}
