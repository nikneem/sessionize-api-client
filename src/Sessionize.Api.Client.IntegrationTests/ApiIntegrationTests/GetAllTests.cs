using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetAllTests : SessionizeIntegrationTestBase
{
    [Fact]
    public async Task GetSchedule_WhenConfiguredWithAppSettings_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-with-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetAllDataAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task FetchSchedule_WhenConfiguredManually_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        client.SessionizeApiId = "45br5oxc";

        // Act
        var result = await client.GetAllDataAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task FetchSchedule_WhenPassingApiId_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetAllDataAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task FetchSchedule_WhenUnconfigured_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var act = () => client.GetAllDataAsync();

        // Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(act);
    }
}