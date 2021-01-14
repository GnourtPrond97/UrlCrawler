using HtmlAgilityPack;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UrlCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            //var doc = new HtmlWeb().Load("https://vnexpress.net/suc-khoe/dinh-duong");
            var doc = new HtmlWeb().Load("https://vnexpress.net/suc-khoe/dinh-duong");
            //var linkTags = doc.DocumentNode.Descendants("link");
            var linkedPages = doc.DocumentNode.Descendants("a")
                                              .Select(a => a.GetAttributeValue("href", null))
                                              .Where(u => !String.IsNullOrEmpty(u));
            HashSet<string> result = new HashSet<string>();
            foreach (string item in linkedPages)
            {   
                if (item.Contains(".html") && !item.Contains("#box_comment_vne"))
                {
                    result.Add(item);

                }
            }

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                string message = "Chui thecc";

                int count = 0;
                foreach (string item in result)
                {
                    

                    var body = Encoding.UTF8.GetBytes(item);
                    channel.BasicPublish(exchange: "",
                                    routingKey: "hello",
                                    basicProperties: null,
                                    body: body);
                    count++;
                }



                Console.WriteLine(" [x] Sent " + count);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    
    
    }
}
