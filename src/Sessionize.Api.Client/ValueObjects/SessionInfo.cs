namespace Sessionize.Api.Client.ValueObjects;

public record SessionInfo(string Id,
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
    List<SessionSpeaker> Speakers);
