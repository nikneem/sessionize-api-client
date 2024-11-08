using Sessionize.Api.Client.ValueObjects;

namespace Sessionize.Api.Client.DataTransferObjects;

public record AllDataResponse(
    List<SessionDetails> Sessions,
    List<SpeakerDetails> Speakers,
    List<RoomName> Rooms);
