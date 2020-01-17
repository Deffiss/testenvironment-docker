using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TestEnvironment.Docker.Containers.Mongo;

namespace AspNetCoreSample.Tests
{
    public class ReadEnvironmentFixture : EnvironmentFixture
    {
        public ReadEnvironmentFixture() : base("env2")
        {
        }

        protected override async Task OnInitialized(MongoContainer mongoContainer)
        {
            await base.OnInitialized(mongoContainer);

            var client = new MongoClient(mongoContainer.GetConnectionString());
            await client.GetDatabase("Forecast").GetCollection<WeatherForecast>("WeatherForecast").InsertManyAsync(new[]
            {
                new WeatherForecast { Date = DateTime.Today.ToUniversalTime(), Summary = "Good weather", TemperatureC = 7 },
                new WeatherForecast { Date = DateTime.Today.AddDays(-1).ToUniversalTime(), Summary = "Usual weather", TemperatureC = 3 },
                new WeatherForecast { Date = DateTime.Today.AddDays(-2).ToUniversalTime(), Summary = "Bad weather", TemperatureC = 0 }
            });
        }
    }
}
