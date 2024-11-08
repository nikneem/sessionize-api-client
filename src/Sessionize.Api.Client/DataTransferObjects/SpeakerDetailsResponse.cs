using Sessionize.Api.Client.ValueObjects;

namespace Sessionize.Api.Client.DataTransferObjects;

public record SpeakerDetailsResponse(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string FullName, 
    string Bio, 
    string TagLine, 
    string ProfilePicture,
    bool IsTopSpeaker,
    SpeakerSession[] Sessions);