using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataDto> GetAllDataAsync(CancellationToken cancellationToken);
    Task<List<ScheduleGridDto>> GetScheduleGridAsync(CancellationToken cancellationToken);
    Task<List<SpeakerDetailsDto>> GetSpeakersListAsync(CancellationToken cancellationToken);
    Task<List<SessionListDto>> GetSessionsListAsync(CancellationToken cancellationToken);
    Task<List<SpeakerWallDto>> GetSpeakerWallAsync(CancellationToken cancellationToken);
}