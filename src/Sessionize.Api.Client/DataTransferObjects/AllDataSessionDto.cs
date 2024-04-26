namespace Sessionize.Api.Client.DataTransferObjects;

public record AllDataSessionDto(
    string Id,
    string Title,
    string Description,
    DateTime StartsAt,
    DateTime EndsAt,
    bool IsServiceSession,
    bool IsPlenumSession,
    List<string> Speakers,
    List<object> CategoryItems,
    List<object> QuestionAnswers,
    int RoomId,
    object LiveUrl,
    object RecordingUrl,
    string Status,
    bool IsInformed,
    bool IsConfirmed);
