using HB.Weather.Api.Data;
using HB.Weather.Api.Models;
using Infra.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HB.Weather.Api.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration configuration;
        private readonly IWeatherService weatherService;
        
        private string replyQueueName;
        private EventingBasicConsumer consumer;
        public QueueService(IConfiguration configuration, IWeatherService weatherService)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));            
        }
        public async Task PublishMessage(string cityAndCountry)
        {
            using (IConnection connection = new ConnectionFactory()
            {
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
                                          durable: false, 
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);
                    
                    consumer = new EventingBasicConsumer(channel);
                    replyQueueName = channel.QueueDeclare().QueueName;
                    var tcs = new TaskCompletionSource<string>();
                    var resultTask = tcs.Task;

                    var correlationId = Guid.NewGuid().ToString();

                    IBasicProperties props = channel.CreateBasicProperties();
                    props.CorrelationId = correlationId;
                    props.ReplyTo = replyQueueName;
                    
                    EventHandler<BasicDeliverEventArgs> handler = null;
                    handler = (model, ea) =>
                    {
                        if (ea.BasicProperties.CorrelationId == correlationId)
                        {
                            consumer.Received -= handler;

                            var body = ea.Body;
                            var response = Encoding.UTF8.GetString(body);

                            var listOfReturn = JsonConvert.DeserializeObject<IEnumerable<WeatherToApiDto>>(response);
                            
                            weatherService.Save(listOfReturn);
                            
                            tcs.SetResult("ok");
                        }
                    };
                    consumer.Received += handler;

                    String jsonified = JsonConvert.SerializeObject(cityAndCountry);
                    byte[] orderBuffer = Encoding.UTF8.GetBytes(jsonified);

                    channel.BasicPublish(
                        exchange: "",                         
                        routingKey: queueName,
                        basicProperties: props,
                        body: orderBuffer);

                    channel.BasicConsume(
                        consumer: consumer,
                        queue: replyQueueName);

                    await resultTask.ConfigureAwait(false);                    

                }
            }

        }

   
    }
}
