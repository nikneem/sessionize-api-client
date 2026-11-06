# Tasks

## 1. Setup and configuration sections

- [ ] 1.1 Add an "Installation" section with `dotnet add package Sessionize.Api.Client`, the nuget.org link, and the supported target framework(s) read from `src/Sessionize.Api.Client/Sessionize.Api.Client.csproj`. Verify that the listed frameworks match the csproj `TargetFramework`/`TargetFrameworks` value.
- [ ] 1.2 Rewrite "Getting Started" as: install, configure, register with DI, inject `ISessionizeApiClient`. Remove the "create an instance of `SessionizeApiClient`" step. Verify by reading that the steps match section 1.3/1.4 and the Usage section.
- [ ] 1.3 Update "Configuration":
  - `BaseUrl` is optional, defaults to `https://sessionize.com`, must be HTTPS, and gets `/api/v2` added automatically.
  - The API ID comes from the API/Embed settings and must be 8–12 alphanumeric characters.
  - The API ID is resolved in this order: per-call argument, then the `SessionizeApiId` init property, then `Sessionize:ApiId`.
  - If no API ID is set anywhere, the call fails with `InvalidConfiguration`.

  Verify each statement against `SessionizeConfiguration.cs` and `SessionizeApiClient.GetViewEndpoint`.
- [ ] 1.4 Rename the "Depencency Injection" heading to "Dependency Injection". Document both `AddSessionizeApiClient` overloads with code samples, including the optional section-name parameter, the scoped `ISessionizeApiClient` registration, and the standard resilience handler. Verify against `DependencyInjection/ServiceCollectionExtensions.cs`.

## 2. Usage, filtering, cancellation and errors

- [ ] 2.1 Add a "Usage" section with a table of the five `ISessionizeApiClient` methods, the Sessionize view each calls (`All`, `GridSmart`, `Sessions`, `Speakers`, `SpeakerWall`), and the return type of each. Add one end-to-end sample that injects the client and calls a method. Verify that the table lists exactly the methods in `Abstractions/ISessionizeApiClient.cs`.
- [ ] 2.2 Add a "Filtering" section:
  - `SessionFilter` (Title, Description, StartDate, EndDate) applies to `GetAllDataAsync`, `GetScheduleGridAsync` and `GetSessionsListAsync`.
  - `SpeakerFilter` (FirstName, LastName, IsTopSpeaker) applies to `GetSpeakersListAsync` and `GetSpeakerWallAsync`. `IsTopSpeaker` only affects the speaker wall.
  - Text matches are case-insensitive "contains" matches.
  - Filtering happens on the client after download.

  Verify against the `Matches*Filter` methods in `SessionizeApiClient.cs`.
- [ ] 2.3 Add a cancellation note and a sample call that passes a `CancellationToken`. Verify that every interface method has a `CancellationToken?` parameter.
- [ ] 2.4 Add an "Error Handling" section:
  - `SessionizeApiClientException` with `InvalidConfiguration`, `InvalidApiId`, `SessionizeResponseEmpty` and `DeserializationFailed`.
  - `ArgumentException` for an invalid `BaseUrl`.
  - `HttpRequestException` for non-success HTTP status codes.

  Verify that every listed error code is thrown somewhere in `src/Sessionize.Api.Client`.

## 3. Navigation and final checks

- [ ] 3.1 Update the "Features" bullets and the navigation links so they match the new sections. Change the `LICENSE` link to an absolute GitHub URL. Leave all badge lines unchanged. Verify that every `#anchor` in the navigation matches a heading, and that `git diff README.md` shows no badge lines changed.
- [ ] 3.2 Compile the usage, DI and cancellation samples against the library: temporarily paste them into a scratch console project that references `src/Sessionize.Api.Client`, then delete the project. Verify that `dotnet build` succeeds and no scratch files remain in the repository.
- [ ] 3.3 Run `dotnet pack src/Sessionize.Api.Client` and check that the packed `README.md` is the updated one. Verify that packing succeeds.
