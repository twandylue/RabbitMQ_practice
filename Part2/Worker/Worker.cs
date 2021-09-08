using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {   
            // one worker
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
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                channel.BasicQos(
                    prefetchSize: 0,
                    prefetchCount: 1,
                    global: false
                );
                Console.WriteLine(" [*] Waiting for messages.");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    // Console.WriteLine(" [x] Received {0}", message);
                    Person ms = JsonSerializer.Deserialize<Person>(message);
                    Console.WriteLine($"Name: {ms.name}");
                    Console.WriteLine($"Age: {ms.age}");
                    Console.WriteLine(" [x] Done");
                    channel.BasicAck(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false
                    );
                    Thread.Sleep(500);
                };
                channel.BasicConsume(
                    queue: "task_queue",
                    autoAck: false,
                    consumer: consumer
                );
                Console.WriteLine(" Press [enter] to exit");
                Console.ReadLine();
            }
        }
    }

    class Person
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}
