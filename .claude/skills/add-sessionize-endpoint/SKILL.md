---
name: add-sessionize-endpoint
description: How to change the Sessionize API client library, for example adding support for a new Sessionize API view with DTOs, an optional filter, error codes and tests.
---

# Changing the library: add a Sessionize view

The client maps each Sessionize view (`All`, `GridSmart`, `Speakers`, `Sessions`, `SpeakerWall`) to a method on `ISessionizeApiClient`. Follow these steps to add one, and use the same order for other changes.

## Steps

1. **Model the response.** Add positional `record` types in `src/Sessionize.Api.Client/DataTransferObjects/` (top-level responses, named `...Response`) and `src/Sessionize.Api.Client/ValueObjects/` (nested parts). Property names are PascalCase; JSON is read with `JsonNamingPolicy.CamelCase`. Use nullable types for fields Sessionize may omit.
2. **Extend the contract.** Add the method to `src/Sessionize.Api.Client/Abstractions/ISessionizeApiClient.cs` with the shared signature:
   `Task<TResponse> GetXxxAsync(string? sessionizeApiId = null, CancellationToken? cancellationToken = null);`
   If the data can be filtered, add an overload that takes a filter first (`SessionFilter?` or `SpeakerFilter?`, or a new class in `Filters/`).
3. **Implement.** In `src/Sessionize.Api.Client/SessionizeApiClient.cs`, log with `_logger.LogInformation` and call `SendRequestAsync<TResponse>("ViewName", sessionizeApiId, cancellationToken)`. For filter overloads, return the response unchanged when the filter is null, otherwise return a filtered copy using LINQ and `with` expressions; never mutate the input.
4. **Errors.** If you add a failure mode, add a `static readonly ErrorCode` to `src/Sessionize.Api.Client/Exceptions/ErrorCode.cs` and throw `SessionizeApiClientException`.
5. **Configuration.** New settings go on `SessionizeConfiguration` (section `Sessionize`) and must keep the HTTPS validation of `BaseUrl`.
6. **Unit tests.** In `src/Sessionize.Api.Client.Tests`, mock `HttpMessageHandler` with Moq.Contrib.HttpClient, create the client from a mocked `IHttpClientFactory`, `IOptions<SessionizeConfiguration>` and a logger, and cover the happy path, filters, empty responses and invalid API ids.
7. **Integration test.** Add a test in `src/Sessionize.Api.Client.IntegrationTests` that uses the appsettings files and dependency injection (`AddSessionizeApiClient`).
8. **Docs and package.** Update `README.md` when public behaviour changes and `PackageReleaseNotes` in `Sessionize.Api.Client.csproj`. Do not break existing public signatures; the package is published to NuGet.
9. **Verify** with the `build-and-test` skill, then ask the `dotnet-critic`, `security-critic` and `code-style-critic` agents for a review.

## Dependencies

Only add packages when needed. Keep Microsoft.Extensions packages on the same major line (10.x) as the existing references, and pin exact versions.
