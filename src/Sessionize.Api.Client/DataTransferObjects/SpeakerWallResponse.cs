
namespace Sessionize.Api.Client.DataTransferObjects;

public record SpeakerWallResponse(Guid Id, string FirstName, string LastName, string FullName, string TagLine, string ProfilePicture, bool IsTopSpeaker);
