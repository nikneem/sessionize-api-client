using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client;

public class SessionizeApiClient : ISessionizeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SessionizeApiClient> _logger;
    private readonly IOptions<SessionizeConfiguration> _sessionizeConfiguration;

    private readonly Lazy<JsonSerializerOptions> _jsonDeSerializerOptions = new Lazy<JsonSerializerOptions>(() =>
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

    public string? SessionizeApiId { get; set; }

    public async Task<List<SpeakerDetailsDto>> GetSpeakersListAsync()
    {
        _logger.LogInformation("Getting speakers list");
        var httpClient = _httpClientFactory.CreateClient(SessionizeConstants.HttpClientName);
        _logger.LogInformation("Getting speakers list from {Endpoint}", GetViewEndpoint("Speakers"));
        var response = await httpClient.GetAsync(GetViewEndpoint("Speakers"));
        response.EnsureSuccessStatusCode();
        var responseList =  await DeserializeResponse<List<SpeakerDetailsDto>>(response.Content);
        return responseList ?? new List<SpeakerDetailsDto>();
    }

    public async Task<SessionListDto> GetSessionsListAsync()
    {
        _logger.LogInformation("Getting sessions list");
        var httpClient = _httpClientFactory.CreateClient(SessionizeConstants.HttpClientName);
        _logger.LogInformation("Getting sessions list from {Endpoint}", GetViewEndpoint("Sessions"));
        var response = await httpClient.GetAsync(GetViewEndpoint("Sessions"));
        response.EnsureSuccessStatusCode();
        var responseList =  await DeserializeResponse<SessionListDto>(response.Content);
        return responseList ?? new(false, string.Empty, string.Empty, new List<SessionDetailsDto>());
    }

    private string GetViewEndpoint(string viewName)
    {
        if (string.IsNullOrEmpty(SessionizeApiId))
        {
            if (string.IsNullOrWhiteSpace(_sessionizeConfiguration.Value.ApiId))
            {
                throw new InvalidOperationException("SessionizeConfiguration is not set");
            }
            SessionizeApiId = _sessionizeConfiguration.Value.ApiId;
        }

        return $"/{SessionizeApiId}/view/{viewName}";
    }

    private async Task<TResponse?> DeserializeResponse<TResponse>(HttpContent responseContent)
    {
        var responseContentString = await responseContent.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContentString))
        {
            throw new InvalidOperationException("Response content is empty");
        }
        var responseObject = JsonSerializer.Deserialize<TResponse>(responseContentString, _jsonDeSerializerOptions.Value);

        return responseObject;
    }

    public SessionizeApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<SessionizeApiClient> logger,
        IOptions<SessionizeConfiguration> sessionizeConfiguration,
        string sessionizeApiId)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _sessionizeConfiguration = sessionizeConfiguration;
        SessionizeApiId = sessionizeApiId;
    }

    public SessionizeApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<SessionizeApiClient> logger,
        IOptions<SessionizeConfiguration> sessionizeConfiguration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _sessionizeConfiguration = sessionizeConfiguration;
    }

}