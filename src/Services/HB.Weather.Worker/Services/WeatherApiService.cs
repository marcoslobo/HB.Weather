using HB.Weather.Worker.Services;
using Infra.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HB.Weather.Worker
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient httpClient;

        public WeatherApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task Send(WeatherToApiDto weatherToApiDto)
        {
            var jsonPost = new StringContent(JsonConvert.SerializeObject(weatherToApiDto), UnicodeEncoding.UTF8, "application/json");
            
            await httpClient.PostAsync("api/v1/weather/", jsonPost).ConfigureAwait(false);            
                
        }
    }
}
