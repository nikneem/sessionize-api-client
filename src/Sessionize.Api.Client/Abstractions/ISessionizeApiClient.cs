using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataDto> GetAllDataAsync();
    Task<List<ScheduleGridDto>> GetScheduleGridAsync();
    Task<List<SpeakerDetailsDto>> GetSpeakersListAsync();
    Task<List<SessionListDto>> GetSessionsListAsync();
    Task<List<SpeakerWallDto>> GetSpeakerWallAsync();
}