using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<List<SpeakerDetailsDto>> GetSpeakersListAsync();
    Task<SessionListDto> GetSessionsListAsync();
}