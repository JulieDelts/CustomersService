
using System.Net;
using System.Text;
using System.Text.Json;
using CustomersService.Application.Exceptions;

namespace CustomersService.Application.Integrations
{
    internal class CommonHttpClient
    {
        private readonly HttpClient _httpClient = new();
        private JsonSerializerOptions _serializerOptions;

        public CommonHttpClient(string baseUrl, HttpMessageHandler? handler = null)
        {
            if (handler != null)
            {
                _httpClient = new HttpClient(handler);
            }

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = new TimeSpan(0, 5, 0);
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
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.InternalServerError)
                    throw new BadGatewayException("Invalid response from the upstream server.");
                else throw new ServiceUnavailableException("Request to the service failed.");
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
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.InternalServerError)
                    throw new BadGatewayException("Invalid response from the upstream server.");
                else throw new ServiceUnavailableException("Request to the service failed.");
            }
        }
    }
}
