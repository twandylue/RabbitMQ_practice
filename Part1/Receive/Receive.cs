using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receive
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
                    queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (ch, m) =>
                {
                    var body = m.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("[x] Received {0}", message);
                };

                // for (int i = 0; i < 10; i++) {
                //     channel.BasicConsume(
                //     queue: "hello",
                //     autoAck: true,
                //     consumer: consumer
                //     );
                // }

                channel.BasicConsume(
                    queue: "hello",
                    autoAck: true,
                    consumer: consumer
                    );

                Console.WriteLine("Received DONE!");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
