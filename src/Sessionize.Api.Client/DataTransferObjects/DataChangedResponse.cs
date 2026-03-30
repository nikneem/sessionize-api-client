namespace Sessionize.Api.Client.DataTransferObjects;

/// <summary>
/// Response from a data change check against the Sessionize API.
/// Uses the <c>?hashonly=true</c> query parameter to retrieve a lightweight
/// hash of the data for a given view endpoint, avoiding a full data download.
/// </summary>
/// <param name="HasChanged">True if the server hash differs from the provided hash, or if no previous hash was provided.</param>
/// <param name="Hash">The current hash from the server. Store this value and pass it to subsequent calls.</param>
public record DataChangedResponse(bool HasChanged, string Hash);
