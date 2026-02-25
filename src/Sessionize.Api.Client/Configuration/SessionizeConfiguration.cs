namespace Sessionize.Api.Client.Configuration;

public class SessionizeConfiguration
{

    public const string SectionName = "Sessionize";

    private string _baseUrl = null!;

    public string BaseUrl
    {
        set
        {
            ValidateBaseUrl(value);
            _baseUrl = value;
        }
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

        var url = _baseUrl.TrimEnd('/');
        return url.EndsWith("/api/v2", StringComparison.OrdinalIgnoreCase) ? $"{url}/" : $"{url}/api/v2/";
    }

    private static void ValidateBaseUrl(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return; // Allow null/empty, will use default
        }

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("BaseUrl must be a valid absolute URI", nameof(baseUrl));
        }

        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("BaseUrl must use HTTPS for secure communication", nameof(baseUrl));
        }
    }

}