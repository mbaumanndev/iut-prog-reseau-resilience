using IutAmiens.ProgReseau.Shared;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IutAmiens.ProgReseau.Resilience.Services
{
    public interface IWeatherService
    {
        [Get("/WeatherForecast")]
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
    }
}