using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.DataTransferObjects;
using Sessionize.Api.Client.Exceptions;

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

    public Task<AllDataDto> GetAllDataAsync()
    {
        _logger.LogInformation("Getting all data");
        return SendRequestAsync<AllDataDto>("All");
    }

    public Task<List<ScheduleGridDto>> GetScheduleGridAsync()
    {
        _logger.LogInformation("Getting schedule grid");
        return SendRequestAsync<List<ScheduleGridDto>>("GridSmart");
    }

    public Task<List<SpeakerDetailsDto>> GetSpeakersListAsync()
    {
        _logger.LogInformation("Getting speakers list");
        return SendRequestAsync<List<SpeakerDetailsDto>>("Speakers");
    }

    public Task<List<SessionListDto>> GetSessionsListAsync()
    {
        _logger.LogInformation("Getting sessions list");
        return SendRequestAsync<List<SessionListDto>>("Sessions");
    }

    public Task<List<SpeakerWallDto>> GetSpeakerWallAsync()
    {
        _logger.LogInformation("Getting speaker wall");
        return SendRequestAsync<List<SpeakerWallDto>>("SpeakerWall");
    }

    private async Task<TResult> SendRequestAsync<TResult>(string endpoint)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_sessionizeConfiguration.Value.BaseUrl);
        _logger.LogInformation("Sending GET request to endpoint {Endpoint}", GetViewEndpoint(endpoint));
        var response = await httpClient.SendAsync(GetRequest(endpoint));
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<TResult>(response.Content);
    }

    private HttpRequestMessage GetRequest(string viewName)
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(GetViewEndpoint(viewName), UriKind.Relative)
        };
    }

    private string GetViewEndpoint(string viewName)
    {
        if (string.IsNullOrEmpty(SessionizeApiId))
        {
            if (string.IsNullOrWhiteSpace(_sessionizeConfiguration.Value.ApiId))
            {
                throw new SessionizeApiClientException(ErrorCode.InvalidConfiguration);
            }

            SessionizeApiId = _sessionizeConfiguration.Value.ApiId;
        }

        return $"{SessionizeApiId}/view/{viewName}";
    }

    private async Task<TResponse> DeserializeResponse<TResponse>(HttpContent responseContent) where TResponse : class
    {
        var responseContentString = await responseContent.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContentString))
        {
            throw new SessionizeApiClientException(ErrorCode.SessionizeResponseEmpty);
        }

        var responseObject = JsonSerializer.Deserialize<TResponse>(responseContentString, _jsonDeSerializerOptions.Value);
        if (responseObject == null)
        {
            throw new SessionizeApiClientException(ErrorCode.DeserializationFailed);
        }
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
