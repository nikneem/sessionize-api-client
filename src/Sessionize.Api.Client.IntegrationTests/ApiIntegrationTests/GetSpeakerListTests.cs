using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;
using Sessionize.Api.Client.Filters;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetSpeakerListTests : SessionizeIntegrationTestBase
{
    private const string PublicApiId = "t9hbeiv7";

    [Fact]
    public async Task GetSpeakersList_WithPublicApiId_ReturnsSpeakerList()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSpeakersList_WithApiIdParameter_ReturnsSpeakerList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync(PublicApiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSpeakersList_VerifiesDataStructure_HasValidId()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync();

        // Assert
        var firstSpeaker = result.FirstOrDefault();
        Assert.NotNull(firstSpeaker);
        Assert.NotEqual(Guid.Empty, firstSpeaker.Id);
    }

    [Fact]
    public async Task GetSpeakersList_VerifiesDataStructure_HasFullName()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync();

        // Assert
        var firstSpeaker = result.FirstOrDefault();
        Assert.NotNull(firstSpeaker);
        Assert.False(string.IsNullOrEmpty(firstSpeaker.FullName));
    }

    [Fact]
    public async Task GetSpeakersList_VerifiesDataStructure_HasSessions()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync();

        // Assert
        Assert.All(result, speaker => Assert.NotNull(speaker.Sessions));
    }

    [Fact]
    public async Task GetSpeakersList_WithSpeakerFilter_ReturnsFilteredSpeakers()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        var filter = new SpeakerFilter();

        // Act
        var result = await client.GetSpeakersListAsync(filter, PublicApiId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSpeakersList_WithoutConfiguration_ThrowsException()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act & Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(() => client.GetSpeakersListAsync());
    }

    [Fact]
    public async Task FetchSpeakerList_WhenConfiguredWithAppSettings_WhenCalled_ReturnsSpeakerList()
    {
        WithAppSettingsConfiguration("appsettings-with-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSpeakerList_WhenConfiguredManually_WhenCalled_ReturnsSpeakerList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSpeakerList_WhenApiIdParameterPassed_WhenCalled_ReturnsSpeakerList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSpeakersListAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSpeakerList_WhenUnconfigured_WhenCalled_ReturnsSpeakerList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var act = () => client.GetSpeakersListAsync();

        // Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(act);
    }
}