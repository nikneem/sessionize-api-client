using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetSpeakerListTests : SessionizeIntegrationTestBase
{
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
        client.SessionizeApiId = "45br5oxc";

        // Act
        var result = await client.GetSpeakersListAsync();

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