using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataResponse> GetAllDataAsync(string? sessionizeApiId = null,CancellationToken? cancellationToken = null);
    Task<List<ScheduleGridResponse>> GetScheduleGridAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SessionListResponse>> GetSessionsListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
}