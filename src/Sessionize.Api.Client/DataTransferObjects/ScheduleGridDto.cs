namespace Sessionize.Api.Client.DataTransferObjects;

public record ScheduleGridDto(DateTimeOffset Date, bool IsDefault, List<RoomListItemDto> Rooms, List<TimeSlotDto> TimeSlots);
