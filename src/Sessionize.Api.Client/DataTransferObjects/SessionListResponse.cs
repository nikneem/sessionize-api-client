using Sessionize.Api.Client.ValueObjects;

namespace Sessionize.Api.Client.DataTransferObjects;

public record SessionListResponse(bool IsDefault, string GroupId, string GroupName, List<SessionInfo> Sessions);