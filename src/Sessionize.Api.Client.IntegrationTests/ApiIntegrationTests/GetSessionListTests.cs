using FluentAssertions;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetSessionListTests : SessionizeIntegrationTestBase
{
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
        client.SessionizeApiId = "45br5oxc";

        // Act
        var result = await client.GetSessionsListAsync();

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
        await act.Should().ThrowAsync<SessionizeApiClientException>();
    }
}