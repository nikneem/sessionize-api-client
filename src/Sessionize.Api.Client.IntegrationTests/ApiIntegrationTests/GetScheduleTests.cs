using FluentAssertions;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetScheduleTests : SessionizeIntegrationTestBase
{
    [Fact]
    public async Task GetSchedule_WhenConfiguredWithAppSettings_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-with-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetScheduleGridAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
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
        var result = await client.GetScheduleGridAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSchedule_WhenUnconfigured_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var act = () => client.GetScheduleGridAsync();

        // Assert
        await act.Should().ThrowAsync<SessionizeApiClientException>();
    }
}