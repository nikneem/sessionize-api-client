using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;
using Sessionize.Api.Client.Filters;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetSpeakerWallTests : SessionizeIntegrationTestBase
{
    private const string PublicApiId = "t9hbeiv7";

    [Fact]
    public async Task GetSpeakerWall_WithPublicApiId_ReturnsSpeakerWall()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSpeakerWall_WithApiIdParameter_ReturnsSpeakerWall()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync(PublicApiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSpeakerWall_VerifiesDataStructure_HasValidId()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync();

        // Assert
        var firstSpeaker = result.FirstOrDefault();
        Assert.NotNull(firstSpeaker);
        Assert.NotEqual(Guid.Empty, firstSpeaker.Id);
    }

    [Fact]
    public async Task GetSpeakerWall_VerifiesDataStructure_HasFullName()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync();

        // Assert
        var firstSpeaker = result.FirstOrDefault();
        Assert.NotNull(firstSpeaker);
        Assert.False(string.IsNullOrEmpty(firstSpeaker.FullName));
    }

    [Fact]
    public async Task GetSpeakerWall_VerifiesDataStructure_CountMatchesExpectedData()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync();

        // Assert
        Assert.True(result.Count > 0);
    }

    [Fact]
    public async Task GetSpeakerWall_WithSpeakerFilter_ReturnsFilteredSpeakers()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        var filter = new SpeakerFilter();

        // Act
        var result = await client.GetSpeakerWallAsync(filter, PublicApiId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSpeakerWall_WithoutConfiguration_ThrowsException()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act & Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(() => client.GetSpeakerWallAsync());
    }

    [Fact]
    public async Task FetchSpeakerWall_WhenConfiguredWithAppSettings_WhenCalled_ReturnsSpeakerWall()
    {
        WithAppSettingsConfiguration("appsettings-with-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSpeakerWall_WhenConfiguredManually_WhenCalled_ReturnsSpeakerWall()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSpeakerWall_WhenApiIdParameterPassed_WhenCalled_ReturnsSpeakerWall()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakerWallAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSpeakerWall_WhenUnconfigured_WhenCalled_ReturnsSpeakerWall()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var act = () => client.GetSpeakerWallAsync();

        // Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(act);
    }
}