using HB.Weather.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HB.Weather.Api.Data
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherContext weatherContext;
        public WeatherRepository(WeatherContext weatherContext)
        {
            this.weatherContext = weatherContext ?? throw new ArgumentNullException(nameof(weatherContext));
        }

        public IEnumerable<WeatherForecast> GetAll()
        {
            return weatherContext
                    .WeatherForecasts
                    .ToList();
        }

        public void Save(WeatherForecast weatherForecast)
        {
            weatherContext.Add(weatherForecast);
            weatherContext.SaveChanges();
        }


      
    }
}
