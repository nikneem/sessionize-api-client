namespace Sessionize.Api.Client.DataTransferObjects;

public record AllDataDto(
    List<AllDataSessionDto> Sessions,
    List<AllDataSpeakerDto> Speakers,
    List<AllDataRoomDto> Rooms);
