using System;
using System.Net.Mime;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Net.Http.Headers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spire.Doc;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;

namespace Wrd2Pdf.Consumer
{
    public class Program
    {
        private const string MailFrom = "gicirbeymail@gmail.com";
        private const string pass = "192837eyrt";
        private const string ConverterRoutingKey = "WordToPdf";
        private const string ConverterQueue = "File";
        private const string ConverterDirectExchange = "converter-exchange";
        private const string rabbitMqConnectionString = "amqp://jpvhygyn:y-3JUFmkusd-FmLbcKmPCyhapFV2Mx88@orangutan.rmq.cloudamqp.com/jpvhygyn";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(rabbitMqConnectionString);
            bool result = false;

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("DirectExchange", type: ExchangeType.Direct, durable: true);
                    //publisher ile queue oluşturduğumuz için burada queue declare etmeye gerek yok
                    channel.QueueBind(queue: ConverterQueue, exchange: ConverterDirectExchange, routingKey: ConverterRoutingKey, null);

                    //message dispatch
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume(queue: ConverterQueue, autoAck: false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            Console.WriteLine("Message received from queue processing...");
                            var messageJsonStr = Encoding.UTF8.GetString(ea.Body.ToArray());
                            var message = JsonConvert.DeserializeObject<FileMessageQueue>(messageJsonStr);

                            #region Converting process
                            Document document = new Document();
                            document.LoadFromStream(new MemoryStream(message.WordByte), FileFormat.Docx2013);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                document.SaveToStream(ms, FileFormat.PDF);
                                result = SendEmail(message.Email, ms, message.FileName);
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        if (result)
                        {
                            Console.WriteLine("Message processed succesfully");
                            //mesaj işlendi mesajı sil broker kardeşe mesaj
                            channel.BasicAck(ea.DeliveryTag, multiple: false);
                        }

                    };
                    Console.WriteLine("Nicely done press to end");
                    Console.ReadLine();
                }
            }
        }

        public static bool SendEmail(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                //memory başlangıç pozisyonu
                memoryStream.Position = 0;
                //içerik tipi pdf olarak seçildi
                ContentType contentType = new ContentType(MediaTypeNames.Application.Pdf);

                //attachment a streamden dosya okunacak
                Attachment attachment = new Attachment(contentStream: memoryStream, contentType: contentType);
                attachment.ContentDisposition.FileName = $"{fileName}.pdf";

                SmtpClient smtpClient = new SmtpClient()
                {
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(MailFrom, pass)
                };
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(MailFrom),
                    Subject = "PDF File convert",
                    Body = "Look attachment",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);
                mailMessage.Attachments.Add(attachment);

                smtpClient.Send(mailMessage);
                memoryStream.Close();
                Console.WriteLine("Mail Sent");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
