using System;
using System.ComponentModel.DataAnnotations;

namespace HB.Weather.Api.Models
{
    public class WeatherForecast
    {
        [Key]
        public long Id { get; set; }
        public string CityName { get; set; }
        public string Country { get; set; }
        public float MaxTemp { get; set; }
        public float MinTemp { get; set; }                
        public DateTime Date { get; set; }
    }
}
