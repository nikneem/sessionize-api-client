using Microsoft.Extensions.Options;
using Sessionize.Api.Client.Configuration;

namespace Sessionize.Api.Client.Tests;

public class BaseUrlValidationTests
{
    [Theory]
    [InlineData("https://sessionize.com")]
    [InlineData("https://sessionize.com/")]
    [InlineData("https://api.sessionize.com")]
    [InlineData("https://example.com")]
    [InlineData("https://localhost:5001")]
    public void ConfigurationValidation_WithValidHttpsUrl_Succeeds(string validUrl)
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = validUrl
        };
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Theory]
    [InlineData("http://sessionize.com")]           // HTTP instead of HTTPS
    [InlineData("http://example.com")]              // HTTP instead of HTTPS
    [InlineData("http://localhost:5000")]           // HTTP instead of HTTPS
    public void ConfigurationValidation_WithHttpUrl_Fails(string httpUrl)
    {
        // Arrange
        var config = new SessionizeConfiguration();
        var validation = new SessionizeConfigurationValidation();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.BaseUrl = httpUrl);
        Assert.Contains("HTTPS", exception.Message);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("sessionize.com")]
    public void ConfigurationValidation_WithInvalidUrl_Fails(string invalidUrl)
    {
        // Arrange
        var config = new SessionizeConfiguration();
        var validation = new SessionizeConfigurationValidation();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.BaseUrl = invalidUrl);
        Assert.Contains("URI", exception.Message);
    }

    [Theory]
    [InlineData("ftp://sessionize.com")]       // FTP protocol
    [InlineData("//sessionize.com")]           // Protocol-relative URL
    [InlineData("javascript:alert(1)")]        // JavaScript protocol
    public void ConfigurationValidation_WithNonHttpsScheme_Fails(string url)
    {
        // Arrange
        var config = new SessionizeConfiguration();
        var validation = new SessionizeConfigurationValidation();

        // Act & Assert - These URLs parse as valid URIs but have wrong scheme
        var exception = Assert.Throws<ArgumentException>(() => config.BaseUrl = url);
        Assert.Contains("HTTPS", exception.Message);
    }

    [Fact]
    public void Configuration_WithNullBaseUrl_UsesDefault()
    {
        // Arrange
        var config = new SessionizeConfiguration();

        // Act
        var baseUrl = config.BaseUrl;

        // Assert
        Assert.Equal("https://sessionize.com/api/v2/", baseUrl);
    }

    [Fact]
    public void Configuration_WithEmptyBaseUrl_UsesDefault()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = ""
        };

        // Act
        var baseUrl = config.BaseUrl;

        // Assert
        Assert.Equal("https://sessionize.com/api/v2/", baseUrl);
    }

    [Fact]
    public void Configuration_WithWhitespaceBaseUrl_UsesDefault()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "   "
        };

        // Act
        var baseUrl = config.BaseUrl;

        // Assert
        Assert.Equal("https://sessionize.com/api/v2/", baseUrl);
    }

    [Fact]
    public void ConfigurationValidation_WithNullBaseUrl_Succeeds()
    {
        // Arrange
        var config = new SessionizeConfiguration();
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Theory]
    [InlineData("https://sessionize.com", "https://sessionize.com/api/v2/")]
    [InlineData("https://sessionize.com/", "https://sessionize.com/api/v2/")]
    [InlineData("https://sessionize.com/api/v2", "https://sessionize.com/api/v2/")]
    [InlineData("https://sessionize.com/api/v2/", "https://sessionize.com/api/v2/")]
    [InlineData("https://api.example.com", "https://api.example.com/api/v2/")]
    public void Configuration_AppendsApiV2Path_WhenNotPresent(string input, string expected)
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = input
        };

        // Act
        var result = config.BaseUrl;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConfigurationValidation_WithHttpUrl_FailsValidation()
    {
        // Arrange - create config with pre-set HTTP URL (bypassing setter validation for this test)
        var validation = new SessionizeConfigurationValidation();

        // We can't test this directly due to the setter validation
        // This test ensures validation layer catches it even if setter is bypassed
        var config = new SessionizeConfiguration();

        // Set via reflection to bypass setter for testing validation
        var field = typeof(SessionizeConfiguration).GetField("_baseUrl",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(config, "http://sessionize.com");

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("HTTPS", result.FailureMessage);
    }

    [Fact]
    public void ConfigurationValidation_WithInvalidUri_FailsValidation()
    {
        // Arrange
        var validation = new SessionizeConfigurationValidation();
        var config = new SessionizeConfiguration();

        // Set via reflection to bypass setter for testing validation
        var field = typeof(SessionizeConfiguration).GetField("_baseUrl",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(config, "not-a-valid-url");

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("URI", result.FailureMessage);
    }

    [Theory]
    [InlineData("https://malicious-site.com")]
    [InlineData("https://attacker.example.com")]
    public void Configuration_AllowsAnyHttpsDomain_ButValidatesFormat(string url)
    {
        // Arrange & Act
        var config = new SessionizeConfiguration
        {
            BaseUrl = url
        };

        // Assert - should not throw as it's a valid HTTPS URL
        // Note: Domain restriction is out of scope for this library
        // Consumers should restrict domains in their own configuration
        Assert.NotNull(config.BaseUrl);
        Assert.StartsWith("https://", config.BaseUrl);
    }
}
