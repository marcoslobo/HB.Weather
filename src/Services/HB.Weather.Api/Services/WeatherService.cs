using HB.Weather.Api.Data;
using HB.Weather.Api.Models;
using Infra.Dto;
using System;
using System.Collections.Generic;

namespace HB.Weather.Api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository weatherRepository;
        public WeatherService(IWeatherRepository weatherRepository)
        {
            this.weatherRepository = weatherRepository ?? throw new ArgumentNullException(nameof(weatherRepository));
        }
        public void Save(WeatherToApiDto weatherToApiDto)
        {
            var entity = MapToDomain(weatherToApiDto);

            weatherRepository.Save(entity);
        }

        private WeatherForecast MapToDomain(WeatherToApiDto weatherToApiDto)
        {
            return new WeatherForecast()
            {
                CityName = weatherToApiDto.CityName,
                Country = weatherToApiDto.CountryName,
                Date = weatherToApiDto.Date,
                MaxTemp = weatherToApiDto.MaxTemp,
                MinTemp = weatherToApiDto.MinTemp
            };

        }

        public IEnumerable<WeatherForecast> GetAll()
        {
            return weatherRepository.GetAll();
        }

        public void Save(IEnumerable<WeatherToApiDto> listOfReturn)
        {
            foreach (var item in listOfReturn)
            {
                weatherRepository.Save(MapToDomain(item));
            }
        }
    }
}
