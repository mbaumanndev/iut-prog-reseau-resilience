using IutAmiens.ProgReseau.Resilience.Models;
using IutAmiens.ProgReseau.Resilience.Services;
using IutAmiens.ProgReseau.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace IutAmiens.ProgReseau.Resilience.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWeatherService m_WeatherService;
        private readonly ILogger m_Logger;

        public HomeController(IWeatherService p_WeatherService, ILogger<HomeController> p_Logger)
        {
            m_WeatherService = p_WeatherService;
            m_Logger = p_Logger;
        }

        [Route("/")]
        public async Task<IActionResult> IndexAsync(
            [FromServices] IHttpClientFactory p_Factory
        )
        {
            WeatherForecastViewModel v_Model = new WeatherForecastViewModel();

            try
            {
                string v_Endpoint = "WeatherForecast";
                var v_Client = p_Factory.CreateClient("weather");

                var v_Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri(v_Client.BaseAddress + v_Endpoint)
                );
                m_Logger.LogInformation("URL: " + new Uri(v_Client.BaseAddress + v_Endpoint));
                v_Request.SetPolicyExecutionContext(
                    new Context("GetWeather")
                );
                var v_Response = await v_Client.SendAsync(v_Request);

                if (!v_Response.IsSuccessStatusCode)
                    throw new ApplicationException("Echec requête");
                
                v_Model.Forecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(
                    await v_Response.Content.ReadAsStringAsync()
                );
                //v_Model.Forecast = await m_WeatherService.GetWeatherForecastAsync();
            }
            catch (Exception v_Ex)
            {
                m_Logger.LogError(v_Ex, "Failed to get weather forecast");

                v_Model.HasError = true;
            }

            return View(v_Model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}