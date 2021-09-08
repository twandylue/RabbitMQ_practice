using System;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
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
                    queue: "task_queue",
                    // durable: true,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);
                // var properties = channel.CreateBasicProperties(); // 保存訊息
                // properties.Persistent = true; //

                for (int i = 0; i < 20; i++)
                {
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "task_queue",
                        // basicProperties: properties, 
                        basicProperties: null,
                        body: body
                    );
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }
        private static string GetMessage(string[] args)
        {
            var target = new Person
            {
                name = "Andy",
                age = 27
            };
            string jsonString = JsonSerializer.Serialize<Person>(target);
            return ((args.Length > 0) ? string.Join(" ", args) : jsonString);
        }
    }

    class Person
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}
