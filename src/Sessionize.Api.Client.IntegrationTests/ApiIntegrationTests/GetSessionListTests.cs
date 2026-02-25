using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;
using Sessionize.Api.Client.Filters;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetSessionListTests : SessionizeIntegrationTestBase
{
    private const string PublicApiId = "t9hbeiv7";

    [Fact]
    public async Task GetSessionsList_WithPublicApiId_ReturnsSessionList()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSessionsList_WithApiIdParameter_ReturnsSessionList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync(PublicApiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSessionsList_VerifiesDataStructure_HasValidId()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync();

        // Assert
        var firstGroup = result.FirstOrDefault();
        Assert.NotNull(firstGroup);
        Assert.NotNull(firstGroup.Sessions);
        var firstSession = firstGroup.Sessions.FirstOrDefault();
        Assert.NotNull(firstSession);
        Assert.False(string.IsNullOrEmpty(firstSession.Id));
    }

    [Fact]
    public async Task GetSessionsList_VerifiesDataStructure_HasTitle()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync();

        // Assert
        var firstGroup = result.FirstOrDefault();
        Assert.NotNull(firstGroup);
        Assert.NotNull(firstGroup.Sessions);
        var firstSession = firstGroup.Sessions.FirstOrDefault();
        Assert.NotNull(firstSession);
        Assert.False(string.IsNullOrEmpty(firstSession.Title));
    }

    [Fact]
    public async Task GetSessionsList_VerifiesDataStructure_HasSpeakers()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync();

        // Assert
        Assert.All(result, group =>
        {
            Assert.NotNull(group.Sessions);
            Assert.All(group.Sessions, session => Assert.NotNull(session.Speakers));
        });
    }

    [Fact]
    public async Task GetSessionsList_WithSessionFilter_ReturnsFilteredSessions()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        var filter = new SessionFilter();

        // Act
        var result = await client.GetSessionsListAsync(filter, PublicApiId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSessionsList_WithoutConfiguration_ThrowsException()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act & Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(() => client.GetSessionsListAsync());
    }

    [Fact]
    public async Task GetSessionList_WhenConfiguredWithAppSettings_WhenCalled_ReturnsSessionList()
    {
        WithAppSettingsConfiguration("appsettings-with-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSessionList_WhenConfiguredManually_WhenCalled_ReturnsSessionList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSessionList_WhenApiIdParameterPassed_WhenCalled_ReturnsSessionList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetSessionsListAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSessionList_WhenUnconfigured_WhenCalled_ReturnsSessionList()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var act = () => client.GetSessionsListAsync();

        // Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(act);
    }
}