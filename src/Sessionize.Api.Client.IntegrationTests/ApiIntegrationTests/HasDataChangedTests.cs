using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class HasDataChangedTests : SessionizeIntegrationTestBase
{
    private const string PublicApiId = "t9hbeiv7";

    [Fact]
    public async Task HasDataChanged_WithNoPreviousHash_ReturnsChangedWithHash()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.HasDataChangedAsync("All");

        // Assert
        Assert.True(result.HasChanged);
        Assert.NotEmpty(result.Hash);
    }

    [Fact]
    public async Task HasDataChanged_WithDifferentHash_ReturnsChanged()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        var fakeHash = "0000000000000000000000000000000000000000";

        // Act
        var result = await client.HasDataChangedAsync("All", fakeHash);

        // Assert
        Assert.True(result.HasChanged);
        Assert.NotEmpty(result.Hash);
    }

    [Fact]
    public async Task HasDataChanged_WithCurrentHash_ReturnsNotChanged()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // First, get the current hash
        var initial = await client.HasDataChangedAsync("All");
        Assert.NotEmpty(initial.Hash);

        // Act - check again with the hash we just got
        var result = await client.HasDataChangedAsync("All", initial.Hash);

        // Assert
        Assert.False(result.HasChanged);
        Assert.NotEmpty(result.Hash);
    }

    [Fact]
    public async Task HasDataChanged_WithApiIdParameter_ReturnsResult()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.HasDataChangedAsync("All", sessionizeApiId: PublicApiId);

        // Assert
        Assert.True(result.HasChanged);
        Assert.NotEmpty(result.Hash);
    }

    [Fact]
    public async Task HasDataChanged_WithDifferentViewNames_ReturnsDifferentHashes()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var allResult = await client.HasDataChangedAsync("All");
        var sessionsResult = await client.HasDataChangedAsync("Sessions");
        var speakersResult = await client.HasDataChangedAsync("Speakers");

        // Assert
        Assert.NotEmpty(allResult.Hash);
        Assert.NotEmpty(sessionsResult.Hash);
        Assert.NotEmpty(speakersResult.Hash);
        // Different views should return different hashes
        Assert.NotEqual(allResult.Hash, sessionsResult.Hash);
    }

    [Fact]
    public async Task HasDataChanged_WithoutConfiguration_ThrowsException()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act & Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(() =>
            client.HasDataChangedAsync("All"));
    }
}
