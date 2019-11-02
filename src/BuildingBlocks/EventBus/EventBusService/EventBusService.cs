using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HB.Weather.BuildingBlocks.EventBus
{
    public class RabbitBusService : IEventPublisher
    {
        private readonly IConfiguration configuration;
        

        public RabbitBusService(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public Task PublishMessage<T>(T msg)
        {
            using (IConnection connection = new ConnectionFactory() { 
                HostName = configuration.GetSection("Queue:Address").Value, 
                Port = int.Parse(configuration.GetSection("Queue:Port").Value.ToString()),
                UserName = configuration.GetSection("Queue:Login").Value,
                Password = configuration.GetSection("Queue:Password").Value

            }.CreateConnection())


            {
                using (IModel channel = connection.CreateModel())
                {
                    var queueName = configuration.GetSection("Queue:Name").Value;

                    channel.QueueDeclare(queue: queueName,
                                          durable: false, // Could be TRUE
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

                    
                    var correlationId = Guid.NewGuid().ToString();

                    IBasicProperties props = channel.CreateBasicProperties();
                    props.CorrelationId = correlationId;

                    String jsonified = JsonConvert.SerializeObject(msg);
                    byte[] orderBuffer = Encoding.UTF8.GetBytes(jsonified);
                    
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: props,
                        body: orderBuffer);

                    return Task.CompletedTask;
                      
                }
            }

        }
    }
}
