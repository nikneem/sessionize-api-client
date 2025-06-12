using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.DataTransferObjects;
using Sessionize.Api.Client.Filters;
using Sessionize.Api.Client.ValueObjects;
using System.Net;
using System.Text.Json;

namespace Sessionize.Api.Client.Tests;

public class SessionFilterTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<SessionizeApiClient>> _loggerMock;
    private readonly Mock<IOptions<SessionizeConfiguration>> _configMock;
    private readonly SessionizeApiClient _client;

    public SessionFilterTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<SessionizeApiClient>>();
        _configMock = new Mock<IOptions<SessionizeConfiguration>>();
        
        var config = new SessionizeConfiguration
        {
            BaseUrl = "https://sessionize.com/api/v2/",
            ApiId = "test-api-id"
        };
        _configMock.Setup(x => x.Value).Returns(config);
        
        _client = new SessionizeApiClient(_httpClientFactoryMock.Object, _loggerMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task GetAllDataAsync_WithSessionFilter_FiltersSessionsByTitle()
    {
        // Arrange
        var testData = CreateTestAllDataResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SessionFilter { Title = "Test Session 1" };

        // Act
        var result = await _client.GetAllDataAsync(filter);

        // Assert
        Assert.Single(result.Sessions);
        Assert.Equal("Test Session 1", result.Sessions[0].Title);
        Assert.Single(result.Speakers); // Speaker should remain because they have a matching session
    }

    [Fact]
    public async Task GetAllDataAsync_WithSessionFilter_FiltersSessionsByDescription()
    {
        // Arrange
        var testData = CreateTestAllDataResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SessionFilter { Description = "Description 1" };

        // Act
        var result = await _client.GetAllDataAsync(filter);

        // Assert
        Assert.Single(result.Sessions);
        Assert.Equal("Test Session 1", result.Sessions[0].Title);
    }

    [Fact]
    public async Task GetAllDataAsync_WithSessionFilter_FiltersSessionsByDateRange()
    {
        // Arrange
        var testData = CreateTestAllDataResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SessionFilter 
        { 
            StartDate = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2024, 1, 1, 11, 0, 0, TimeSpan.Zero)
        };

        // Act
        var result = await _client.GetAllDataAsync(filter);

        // Assert
        Assert.Single(result.Sessions);
        Assert.Equal("Test Session 1", result.Sessions[0].Title);
    }

    [Fact]
    public async Task GetAllDataAsync_WithSessionFilter_RemovesSpeakersWithNoRemainingSession()
    {
        // Arrange
        var testData = CreateTestAllDataResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SessionFilter { Title = "Test Session 2" }; // Only speaker 2 has this session

        // Act
        var result = await _client.GetAllDataAsync(filter);

        // Assert
        Assert.Single(result.Sessions);
        Assert.Equal("Test Session 2", result.Sessions[0].Title);
        Assert.Single(result.Speakers);
        Assert.Equal("Speaker", result.Speakers[0].FirstName);
        Assert.Equal("Two", result.Speakers[0].LastName);
    }

    [Fact]
    public async Task GetSpeakersListAsync_WithSpeakerFilter_FiltersByFirstName()
    {
        // Arrange
        var testData = CreateTestSpeakersResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SpeakerFilter { FirstName = "Speaker" };

        // Act
        var result = await _client.GetSpeakersListAsync(filter);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetSpeakerWallAsync_WithSpeakerFilter_FiltersByIsTopSpeaker()
    {
        // Arrange
        var testData = CreateTestSpeakerWallResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SpeakerFilter { IsTopSpeaker = true };

        // Act
        var result = await _client.GetSpeakerWallAsync(filter);

        // Assert
        Assert.Single(result);
        Assert.True(result[0].IsTopSpeaker);
    }

    [Fact]
    public async Task GetAllDataAsync_WithNullFilter_ReturnsAllData()
    {
        // Arrange
        var testData = CreateTestAllDataResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await _client.GetAllDataAsync((SessionFilter?)null);

        // Assert
        Assert.Equal(2, result.Sessions.Count);
        Assert.Equal(2, result.Speakers.Count);
    }

    [Fact]
    public async Task GetSessionsListAsync_WithSessionFilter_FiltersCorrectly()
    {
        // Arrange
        var testData = CreateTestSessionsListResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SessionFilter { Title = "Session 1" };

        // Act
        var result = await _client.GetSessionsListAsync(filter);

        // Assert
        Assert.Single(result);
        Assert.Single(result[0].Sessions);
        Assert.Equal("Test Session 1", result[0].Sessions[0].Title);
    }

    [Fact]
    public async Task GetAllDataAsync_WithComplexFilter_HandlesMultipleCriteria()
    {
        // Arrange
        var testData = CreateComplexTestAllDataResponse();
        var httpClient = SetupHttpClientWithResponse(testData);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var filter = new SessionFilter 
        { 
            Title = "Advanced",
            StartDate = new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2024, 1, 1, 16, 0, 0, TimeSpan.Zero)
        };

        // Act
        var result = await _client.GetAllDataAsync(filter);

        // Assert
        Assert.Single(result.Sessions);
        Assert.Equal("Advanced Session", result.Sessions[0].Title);
        Assert.Single(result.Speakers); // Only speaker with matching session should remain
        Assert.Equal("Advanced", result.Speakers[0].FirstName);
    }

    private HttpClient SetupHttpClientWithResponse<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupAnyRequest()
               .ReturnsResponse(HttpStatusCode.OK, json, "application/json");

        return handler.CreateClient();
    }

    private AllDataResponse CreateTestAllDataResponse()
    {
        return new AllDataResponse(
            Sessions: new List<SessionDetails>
            {
                new("1", "Test Session 1", "Description 1", 
                    new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 11, 0, 0, TimeSpan.Zero),
                    false, false, new List<string> { "speaker1" }, new List<object>(), new List<object>(),
                    1, "", "", "Confirmed", true, true),
                new("2", "Test Session 2", "Description 2", 
                    new DateTimeOffset(2024, 1, 2, 10, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 2, 11, 0, 0, TimeSpan.Zero),
                    false, false, new List<string> { "speaker2" }, new List<object>(), new List<object>(),
                    2, "", "", "Confirmed", true, true)
            },
            Speakers: new List<SpeakerDetails>
            {
                new("speaker1", "Speaker", "One", "Bio 1", "Tag 1", "pic1.jpg", false, 
                    new object[0], new int[] { 1 }, "Speaker One", new object[0], new object[0]),
                new("speaker2", "Speaker", "Two", "Bio 2", "Tag 2", "pic2.jpg", true, 
                    new object[0], new int[] { 2 }, "Speaker Two", new object[0], new object[0])
            },
            Rooms: new List<RoomName>
            {
                new(1, "Room 1", 1),
                new(2, "Room 2", 2)
            }
        );
    }

    private List<SpeakerDetailsResponse> CreateTestSpeakersResponse()
    {
        return new List<SpeakerDetailsResponse>
        {
            new(Guid.NewGuid(), "Speaker", "One", "Speaker One", "Bio 1", "Tag 1", "pic1.jpg", false, new SpeakerSession[0]),
            new(Guid.NewGuid(), "Speaker", "Two", "Speaker Two", "Bio 2", "Tag 2", "pic2.jpg", true, new SpeakerSession[0]),
            new(Guid.NewGuid(), "John", "Doe", "John Doe", "Bio 3", "Tag 3", "pic3.jpg", false, new SpeakerSession[0])
        };
    }

    private List<SpeakerWallResponse> CreateTestSpeakerWallResponse()
    {
        return new List<SpeakerWallResponse>
        {
            new(Guid.NewGuid(), "Speaker", "One", "Speaker One", "Tag 1", "pic1.jpg", false),
            new(Guid.NewGuid(), "Speaker", "Two", "Speaker Two", "Tag 2", "pic2.jpg", true)
        };
    }

    private List<SessionListResponse> CreateTestSessionsListResponse()
    {
        return new List<SessionListResponse>
        {
            new(true, "group1", "Group 1", new List<SessionInfo>
            {
                new("1", "Test Session 1", "Description 1", 
                    new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), 
                    new DateTimeOffset(2024, 1, 1, 11, 0, 0, TimeSpan.Zero),
                    false, false, 1, "Room 1", "Confirmed", true, true, 
                    new List<SessionSpeaker> { new("speaker1", "Speaker One") }),
                new("2", "Test Session 2", "Description 2", 
                    new DateTimeOffset(2024, 1, 2, 10, 0, 0, TimeSpan.Zero), 
                    new DateTimeOffset(2024, 1, 2, 11, 0, 0, TimeSpan.Zero),
                    false, false, 2, "Room 2", "Confirmed", true, true, 
                    new List<SessionSpeaker> { new("speaker2", "Speaker Two") })
            })
        };
    }

    private AllDataResponse CreateComplexTestAllDataResponse()
    {
        return new AllDataResponse(
            Sessions: new List<SessionDetails>
            {
                new("1", "Basic Session", "Basic description", 
                    new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 11, 0, 0),
                    false, false, new List<string> { "speaker1" }, new List<object>(), new List<object>(),
                    1, "", "", "Confirmed", true, true),
                new("2", "Advanced Session", "Advanced description", 
                    new DateTime(2024, 1, 1, 15, 0, 0), new DateTime(2024, 1, 1, 16, 0, 0),
                    false, false, new List<string> { "speaker2" }, new List<object>(), new List<object>(),
                    2, "", "", "Confirmed", true, true),
                new("3", "Expert Session", "Expert description", 
                    new DateTime(2024, 1, 2, 10, 0, 0), new DateTime(2024, 1, 2, 11, 0, 0),
                    false, false, new List<string> { "speaker3" }, new List<object>(), new List<object>(),
                    3, "", "", "Confirmed", true, true)
            },
            Speakers: new List<SpeakerDetails>
            {
                new("speaker1", "Basic", "Speaker", "Bio 1", "Tag 1", "pic1.jpg", false, 
                    new object[0], new int[] { 1 }, "Basic Speaker", new object[0], new object[0]),
                new("speaker2", "Advanced", "Speaker", "Bio 2", "Tag 2", "pic2.jpg", true, 
                    new object[0], new int[] { 2 }, "Advanced Speaker", new object[0], new object[0]),
                new("speaker3", "Expert", "Speaker", "Bio 3", "Tag 3", "pic3.jpg", false, 
                    new object[0], new int[] { 3 }, "Expert Speaker", new object[0], new object[0])
            },
            Rooms: new List<RoomName>
            {
                new(1, "Room 1", 1),
                new(2, "Room 2", 2),
                new(3, "Room 3", 3)
            }
        );
    }
}