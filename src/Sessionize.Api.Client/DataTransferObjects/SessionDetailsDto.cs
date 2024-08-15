namespace Sessionize.Api.Client.DataTransferObjects;

public record SessionDetailsDto(string Id,
    string Title,
    string? Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    bool IsServiceSession,
    bool IsPlenumSession,
    int RoomId,
    string Room,
    string Status,
    bool IsInformed,
    bool IsConfirmed,
    List<SessionSpeakerDto> Speakers);
