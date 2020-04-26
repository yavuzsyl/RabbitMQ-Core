using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
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

                    channel.ExchangeDeclare("HeaderExchange", type: ExchangeType.Headers, durable: true);

                    channel.QueueDeclare("queue1", false, false, false, null);

                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format", "pdf");
                    headers.Add("shape", "A4");
                    headers.Add("x-match", "any");//header değerleri bire bir uymalı

                    channel.QueueBind("queue1", "HeaderExchange", string.Empty, arguments: headers);

                    var consumer = new EventingBasicConsumer(channel);
                    //gelen mesajı consume etme işlemi
                    channel.BasicConsume("queue1", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body);
                        Console.WriteLine($"Received message {message}");
                        channel.BasicAck(ea.DeliveryTag, multiple: false);


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
