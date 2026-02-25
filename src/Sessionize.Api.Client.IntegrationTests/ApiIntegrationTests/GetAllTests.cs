using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;
using Sessionize.Api.Client.Filters;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetAllTests : SessionizeIntegrationTestBase
{
    private const string PublicApiId = "t9hbeiv7";

    [Fact]
    public async Task GetAllData_WithPublicApiId_ReturnsAllData()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetAllDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Sessions);
        Assert.NotEmpty(result.Sessions);
        Assert.NotNull(result.Speakers);
        Assert.NotEmpty(result.Speakers);
        Assert.NotNull(result.Rooms);
    }

    [Fact]
    public async Task GetAllData_WithApiIdParameter_ReturnsAllData()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetAllDataAsync(PublicApiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Sessions);
        Assert.NotEmpty(result.Sessions);
        Assert.NotNull(result.Speakers);
        Assert.NotEmpty(result.Speakers);
    }

    [Fact]
    public async Task GetAllData_WithSessionFilter_ReturnsFilteredData()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();
        var filter = new SessionFilter();

        // Act
        var result = await client.GetAllDataAsync(filter, PublicApiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Sessions);
    }

    [Fact]
    public async Task GetAllData_VerifiesDataStructure_HasValidSpeakers()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetAllDataAsync();

        // Assert
        var firstSpeaker = result.Speakers.FirstOrDefault();
        Assert.NotNull(firstSpeaker);
        Assert.False(string.IsNullOrEmpty(firstSpeaker.Id));
        Assert.False(string.IsNullOrEmpty(firstSpeaker.FullName));
    }

    [Fact]
    public async Task GetAllData_VerifiesDataStructure_HasValidSessions()
    {
        WithAppSettingsConfiguration("appsettings-public-api.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act
        var result = await client.GetAllDataAsync();

        // Assert
        var firstSession = result.Sessions.FirstOrDefault();
        Assert.NotNull(firstSession);
        Assert.False(string.IsNullOrEmpty(firstSession.Id));
        Assert.False(string.IsNullOrEmpty(firstSession.Title));
    }

    [Fact]
    public async Task GetAllData_WithoutConfiguration_ThrowsException()
    {
        WithAppSettingsConfiguration("appsettings-without-api-id.json");
        WithSessionizeClientRegistered();

        // Arrange
        var client = GetService<ISessionizeApiClient>();

        // Act & Assert
        await Assert.ThrowsAsync<SessionizeApiClientException>(() => client.GetAllDataAsync());
    }

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

        // Act
        var result = await client.GetAllDataAsync("45br5oxc");

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