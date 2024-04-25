using Microsoft.Extensions.Options;

namespace Sessionize.Api.Client.Configuration;

public class SessionizeConfigurationValidation : IValidateOptions<SessionizeConfiguration>
{
    public ValidateOptionsResult Validate(string? name, SessionizeConfiguration options)
    {
        var errorList = new List<string>();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errorList.Add($"The app setting {SessionizeConfiguration.SectionName}.BaseUrl cannot be null or empty");
        }
        return errorList.Count > 0 ? ValidateOptionsResult.Fail(errorList) : ValidateOptionsResult.Success;
    }
}