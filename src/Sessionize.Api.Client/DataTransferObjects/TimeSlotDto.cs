namespace Sessionize.Api.Client.DataTransferObjects;

public record TimeSlotDto(string SlotStart, List<SessionRoomDto> Rooms);
