
using System.Text;
using System.Text.Json;
using CustomersService.Application.Exceptions;

namespace CustomersService.Application.Integrations
{
    internal class CommonHttpClient
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _serializerOptions;

        public CommonHttpClient(string baseUrl)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = new TimeSpan(0, 5, 0)
            };

            _serializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        public async Task<T> SendGetRequestAsync<T>(string path)
        {
            try
            {
                var response = await _httpClient.GetAsync(path);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(content, _serializerOptions);
                return result;
            }
            catch (HttpRequestException)
            {
                throw new ServiceUnavailableException("Request to the service failed.");
            }
        }

        public async Task<K> SendPostRequestAsync<T, K>(string path, T postRequestModel)
        {
            try
            {
                var jsonModel = JsonSerializer.Serialize(postRequestModel);
                var requestContent = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(path, requestContent);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<K>(content, _serializerOptions);
                return result;
            }
            catch (HttpRequestException)
            {
                throw new ServiceUnavailableException("Request to the service failed.");
            }
        }
    }
}
