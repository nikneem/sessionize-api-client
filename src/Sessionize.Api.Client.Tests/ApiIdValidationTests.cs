using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.Exceptions;
using System.Net;

namespace Sessionize.Api.Client.Tests;

public class ApiIdValidationTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<SessionizeApiClient>> _loggerMock;
    private readonly Mock<IOptions<SessionizeConfiguration>> _configMock;

    public ApiIdValidationTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<SessionizeApiClient>>();
        _configMock = new Mock<IOptions<SessionizeConfiguration>>();
    }

    [Theory]
    [InlineData("45br5oxc")]      // 8 characters - valid
    [InlineData("testapi01")]     // 10 characters - valid
    [InlineData("abcdefgh")]      // 8 characters - valid
    [InlineData("123456789012")]  // 12 characters - valid
    [InlineData("ABC123def")]     // 9 characters - mixed case valid
    public async Task GetAllDataAsync_WithValidApiId_Succeeds(string validApiId)
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/"
        };
        _configMock.Setup(x => x.Value).Returns(config);

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(HttpMethod.Get, $"https://sessionize.com/api/v2/{validApiId}/view/All")
            .ReturnsResponse(HttpStatusCode.OK, "{\"sessions\":[],\"speakers\":[],\"rooms\":[]}");

        var httpClient = handler.CreateClient();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);

        // Act
        var result = await client.GetAllDataAsync(validApiId);

        // Assert
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("short")]           // Too short (5 chars)
    [InlineData("1234567")]         // Too short (7 chars)
    [InlineData("toolongapiid123")] // Too long (16 chars)
    [InlineData("test-api-id")]     // Contains hyphen
    [InlineData("test_api_id")]     // Contains underscore
    [InlineData("test@api")]        // Contains @ symbol
    [InlineData("../../../")]       // Path traversal attempt
    [InlineData("api id 01")]       // Contains space
    [InlineData("api.id.01")]       // Contains periods
    public async Task GetAllDataAsync_WithInvalidApiId_ThrowsException(string invalidApiId)
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/"
        };
        _configMock.Setup(x => x.Value).Returns(config);

        // Mock HTTP client (validation happens before HTTP call)
        var handler = new Mock<HttpMessageHandler>();
        var httpClient = handler.CreateClient();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SessionizeApiClientException>(() =>
            client.GetAllDataAsync(invalidApiId));

        Assert.Equal(ErrorCode.InvalidApiId, exception.ErrorCode);
    }

    [Fact]
    public async Task GetAllDataAsync_WithEmptyApiId_ThrowsException()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/"
        };
        _configMock.Setup(x => x.Value).Returns(config);

        // Mock HTTP client
        var handler = new Mock<HttpMessageHandler>();
        var httpClient = handler.CreateClient();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);

        // Act & Assert - empty string should trigger InvalidConfiguration error
        var exception = await Assert.ThrowsAsync<SessionizeApiClientException>(() =>
            client.GetAllDataAsync(""));

        Assert.Equal(ErrorCode.InvalidConfiguration, exception.ErrorCode);
    }

    [Fact]
    public async Task GetAllDataAsync_WithWhitespaceApiId_ThrowsException()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/"
        };
        _configMock.Setup(x => x.Value).Returns(config);

        // Mock HTTP client
        var handler = new Mock<HttpMessageHandler>();
        var httpClient = handler.CreateClient();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);

        // Act & Assert - whitespace fails regex validation
        var exception = await Assert.ThrowsAsync<SessionizeApiClientException>(() =>
            client.GetAllDataAsync("   "));

        Assert.Equal(ErrorCode.InvalidApiId, exception.ErrorCode);
    }

    [Theory]
    [InlineData("45br5oxc")]      // Valid
    [InlineData("testapi01")]     // Valid
    [InlineData("ABC123def")]     // Valid
    public void ConfigurationValidation_WithValidApiId_Succeeds(string validApiId)
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com",
            ApiId = validApiId
        };
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Theory]
    [InlineData("short")]           // Too short
    [InlineData("toolongapiid123")] // Too long
    [InlineData("test-api-id")]     // Contains hyphen
    [InlineData("test_api")]        // Contains underscore
    [InlineData("../../../")]       // Path traversal
    public void ConfigurationValidation_WithInvalidApiId_Fails(string invalidApiId)
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com",
            ApiId = invalidApiId
        };
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("alphanumeric", result.FailureMessage);
    }

    [Fact]
    public void ConfigurationValidation_WithNullApiId_Succeeds()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com",
            ApiId = null
        };
        var validation = new SessionizeConfigurationValidation();

        // Act
        var result = validation.Validate(null, config);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Fact]
    public async Task GetAllDataAsync_WithValidConfigApiId_Succeeds()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/",
            ApiId = "45br5oxc"
        };
        _configMock.Setup(x => x.Value).Returns(config);

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(HttpMethod.Get, "https://sessionize.com/api/v2/45br5oxc/view/All")
            .ReturnsResponse(HttpStatusCode.OK, "{\"sessions\":[],\"speakers\":[],\"rooms\":[]}");

        var httpClient = handler.CreateClient();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);

        // Act
        var result = await client.GetAllDataAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllDataAsync_WithInvalidConfigApiId_ThrowsException()
    {
        // Arrange
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/",
            ApiId = "invalid-id"
        };
        _configMock.Setup(x => x.Value).Returns(config);

        // Mock HTTP client (validation happens before HTTP call)
        var handler = new Mock<HttpMessageHandler>();
        var httpClient = handler.CreateClient();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SessionizeApiClientException>(() =>
            client.GetAllDataAsync());

        Assert.Equal(ErrorCode.InvalidApiId, exception.ErrorCode);
    }
}
