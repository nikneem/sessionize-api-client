using Sessionize.Api.Client.ValueObjects;

namespace Sessionize.Api.Client.DataTransferObjects;

public record ScheduleGridResponse(DateTimeOffset Date, bool IsDefault, List<RoomSessions> Rooms, List<TimeSlot> TimeSlots);
