using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.IntegrationTests.ApiIntegrationTests;

public class GetSpeakerWallTests : SessionizeIntegrationTestBase
{
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
        client.SessionizeApiId = "45br5oxc";

        // Act
        var result = await client.GetSpeakerWallAsync();

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

    //[Fact]
    //public async Task FetchSpeakerWall_WhenCallehhd_ReturnsSpeakerWall()
    //{
    //    // Arrange
    //    var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
    //    var logger = _serviceProvider.GetRequiredService<ILogger<SessionizeApiClient>>();
    //    var configuration = _serviceProvider.GetRequiredService<IOptions<SessionizeConfiguration>>();
    //    var client = new SessionizeApiClient(httpClientFactory, logger, configuration);


    //    // Act
    //    var result = await client.GetAllDataAsync();

    //    // Assert
    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public async Task FetchSpeakdderList_WhenCallehhd_ReturnsSpeakerWall()
    //{
    //    // Arrange
    //    var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
    //    var logger = _serviceProvider.GetRequiredService<ILogger<SessionizeApiClient>>();
    //    var configuration = _serviceProvider.GetRequiredService<IOptions<SessionizeConfiguration>>();
    //    var client = new SessionizeApiClient(httpClientFactory, logger, configuration);


    //    // Act
    //    var result = await client.GetScheduleGridAsync();

    //    // Assert
    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public async Task FetchSpeakddeddrList_WhenCallehhd_ReturnsSpeakerWall()
    //{
    //    // Arrange
    //    var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
    //    var logger = _serviceProvider.GetRequiredService<ILogger<SessionizeApiClient>>();
    //    var configuration = _serviceProvider.GetRequiredService<IOptions<SessionizeConfiguration>>();
    //    var client = new SessionizeApiClient(httpClientFactory, logger, configuration);


    //    // Act
    //    var result = await client.GetSessionsListAsync();

    //    // Assert
    //    Assert.NotNull(result);
    //}


    //[Fact]
    //public async Task FetchSpdeakddeddrList_WhenCallehhd_ReturnsSpeakerWall()
    //{
    //    // Arrange
    //    var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
    //    var logger = _serviceProvider.GetRequiredService<ILogger<SessionizeApiClient>>();
    //    var configuration = _serviceProvider.GetRequiredService<IOptions<SessionizeConfiguration>>();
    //    var client = new SessionizeApiClient(httpClientFactory, logger, configuration);


    //    // Act
    //    var result = await client.GetSpeakersListAsync();

    //    // Assert
    //    Assert.NotNull(result);
    //}

}