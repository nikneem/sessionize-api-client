using Sessionize.Api.Client.DataTransferObjects;

namespace Sessionize.Api.Client.Abstractions;

public interface ISessionizeApiClient
{
    string? SessionizeApiId { get; set; }
    Task<AllDataResponse> GetAllDataAsync(string? sessionizeApiId = null,CancellationToken? cancellationToken = null);
    Task<List<ScheduleGridResponse>> GetScheduleGridAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerDetailsResponse>> GetSpeakersListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SessionListResponse>> GetSessionsListAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
    Task<List<SpeakerWallResponse>> GetSpeakerWallAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Checks whether data for the specified view has changed by fetching a lightweight hash
    /// using the <c>?hashonly=true</c> query parameter and comparing it against a previously stored hash.
    /// This avoids downloading the full response payload when data hasn't changed.
    /// </summary>
    /// <param name="viewName">The Sessionize view name (e.g., "All", "Sessions", "Speakers", "GridSmart", "SpeakerWall").</param>
    /// <param name="lastKnownHash">The hash from a previous call. If null, the method will return HasChanged=true along with the current hash.</param>
    /// <param name="sessionizeApiId">Optional Sessionize API ID. Uses the configured default if not provided.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A <see cref="DataChangedResponse"/> indicating whether data has changed and the current hash value.</returns>
    Task<DataChangedResponse> HasDataChangedAsync(string viewName, string? lastKnownHash = null, string? sessionizeApiId = null, CancellationToken? cancellationToken = null);
}