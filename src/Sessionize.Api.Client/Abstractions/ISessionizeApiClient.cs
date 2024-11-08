using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataResponse> GetAllDataAsync(CancellationToken? cancellationToken = null);
    Task<List<ScheduleGridResponse>> GetScheduleGridAsync(CancellationToken? cancellationToken = null);
    Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(CancellationToken? cancellationToken = null);
    Task<List<SessionListResponse>> GetSessionsListAsync(CancellationToken? cancellationToken = null);
    Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(CancellationToken? cancellationToken = null);
}