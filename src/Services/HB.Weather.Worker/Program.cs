using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HB.Weather.Worker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HB.Weather.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            CreateHostBuilder(args, config).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(configuration);

                    services.AddHttpClient<IWeatherApiService, WeatherApiService>(c =>
                    {
                        c.BaseAddress = new Uri(configuration.GetSection("UrlApiWeather").Value);
                        c.DefaultRequestHeaders.Add("Accept", "application/json");
                    });

                    services.AddHostedService<WeatherWorker>();
                    
                });
    }
}
