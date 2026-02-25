using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Sessionize.Api.Client.Configuration;

public class SessionizeConfigurationValidation : IValidateOptions<SessionizeConfiguration>
{
    private static readonly Regex ApiIdRegex = new("^[a-zA-Z0-9]{8,12}$", RegexOptions.Compiled);

    public ValidateOptionsResult Validate(string? name, SessionizeConfiguration options)
    {
        var errorList = new List<string>();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errorList.Add($"The app setting {SessionizeConfiguration.SectionName}.{nameof(options.BaseUrl)} cannot be null or empty");
        }

        if (!string.IsNullOrWhiteSpace(options.ApiId) && !ApiIdRegex.IsMatch(options.ApiId))
        {
            errorList.Add($"The app setting {SessionizeConfiguration.SectionName}.{nameof(options.ApiId)} must be an alphanumeric string between 8 and 12 characters");
        }

        return errorList.Count > 0 ? ValidateOptionsResult.Fail(errorList) : ValidateOptionsResult.Success;
    }
}