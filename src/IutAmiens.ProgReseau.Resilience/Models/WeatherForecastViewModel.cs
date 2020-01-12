using IutAmiens.ProgReseau.Shared;
using System.Collections.Generic;

namespace IutAmiens.ProgReseau.Resilience.Models
{
    public class WeatherForecastViewModel
    {
        public bool HasError { get; set; }

        public IEnumerable<WeatherForecast> Forecast { get; set; }
    }
}