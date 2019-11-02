using HB.Weather.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HB.Weather.Api.Data
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public WeatherContext(DbContextOptions<WeatherContext> options) :
           base(options)
        {
        }        
    }
}
