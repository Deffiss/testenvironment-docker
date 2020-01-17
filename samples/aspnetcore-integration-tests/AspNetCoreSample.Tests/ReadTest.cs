using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreSample.Tests
{
    public class ReadTest : IClassFixture<ReadEnvironmentFixture>
    {
        private readonly ReadEnvironmentFixture _environmentFixture;

        public ReadTest(ReadEnvironmentFixture environmentTwoFixture)
        {
            _environmentFixture = environmentTwoFixture;
        }

        [Fact]
        public async Task Test1()
        {
            // Arrange.
            var client = _environmentFixture.TestClient;
            var data = new WeatherForecast { Date = DateTime.Today.ToUniversalTime(), Summary = "Hello", TemperatureC = 10 };

            // Act.
            var getResponse = await client.GetAsync($"/WeatherForecast");
            var dataGot = await getResponse.Content.ReadAsAsync<WeatherForecast[]>();

            // Assert.
            Assert.Equal(3, dataGot.Length);
        }
    }
}
