# Proposal

## Why

`README.md` is the repository front page. It is also the readme packed into the NuGet package (`PackageReadmeFile` in `src/Sessionize.Api.Client/Sessionize.Api.Client.csproj`). It no longer matches the library. It does not list the API methods, filters, cancellation support, per-call API ID, error codes, or the .NET 10 requirement. Some of what it does say is misleading. Issue #59 asks for the README to reflect the current project.

## What Changes

- Add an **Installation** section: the `dotnet add package Sessionize.Api.Client` command and the supported target framework (`net10.0`).
- Rewrite **Getting Started** so it matches how the library is used: register with `AddSessionizeApiClient(...)`, then inject `ISessionizeApiClient`. Remove the step that tells readers to construct `SessionizeApiClient` themselves.
- Update **Configuration**:
  - `BaseUrl` is optional, defaults to `https://sessionize.com`, must be HTTPS, and has `/api/v2` appended automatically.
  - The API ID must be 8–12 alphanumeric characters.
  - The API ID can come from three places, in this order of precedence: the per-call `sessionizeApiId` argument, the `SessionizeApiId` init property, or `Sessionize:ApiId` in configuration.
- Update **Dependency Injection**: document both `AddSessionizeApiClient` overloads (with an `IConfigurationBuilder` and optional section name, and without arguments), the scoped lifetime, and the built-in standard resilience handler. Fix the "Depencency" heading typo.
- Add a **Usage** section with a code sample and a table of the five methods (`GetAllDataAsync`, `GetScheduleGridAsync`, `GetSessionsListAsync`, `GetSpeakersListAsync`, `GetSpeakerWallAsync`), the Sessionize view each method calls, and its return type.
- Add a **Filtering** section for `SessionFilter` (Title, Description, StartDate, EndDate) and `SpeakerFilter` (FirstName, LastName, IsTopSpeaker). It says that filtering happens on the client after the full response is downloaded.
- Add a **Cancellation** note: every method accepts an optional `CancellationToken`.
- Add an **Error Handling** section: `SessionizeApiClientException` and the error codes it is actually raised with, `ArgumentException` for an invalid `BaseUrl`, and `HttpRequestException` for non-success HTTP status codes.
- Update **Features** and the navigation links so they match the new sections.

## Capabilities

### New Capabilities
- `package-documentation`: what the repository and NuGet package README must document so consumers can install, configure and use the library.

### Modified Capabilities
- None. `openspec/specs/` has no specs yet.

## Impact

- **Files**: only `README.md`. No code, API, or dependency changes.
- **NuGet**: the next package that is published will show the new readme on nuget.org.
- **Related in-flight changes**:
  - `replace-sonarcloud-with-local-linting` removes the SonarCloud badges from `README.md`. This change does not touch the badges.
  - `migrate-to-dotnet-11` adds a `net11.0` target. Whichever of the two is applied second must update the README's supported-framework line to match.

## Source issue

<!-- source-issue: nikneem/sessionize-api-client#59 -->
https://github.com/nikneem/sessionize-api-client/issues/59
