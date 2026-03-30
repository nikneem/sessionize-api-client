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

    private readonly Lazy<JsonSerializerOptions> _jsonDeSerializerOptions = new(() =>
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

    public string? SessionizeApiId { get; set; }

    public Task<AllDataResponse> GetAllDataAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting all data");
        return SendRequestAsync<AllDataResponse>("All", sessionizeApiId, cancellationToken);
    }

    public Task<List<ScheduleGridResponse>> GetScheduleGridAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting schedule grid");
        return SendRequestAsync<List<ScheduleGridResponse>>("GridSmart", sessionizeApiId, cancellationToken);
    }

    public Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting speakers list");
        return SendRequestAsync<List<SpeakerDetailsResponse>>("Speakers", sessionizeApiId, cancellationToken);
    }

    public Task<List<SessionListResponse>> GetSessionsListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting sessions list");
        return SendRequestAsync<List<SessionListResponse>>("Sessions", sessionizeApiId, cancellationToken);
    }

    public Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting speaker wall");
        return SendRequestAsync<List<SpeakerWallResponse>>("SpeakerWall", sessionizeApiId, cancellationToken);
    }

    public async Task<DataChangedResponse> HasDataChangedAsync(string viewName, string? lastKnownHash = null, string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        var ct = cancellationToken ?? CancellationToken.None;
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_sessionizeConfiguration.Value.BaseUrl);

        var endpoint = GetViewEndpoint(viewName, sessionizeApiId) + "?hashonly=true";
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        _logger.LogInformation("Fetching data hash from endpoint {Endpoint}", endpoint);

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var serverHash = (await response.Content.ReadAsStringAsync(ct)).Trim();
        if (string.IsNullOrWhiteSpace(serverHash))
        {
            _logger.LogWarning("Empty hash response from {Endpoint}, assuming data has changed", endpoint);
            return new DataChangedResponse(true, string.Empty);
        }

        if (lastKnownHash is null)
        {
            _logger.LogInformation("No previous hash provided, returning current server hash");
            return new DataChangedResponse(true, serverHash);
        }

        var hasChanged = !string.Equals(serverHash, lastKnownHash, StringComparison.OrdinalIgnoreCase);
        _logger.LogInformation("Data changed check for {Endpoint}: server={ServerHash}, local={LocalHash}, hasChanged={HasChanged}",
            endpoint, serverHash, lastKnownHash, hasChanged);

        return new DataChangedResponse(hasChanged, serverHash);
    }

    private async Task<TResult> SendRequestAsync<TResult>(string endpoint, string? sessionizeApiId = null, CancellationToken? cancellationToken = null) where TResult : class
    {
        var ct = cancellationToken ?? CancellationToken.None;
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_sessionizeConfiguration.Value.BaseUrl);
        var httpRequest = GetRequest(endpoint, sessionizeApiId);
        _logger.LogInformation("Sending GET request to endpoint {Endpoint}", httpRequest.RequestUri);
        var response = await httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<TResult>(response.Content);
    }

    private HttpRequestMessage GetRequest(string viewName, string? apiId = null)
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(GetViewEndpoint(viewName, apiId), UriKind.Relative)
        };
    }

    private string GetViewEndpoint(string viewName, string? apiId = null)
    {
        var currentApiId = apiId ?? SessionizeApiId;
        if (string.IsNullOrEmpty(currentApiId))
        {
            if (string.IsNullOrWhiteSpace(_sessionizeConfiguration.Value.ApiId))
            {
                throw new SessionizeApiClientException(ErrorCode.InvalidConfiguration);
            }

            currentApiId = _sessionizeConfiguration.Value.ApiId;
        }

        return $"{currentApiId}/view/{viewName}";
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
