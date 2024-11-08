namespace Sessionize.Api.Client.ValueObjects;

public record RoomSessions(int Id, string Name, List<SessionInfo> Sessions, bool HasOnlyPlenumSessions);
