namespace Sessionize.Api.Client.ValueObjects;

public record RoomSession(int Id, string Name, SessionInfo Session, int Index);
