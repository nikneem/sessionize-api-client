using Microsoft.Extensions.Options;
using Sessionize.Api.Client.Configuration;

namespace Sessionize.Api.Client.IntegrationTests.ConfigurationTests;

public class AppConfigurationTests
{
    [Fact]
    public void WhenConfigurationMeetsMinimalRequirement_ConfigurationValidationSucceeds()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com"
        };
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Fact]
    public void WhenConfigurationIsInvalid_ConfigurationFallsBackToDefault()
    {
        // Arrange
        var config = new SessionizeConfiguration();
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

}