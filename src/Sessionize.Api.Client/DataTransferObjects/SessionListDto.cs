namespace Sessionize.Api.Client.DataTransferObjects;

public record SessionListDto(bool IsDefault, string GroupId, string GroupName, List<SessionDetailsDto> Sessions);