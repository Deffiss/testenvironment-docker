using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreSample.Tests
{
    public class WriteTests : IClassFixture<WriteEnvironmentFixture>
    {
        private readonly WriteEnvironmentFixture _environmentFixture;

        public WriteTests(WriteEnvironmentFixture environmentOneFixture)
        {
            _environmentFixture = environmentOneFixture;
        }

        [Fact]
        public async Task Test1()
        {
            // Arrange.
            var client = _environmentFixture.TestClient;
            var data = new WeatherForecast { Date = DateTime.Today.ToUniversalTime(), Summary = "Hello", TemperatureC = 10 };

            // Act.
            var postResponse = await client.PostAsJsonAsync("/WeatherForecast", data);
            var dataPosted = await postResponse.Content.ReadAsAsync<WeatherForecast>();

            var getResponse = await client.GetAsync($"/WeatherForecast/{dataPosted.Id}");
            var dataGot = await getResponse.Content.ReadAsAsync<WeatherForecast>();

            // Assert.
            Assert.Equal(dataPosted.Id, dataGot.Id);
            Assert.Equal(dataPosted.Summary, dataGot.Summary);
            Assert.Equal(dataPosted.Date, dataGot.Date);
            Assert.Equal(dataPosted.TemperatureC, dataGot.TemperatureC);
        }
    }
}
