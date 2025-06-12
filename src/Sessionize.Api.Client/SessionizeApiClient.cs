using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.DataTransferObjects;
using Sessionize.Api.Client.Exceptions;
using Sessionize.Api.Client.Filters;
using Sessionize.Api.Client.ValueObjects;

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

    public async Task<AllDataResponse> GetAllDataAsync(SessionFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting all data with filtering");
        var response = await SendRequestAsync<AllDataResponse>("All", sessionizeApiId, cancellationToken);
        
        if (filter == null)
            return response;

        return FilterAllDataResponse(response, filter);
    }

    public Task<List<ScheduleGridResponse>> GetScheduleGridAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting schedule grid");
        return SendRequestAsync<List<ScheduleGridResponse>>("GridSmart", sessionizeApiId, cancellationToken);
    }

    public async Task<List<ScheduleGridResponse>> GetScheduleGridAsync(SessionFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting schedule grid with filtering");
        var response = await SendRequestAsync<List<ScheduleGridResponse>>("GridSmart", sessionizeApiId, cancellationToken);
        
        if (filter == null)
            return response;

        return FilterScheduleGridResponse(response, filter);
    }

    public Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting speakers list");
        return SendRequestAsync<List<SpeakerDetailsResponse>>("Speakers", sessionizeApiId, cancellationToken);
    }

    public async Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(SpeakerFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting speakers list with filtering");
        var response = await SendRequestAsync<List<SpeakerDetailsResponse>>("Speakers", sessionizeApiId, cancellationToken);
        
        if (filter == null)
            return response;

        return FilterSpeakersListResponse(response, filter);
    }

    public Task<List<SessionListResponse>> GetSessionsListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting sessions list");
        return SendRequestAsync<List<SessionListResponse>>("Sessions", sessionizeApiId, cancellationToken);
    }

    public async Task<List<SessionListResponse>> GetSessionsListAsync(SessionFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting sessions list with filtering");
        var response = await SendRequestAsync<List<SessionListResponse>>("Sessions", sessionizeApiId, cancellationToken);
        
        if (filter == null)
            return response;

        return FilterSessionsListResponse(response, filter);
    }

    public Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting speaker wall");
        return SendRequestAsync<List<SpeakerWallResponse>>("SpeakerWall", sessionizeApiId, cancellationToken);
    }

    public async Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(SpeakerFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null)
    {
        _logger.LogInformation("Getting speaker wall with filtering");
        var response = await SendRequestAsync<List<SpeakerWallResponse>>("SpeakerWall", sessionizeApiId, cancellationToken);
        
        if (filter == null)
            return response;

        return FilterSpeakerWallResponse(response, filter);
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

    private AllDataResponse FilterAllDataResponse(AllDataResponse response, SessionFilter filter)
    {
        var filteredSessions = FilterSessions(response.Sessions, filter);
        var sessionIds = filteredSessions.Select(s => s.Id).ToHashSet();
        
        // Filter speakers: remove speakers who have no sessions in the filtered set
        var filteredSpeakers = response.Speakers.Where(speaker => 
            speaker.Sessions.Any(sessionId => sessionIds.Contains(sessionId.ToString()))).ToList();
        
        return response with 
        { 
            Sessions = filteredSessions.ToList(),
            Speakers = filteredSpeakers
        };
    }

    private List<ScheduleGridResponse> FilterScheduleGridResponse(List<ScheduleGridResponse> response, SessionFilter filter)
    {
        return response.Select(schedule =>
        {
            var filteredRooms = schedule.Rooms.Select(room =>
            {
                var filteredSessions = FilterSessionInfos(room.Sessions, filter);
                return room with { Sessions = filteredSessions.ToList() };
            }).Where(room => room.Sessions.Any()).ToList();

            return schedule with { Rooms = filteredRooms };
        }).Where(schedule => schedule.Rooms.Any()).ToList();
    }

    private List<SessionListResponse> FilterSessionsListResponse(List<SessionListResponse> response, SessionFilter filter)
    {
        return response.Select(group =>
        {
            var filteredSessions = FilterSessionInfos(group.Sessions, filter);
            return group with { Sessions = filteredSessions.ToList() };
        }).Where(group => group.Sessions.Any()).ToList();
    }

    private List<SpeakerDetailsResponse> FilterSpeakersListResponse(List<SpeakerDetailsResponse> response, SpeakerFilter filter)
    {
        return response.Where(speaker => MatchesSpeakerFilter(speaker, filter)).ToList();
    }

    private List<SpeakerWallResponse> FilterSpeakerWallResponse(List<SpeakerWallResponse> response, SpeakerFilter filter)
    {
        return response.Where(speaker => MatchesSpeakerWallFilter(speaker, filter)).ToList();
    }

    private IEnumerable<SessionDetails> FilterSessions(List<SessionDetails> sessions, SessionFilter filter)
    {
        return sessions.Where(session => MatchesSessionFilter(session, filter));
    }

    private IEnumerable<SessionInfo> FilterSessionInfos(List<SessionInfo> sessions, SessionFilter filter)
    {
        return sessions.Where(session => MatchesSessionInfoFilter(session, filter));
    }

    private bool MatchesSessionFilter(SessionDetails session, SessionFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Title) && 
            !session.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(filter.Description) && 
            !session.Description.Contains(filter.Description, StringComparison.OrdinalIgnoreCase))
            return false;

        if (filter.StartDate.HasValue && session.StartsAt < filter.StartDate.Value)
            return false;

        if (filter.EndDate.HasValue && session.EndsAt > filter.EndDate.Value)
            return false;

        return true;
    }

    private bool MatchesSessionInfoFilter(SessionInfo session, SessionFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Title) && 
            !session.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(filter.Description) && 
            (session.Description == null || !session.Description.Contains(filter.Description, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (filter.StartDate.HasValue && session.StartsAt < filter.StartDate.Value)
            return false;

        if (filter.EndDate.HasValue && session.EndsAt > filter.EndDate.Value)
            return false;

        return true;
    }

    private bool MatchesSpeakerFilter(SpeakerDetailsResponse speaker, SpeakerFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.FirstName) && 
            !speaker.FirstName.Contains(filter.FirstName, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(filter.LastName) && 
            !speaker.LastName.Contains(filter.LastName, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    private bool MatchesSpeakerWallFilter(SpeakerWallResponse speaker, SpeakerFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.FirstName) && 
            !speaker.FirstName.Contains(filter.FirstName, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(filter.LastName) && 
            !speaker.LastName.Contains(filter.LastName, StringComparison.OrdinalIgnoreCase))
            return false;

        if (filter.IsTopSpeaker.HasValue && speaker.IsTopSpeaker != filter.IsTopSpeaker.Value)
            return false;

        return true;
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
