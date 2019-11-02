

using HB.Weather.Api.Models;
using Infra.Dto;
using System.Collections.Generic;

namespace HB.Weather.Api.Services
{
    public interface IWeatherService
    {
        void Save(WeatherToApiDto weatherToApiDto);
        IEnumerable<WeatherForecast> GetAll();
        void Save(IEnumerable<WeatherToApiDto> listOfReturn);
    }
}
