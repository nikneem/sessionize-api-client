namespace Sessionize.Api.Client.DataTransferObjects;

public record RoomListItemDto(int Id, string Name, List<SessionDetailsDto> Sessions, bool HasOnlyPlenumSessions);
