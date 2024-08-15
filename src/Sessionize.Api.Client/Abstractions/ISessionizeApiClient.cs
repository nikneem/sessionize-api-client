using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataDto> GetAllDataAsync(CancellationToken? cancellationToken = null);
    Task<List<ScheduleGridDto>> GetScheduleGridAsync(CancellationToken? cancellationToken = null);
    Task<List<SpeakerDetailsDto>> GetSpeakersListAsync(CancellationToken? cancellationToken = null);
    Task<List<SessionListDto>> GetSessionsListAsync(CancellationToken? cancellationToken = null);
    Task<List<SpeakerWallDto>> GetSpeakerWallAsync(CancellationToken? cancellationToken = null);
}