using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Tests;

public class HasDataChangedTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SessionizeApiClient>> _logger = new();
    private readonly IOptions<SessionizeConfiguration> _configuration;

    public HasDataChangedTests()
    {
        _configuration = Options.Create(new SessionizeConfiguration
        {
            ApiId = "test-api-id",
            BaseUrl = "https://sessionize.com/api/v2/"
        });
    }

    private SessionizeApiClient CreateClient(HttpResponseMessage response)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handler.Object);
        _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return new SessionizeApiClient(_httpClientFactory.Object, _logger.Object, _configuration);
    }

    [Fact]
    public async Task HasDataChanged_SendsGetRequestWithHashOnlyParameter()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("abc123hash")
            });

        var httpClient = new HttpClient(handler.Object);
        _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        var client = new SessionizeApiClient(_httpClientFactory.Object, _logger.Object, _configuration);

        // Act
        await client.HasDataChangedAsync("All");

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Get);
        capturedRequest.RequestUri!.ToString().Should().Contain("test-api-id/view/All?hashonly=true");
    }

    [Fact]
    public async Task HasDataChanged_WithEmptyResponse_ReturnsChangedWithEmptyHash()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("")
        };
        var client = CreateClient(response);

        // Act
        var result = await client.HasDataChangedAsync("All");

        // Assert
        result.HasChanged.Should().BeTrue();
        result.Hash.Should().BeEmpty();
    }

    [Fact]
    public async Task HasDataChanged_WithNoPreviousHash_ReturnsChangedWithServerHash()
    {
        // Arrange
        var serverHash = "aa94b4bbb12bdbcbb9b5059e7dfa37818ebd5f8e";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(serverHash)
        };
        var client = CreateClient(response);

        // Act
        var result = await client.HasDataChangedAsync("All");

        // Assert
        result.HasChanged.Should().BeTrue();
        result.Hash.Should().Be(serverHash);
    }

    [Fact]
    public async Task HasDataChanged_WithDifferentHash_ReturnsChanged()
    {
        // Arrange
        var serverHash = "aa94b4bbb12bdbcbb9b5059e7dfa37818ebd5f8e";
        var localHash = "0000000000000000000000000000000000000000";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(serverHash)
        };
        var client = CreateClient(response);

        // Act
        var result = await client.HasDataChangedAsync("All", localHash);

        // Assert
        result.HasChanged.Should().BeTrue();
        result.Hash.Should().Be(serverHash);
    }

    [Fact]
    public async Task HasDataChanged_WithMatchingHash_ReturnsNotChanged()
    {
        // Arrange
        var hash = "aa94b4bbb12bdbcbb9b5059e7dfa37818ebd5f8e";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(hash)
        };
        var client = CreateClient(response);

        // Act
        var result = await client.HasDataChangedAsync("All", hash);

        // Assert
        result.HasChanged.Should().BeFalse();
        result.Hash.Should().Be(hash);
    }

    [Fact]
    public async Task HasDataChanged_HashComparisonIsCaseInsensitive()
    {
        // Arrange
        var serverHash = "AA94B4BBB12BDBCBB9B5059E7DFA37818EBD5F8E";
        var localHash = "aa94b4bbb12bdbcbb9b5059e7dfa37818ebd5f8e";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(serverHash)
        };
        var client = CreateClient(response);

        // Act
        var result = await client.HasDataChangedAsync("All", localHash);

        // Assert
        result.HasChanged.Should().BeFalse();
    }

    [Fact]
    public async Task HasDataChanged_UsesExplicitApiId_WhenProvided()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("somehash")
            });

        var httpClient = new HttpClient(handler.Object);
        _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        var client = new SessionizeApiClient(_httpClientFactory.Object, _logger.Object, _configuration);

        // Act
        await client.HasDataChangedAsync("All", sessionizeApiId: "custom-api-id");

        // Assert
        capturedRequest!.RequestUri!.ToString().Should().Contain("custom-api-id/view/All?hashonly=true");
    }
}
