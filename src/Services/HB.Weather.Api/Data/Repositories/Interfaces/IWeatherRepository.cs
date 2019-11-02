using HB.Weather.Api.Models;
using System.Collections.Generic;

namespace HB.Weather.Api.Data
{
    public interface IWeatherRepository
    {
        void Save(WeatherForecast weatherForecast);
        IEnumerable<WeatherForecast> GetAll();
    }
}
