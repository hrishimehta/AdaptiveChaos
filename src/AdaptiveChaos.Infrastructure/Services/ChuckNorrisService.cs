using AdaptiveChaos.Shared.Domain;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

namespace AdaptiveChaos.Infrastructure.Services
{
    public class ChuckNorrisService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChuckNorrisService> _logger;
        public ChuckNorrisService(IHttpClientFactory httpClientFactory, ILogger<ChuckNorrisService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ChuckNorrisJoke> GetRandomJoke()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                _logger.LogWarning($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - Call started");
                var httpClient = _httpClientFactory.CreateClient("ChuckNorrisService");
                var jokeResponse = await httpClient.GetFromJsonAsync<ChuckNorrisJoke>("/jokes/random");
                _logger.LogWarning($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - Call completed");
                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds > 4500)
                {
                    // test code to see latency
                    _logger.LogWarning($"long response");
                }

                return jokeResponse;
            }
            catch (HttpRequestException ex)
            {
                // Handle specific HTTP request exceptions
                throw new Exception("Error fetching Chuck Norris joke.Error=" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception("An error occurred while processing the request", ex);
            }
        }
    }
}
