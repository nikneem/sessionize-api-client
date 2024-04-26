namespace Sessionize.Api.Client.DataTransferObjects;

public record AllDataSpeakerDto(
    string Id,
    string FirstName,
    string LastName,
    string Bio,
    string TagLine,
    string ProfilePicture,
    bool IsTopSpeaker,
    object[] Links,
    int[] Sessions,
    string FullName,
    object[] CategoryItems,
    object[] QuestionAnswers
);