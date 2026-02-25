using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Sessionize.Api.Client.Configuration;

public class SessionizeConfigurationValidation : IValidateOptions<SessionizeConfiguration>
{
    private static readonly Regex ApiIdRegex = new("^[a-zA-Z0-9]{8,12}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public ValidateOptionsResult Validate(string? name, SessionizeConfiguration options)
    {
        var errorList = new List<string>();

        // Validate BaseUrl format and HTTPS
        if (!string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri))
            {
                errorList.Add($"The app setting {SessionizeConfiguration.SectionName}.{nameof(options.BaseUrl)} must be a valid absolute URI");
            }
            else if (uri.Scheme != Uri.UriSchemeHttps)
            {
                errorList.Add($"The app setting {SessionizeConfiguration.SectionName}.{nameof(options.BaseUrl)} must use HTTPS for secure communication");
            }
        }

        if (!string.IsNullOrWhiteSpace(options.ApiId) && !ApiIdRegex.IsMatch(options.ApiId))
        {
            errorList.Add($"The app setting {SessionizeConfiguration.SectionName}.{nameof(options.ApiId)} must be an alphanumeric string between 8 and 12 characters");
        }

        return errorList.Count > 0 ? ValidateOptionsResult.Fail(errorList) : ValidateOptionsResult.Success;
    }
}