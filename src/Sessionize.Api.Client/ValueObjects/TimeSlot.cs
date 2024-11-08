namespace Sessionize.Api.Client.ValueObjects;

public record TimeSlot(string SlotStart, List<RoomSession> Rooms);
