namespace Sessionize.Api.Client.Filters;

public class SpeakerFilter
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? IsTopSpeaker { get; set; }
}