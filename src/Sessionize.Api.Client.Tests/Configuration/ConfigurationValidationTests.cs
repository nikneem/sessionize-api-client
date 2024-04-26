using Sessionize.Api.Client.Configuration;

namespace Sessionize.Api.Client.Tests.Configuration;

public class ConfigurationValidationTests
{


    [Fact]
    public void Validate_WhenBaseUrlIsNullOrEmpty_ReturnsError()
    {
        // Arrange
        var configuration = new SessionizeConfiguration
        {
            BaseUrl = string.Empty
        };
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, configuration);

        // Assert
        Assert.False(result.Succeeded);
    }
}