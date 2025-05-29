using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route(
        template : "[controller]"
    )]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
                                                 {
                                                     "Freezing",
                                                     "Bracing",
                                                     "Chilly",
                                                     "Cool",
                                                     "Mild",
                                                     "Warm",
                                                     "Balmy",
                                                     "Hot",
                                                     "Sweltering",
                                                     "Scorching"
                                                 };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(
            Name = "GetWeatherForecast"
        )]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation(
                message : "Fetching weather forecast"
            );
        return Enumerable.Range(
                    start : 1,
                    count : 5
                ).
            Select(
                    selector : index => new WeatherForecast
                                        {
                                            Date = DateOnly.FromDateTime(
                                                    dateTime : DateTime.Now.AddDays(
                                                            value : index
                                                        )
                                                ),
                                            TemperatureC = RandomNumberGenerator.GetInt32(
                                                    fromInclusive : -20,
                                                    toExclusive : 55
                                                ),
                                            Summary = Summaries[RandomNumberGenerator.GetInt32(
                                                    toExclusive : Summaries.Length
                                                )]
                                        }
                ).
            ToArray();
    }
}
