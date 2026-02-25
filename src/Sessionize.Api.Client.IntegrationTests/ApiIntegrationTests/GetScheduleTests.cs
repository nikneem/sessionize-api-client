using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;
using Sessionize.Api.Client.Filters;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetScheduleTests : SessionizeIntegrationTestBase
{
    private const string PublicApiId = "t9hbeiv7";

    [Fact]
    public async Task GetScheduleGrid_WithPublicApiId_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
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
    public async Task GetScheduleGrid_WithApiIdParameter_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetScheduleGridAsync(PublicApiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetScheduleGrid_VerifiesDataStructure_HasValidDates()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetScheduleGridAsync();

        // Assert
        var firstSchedule = result.FirstOrDefault();
        Assert.NotNull(firstSchedule);
        Assert.NotEqual(DateTimeOffset.MinValue, firstSchedule.Date);
    }

    [Fact]
    public async Task GetScheduleGrid_VerifiesDataStructure_HasTimeSlots()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetScheduleGridAsync();

        // Assert
        Assert.All(result, schedule => Assert.NotNull(schedule.TimeSlots));
    }

    [Fact]
    public async Task GetScheduleGrid_VerifiesDataStructure_HasRooms()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetScheduleGridAsync();

        // Assert
        Assert.All(result, schedule => Assert.NotNull(schedule.Rooms));
    }

    [Fact]
    public async Task GetScheduleGrid_WithSessionFilter_ReturnsFilteredSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        var filter = new SessionFilter();

        // Act
        var result = await client.GetScheduleGridAsync(filter, PublicApiId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetScheduleGrid_WithoutConfiguration_ThrowsException()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act & Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(async () => await client.GetScheduleGridAsync());
    }

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

        // Act
        var result = await client.GetScheduleGridAsync("45br5oxc");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task FetchSchedule_WhenApiIdParameterPassed_WhenCalled_ReturnsSchedule()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetScheduleGridAsync("45br5oxc");

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

        // Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(async () => await client.GetScheduleGridAsync());
    }
}