using System;
using RabbitMQ.Client;
using System.Text;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                UserName = "root",
                Password = "admin1234",
                VirtualHost = "/",
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                Console.WriteLine("請輸入訊息：");
                string message = Console.ReadLine();
                // string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                for (int i = 0; i < 100; i++)
                {
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "hello",
                        basicProperties: null,
                        body: body
                    );
                    Console.WriteLine("[x] Sent {0}", message);
                }
            }
            
            Console.WriteLine("Sending Done!");
            Console.WriteLine(" Press [enter] to exit");
            Console.ReadLine();
        }
    }
}
