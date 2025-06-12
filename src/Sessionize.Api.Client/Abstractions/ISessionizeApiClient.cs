using Sessionize.Api.Client.DataTransferObjects;
using Sessionize.Api.Client.Filters;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataResponse> GetAllDataAsync(string? sessionizeApiId = null,CancellationToken? cancellationToken = null);
    Task<AllDataResponse> GetAllDataAsync(SessionFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<ScheduleGridResponse>> GetScheduleGridAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<ScheduleGridResponse>> GetScheduleGridAsync(SessionFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(SpeakerFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SessionListResponse>> GetSessionsListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SessionListResponse>> GetSessionsListAsync(SessionFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(SpeakerFilter? filter, string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
}