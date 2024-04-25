namespace Sessionize.Api.Client.Configuration;

public class SessionizeConfiguration
{
 
    public const string SectionName = "Sessionize";

    public string BaseUrl { get; set; } = null!;
    public string? ApiId { get; set; }

}