using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wrd2Pdf.Producer.Models;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Text;

namespace Wrd2Pdf.Producer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;
        private readonly ConnectionFactory factory;
        private const string ConverterDirectExchange = "converter-exchange";
        private const string ConverterRoutingKey = "WordToPdf";

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ConnectionFactory factory)
        {
            _logger = logger;
            this.configuration = configuration;
            this.factory = factory;
            factory.Uri = new Uri(configuration.GetConnectionString("RabbitMQCloudString"));

        }

        public IActionResult WordToPdf()
        {
            return View();
        }
        [HttpPost]
        public IActionResult WordToPdf(WordToPdf wordToPdfmodel)
        {

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    //channel
                    using (var channel = connection.CreateModel())
                    {
                        //exchange
                        channel.ExchangeDeclare(
                            exchange: ConverterDirectExchange, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                        //queue eğer ayakta subscriber yoksa mesajların kaybolmaması için kuyruğa atılacak bu kuyruk consumerlar tarafından daha sonra consume edilecek
                        channel.QueueDeclare(
                            queue: "File", durable: true, exclusive: false, autoDelete: false, arguments: null);

                        //queue exchang'e bind edilecek 
                        channel.QueueBind(
                            queue: "File", exchange: ConverterDirectExchange, routingKey: ConverterRoutingKey, arguments: null);

                        FileMessageQueue message = new FileMessageQueue();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            wordToPdfmodel.WordFile.CopyTo(ms);
                            message.WordByte = ms.ToArray();
                        }
                        message.Email = wordToPdfmodel.Email;
                        message.FileName = Path.GetFileNameWithoutExtension(wordToPdfmodel.WordFile.FileName);

                        var serializedMessage = JsonConvert.SerializeObject(message);
                        byte[] byteMessage = Encoding.UTF8.GetBytes(serializedMessage);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;//keeps safe the message bro

                        channel.BasicPublish(
                            exchange: ConverterDirectExchange, routingKey: ConverterRoutingKey, basicProperties: properties, body: byteMessage);

                        ViewBag.result = "After word to pdf convert process we will send you pdf file with an email";
                        ViewBag.state = true;

                    }
                }
           
            }
            catch (Exception ex)
            {
                ViewBag.result = $"Couldnt convert file due to {ex.Message}";
                ViewBag.state = false;
            }
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
