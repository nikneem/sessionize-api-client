namespace Sessionize.Api.Client.DataTransferObjects;

public record SessionRoomDto(int Id, string Name, SessionDetailsDto Session, int Index);
