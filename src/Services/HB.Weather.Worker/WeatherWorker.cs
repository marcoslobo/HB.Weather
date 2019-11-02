using HB.Weather.Worker.Services;
using Infra.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unidevel.OpenWeather;

namespace HB.Weather.Worker
{
    public class WeatherWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration configuration;
        private IModel channel;

        public WeatherWorker(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this._logger = loggerFactory.CreateLogger<WeatherWorker>();            
            this.configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {

            IConnection connection = new ConnectionFactory()
            {
               HostName = configuration.GetSection("Queue:Address").Value, 
               Port = int.Parse(configuration.GetSection("Queue:Port").Value.ToString()),
               UserName = configuration.GetSection("Queue:Login").Value,
               Password = configuration.GetSection("Queue:Password").Value
            }.CreateConnection();

            
            channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            channel.QueueDeclare(queue: configuration.GetSection("Queue:Name").Value,
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var cityNameCountryCode = JsonConvert.DeserializeObject<string>(System.Text.Encoding.UTF8.GetString(ea.Body));
                
                var replyQueueName = channel.QueueDeclare().QueueName;

                IBasicProperties props = channel.CreateBasicProperties();
                props.CorrelationId = ea.BasicProperties.CorrelationId;
                props.ReplyTo = ea.BasicProperties.ReplyTo;
                
               var listforReturn = GetWeathersFromThirdPartiesApi(cityNameCountryCode).Result;
                String jsonified = JsonConvert.SerializeObject(listforReturn);
                byte[] orderBuffer = Encoding.UTF8.GetBytes(jsonified);

                channel.BasicPublish(
                       exchange: "",
                       routingKey: props.ReplyTo,
                       basicProperties: props,
                       body: orderBuffer);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            channel.BasicConsume(configuration.GetSection("Queue:Name").Value, false, consumer);
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<WeatherToApiDto>> GetWeathersFromThirdPartiesApi(string cityNameCountryCode)
        {
            using (IOpenWeatherClient openWeatherClient = new OpenWeatherClient(apiKey: configuration.GetSection("ApiWeatherThirdPartieKey").Value))
            {
                var currentWeather = await openWeatherClient.GetWeatherForecast5d3hAsync(cityNameCountryCode: cityNameCountryCode);

                var listOfWeatherToApiDto = new List<WeatherToApiDto>();

                foreach (var item in currentWeather.List)
                {
                    listOfWeatherToApiDto.Add(new WeatherToApiDto
                    {
                        CityName = currentWeather.City.Name,
                        CountryName = currentWeather.City.Country,
                        MaxTemp = item.Main.TempMaxC,
                        MinTemp = item.Main.TempMinC,
                        Date = item.DateTimeUtc                        
                    });
                    
                }
                return listOfWeatherToApiDto;
            }
            
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            channel.Close();
            base.Dispose();
            
        }
    }
}
