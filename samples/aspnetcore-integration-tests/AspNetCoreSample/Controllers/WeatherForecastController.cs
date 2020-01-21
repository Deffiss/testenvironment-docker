using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AspNetCoreSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IConfiguration configuration, ILogger<WeatherForecastController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var client = new MongoClient(_configuration.GetValue<string>("MongoDbConnectionString"));
            return client.GetDatabase("Forecast").GetCollection<WeatherForecast>("WeatherForecast").AsQueryable().ToArray();
        }

        [HttpGet("{id}")]
        public WeatherForecast GetById(string id)
        {
            var client = new MongoClient(_configuration.GetValue<string>("MongoDbConnectionString"));
            return client.GetDatabase("Forecast").GetCollection<WeatherForecast>("WeatherForecast")
                .AsQueryable().Where(w => w.Id == id).FirstOrDefault();
        }

        [HttpPost]
        public IActionResult Post(WeatherForecast weatherForecast)
        {
            var client = new MongoClient(_configuration.GetValue<string>("MongoDbConnectionString"));
            client.GetDatabase("Forecast").GetCollection<WeatherForecast>("WeatherForecast").InsertOne(weatherForecast);

            return Ok(weatherForecast);
        }
    }
}
