namespace Sessionize.Api.Client.DataTransferObjects;

public record SpeakerDetailsDto(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string FullName, 
    string Bio, 
    string TagLine, 
    string ProfilePicture,
    bool IsTopSpeaker,
    SpeakerSessionDto[] Sessions);