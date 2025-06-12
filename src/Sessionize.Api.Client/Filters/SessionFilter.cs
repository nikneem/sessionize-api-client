namespace Sessionize.Api.Client.Filters;

public class SessionFilter
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}