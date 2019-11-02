using System.Threading.Tasks;
using HB.Weather.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HB.Weather.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(ILogger<WeatherController> logger)
        {
            _logger = logger;
        }
        [HttpGet("{countryCode}/{cityName}")]
        public async Task<IActionResult> Get([FromServices] IQueueService queueService, string countryCode, string cityName)
        {
            await queueService.PublishMessage($"{cityName},{countryCode}");                
            
            return Ok();
        }
        [HttpGet]
        public IActionResult  GetAll([FromServices] IWeatherService weatherService)
        {
            return Ok(weatherService.GetAll());
        }
    }
}
