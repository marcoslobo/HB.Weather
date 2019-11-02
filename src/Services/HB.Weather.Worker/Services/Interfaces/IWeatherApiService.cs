using Infra.Dto;
using System.Threading.Tasks;

namespace HB.Weather.Worker.Services
{
    public interface IWeatherApiService
    {
        Task Send(WeatherToApiDto weatherToApiDto);
    }
}
