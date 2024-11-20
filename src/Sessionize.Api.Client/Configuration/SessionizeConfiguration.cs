namespace Sessionize.Api.Client.Configuration;

public class SessionizeConfiguration
{

    public const string SectionName = "Sessionize";

    private string _baseUrl = null!;

    public string BaseUrl
    {
        set => _baseUrl = value;
        get => GetBaseUrl();
    }
    public string? ApiId { get; set; }

    // Make sure the BaseUrl property value ends with /api/v2 if that is not already the case
    // and remove any trailing slashes
    private string GetBaseUrl()
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            return "https://sessionize.com/api/v2/";
        }
        return _baseUrl.EndsWith("api/v2/", StringComparison.OrdinalIgnoreCase) ? _baseUrl : $"{_baseUrl}/api/v2/";
    }

}