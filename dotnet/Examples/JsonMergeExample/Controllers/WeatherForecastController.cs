using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Toolbox.JsonMerge;

namespace JsonMergeExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPatch(Name = "MergePatch"), 
         Consumes(typeof(JsonMergePatchDocument<WeatherForecastPatch>), "application/merge-patch+json")]
        public IActionResult MergePatch(JsonMergePatchDocument<WeatherForecastPatch> patch)
        {
            var source = Get().FirstOrDefault();

            var result = patch.CreatePatchedValue(source, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return Ok(result);
        }

        //[HttpPatch(Name = "Patch"),
        // Consumes(typeof(JsonPatchDocument<WeatherForecast>), "application/json-patch+json")]
        //public IActionResult Patch(JsonPatchDocument<WeatherForecast> patch)
        //{
        //    return Ok("json patch");
        //}
    }
}