﻿using System.Net;
using System.Text;
using System.Text.Json;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomersService.Application.Integrations;

public class CommonHttpClient : ICommonHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<CommonHttpClient> _logger;

    public CommonHttpClient(HttpClient client, ILogger<CommonHttpClient> logger)
    {
        _logger = logger;
        _httpClient = client;
        _serializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }

    public async Task<T> SendGetRequestAsync<T>(string path)
    {
        try
        {
            _logger.LogDebug("Sending GET request to {Path}", path);
            var response = await _httpClient.GetAsync(path);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, _serializerOptions);
            _logger.LogDebug("GET request to {Path} succeeded", path);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "GET request to {Path} failed", path);
            if (ex.StatusCode == HttpStatusCode.InternalServerError)
                throw new BadGatewayException("Invalid response from the upstream server.");
            else throw new ServiceUnavailableException("Request to the service failed.");
        }
    }

    public async Task<K> SendPostRequestAsync<T, K>(string path, T postRequestModel)
    {
        try
        {
            _logger.LogDebug("Sending POST request to {Path}", path);
            var jsonModel = JsonSerializer.Serialize(postRequestModel);
            var requestContent = new StringContent(jsonModel, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(path, requestContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<K>(content, _serializerOptions);
            _logger.LogDebug("POST request to {Path} succeeded", path);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "POST request to {Path} failed", path);
            if (ex.StatusCode == HttpStatusCode.InternalServerError)
                throw new BadGatewayException("Invalid response from the upstream server.");
            else throw new ServiceUnavailableException("Request to the service failed.");
        }
    }
}
