# Design

## Context

See proposal.md (Why) for the motivation. These facts about the current code determine what the README must say. Each was checked in `src/Sessionize.Api.Client/`:

- `ISessionizeApiClient` has five methods, each with two overloads (with and without a filter): `GetAllDataAsync` (view `All`), `GetScheduleGridAsync` (`GridSmart`), `GetSessionsListAsync` (`Sessions`), `GetSpeakersListAsync` (`Speakers`), and `GetSpeakerWallAsync` (`SpeakerWall`). Every overload takes an optional `sessionizeApiId` and an optional `CancellationToken?`.
- API ID resolution in `SessionizeApiClient.GetViewEndpoint` works in this order: the per-call argument, then the `SessionizeApiId` init property, then `SessionizeConfiguration.ApiId`. If all three are empty, the call throws `InvalidConfiguration`. If the ID does not match `^[a-zA-Z0-9]{8,12}$`, it throws `InvalidApiId`.
- `SessionizeConfiguration.BaseUrl` defaults to `https://sessionize.com/api/v2/` and appends `/api/v2/` when it is missing. Its setter throws `ArgumentException` for a non-absolute or non-HTTPS URL.
- `ErrorCode.InvalidBaseUrl` is declared but never thrown.
- `SessionizeConfigurationValidation` exists but is not registered by `AddSessionizeApiClient`.
- `AddSessionizeApiClient(IConfigurationBuilder, string? sectionName)` binds the `Sessionize` section (or a custom one) with `ValidateOnStart()`. `AddSessionizeApiClient()` only sets the default `BaseUrl`. Both overloads add `AddStandardResilienceHandler()` and register `ISessionizeApiClient` as scoped.
- Filters run on the client, after the full response is downloaded. Text matches are case-insensitive "contains" matches. `StartDate` keeps sessions that start on or after the date. `EndDate` keeps sessions that end on or before the date. `IsTopSpeaker` applies only to the speaker wall. On `GetAllDataAsync`, a filter also removes speakers who have no remaining sessions.
- The library targets `net10.0` only.

## Goals / Non-Goals

**Goals:**
- Every statement in the README can be checked against the code above.
- The README works when rendered both on GitHub and on nuget.org.

**Non-Goals:**
- Changing any code. That includes the unused `InvalidBaseUrl` code and the validator that is never registered. Those are separate issues.
- Badges. The SonarCloud badges belong to `replace-sonarcloud-with-local-linting`.
- Contributor or build documentation. That belongs in `CONTRIBUTING.md` and `CLAUDE.md`.

## Decisions

- **Edit `README.md` in place, keeping the header, badges and License section.** A full rewrite would conflict with the in-flight badge change. An in-place edit keeps the diff reviewable.
- **Document the behavior the code actually has, not what it intends.** For example, the README documents `ArgumentException` for a bad `BaseUrl` and leaves out `InvalidBaseUrl`. Documenting a code that is never thrown would mislead consumers. Alternative considered: list all five codes and mark one as reserved. Rejected, because it is noise for readers.
- **Lead with dependency injection.** Remove the "create an instance" step. The constructors need `IHttpClientFactory`, `ILogger` and `IOptions`, so DI is the realistic path. Manual construction gets one short note about the `SessionizeApiId` init property.
- **Use a method table and one end-to-end sample** instead of a sample for every method. The overloads are uniform, so one sample shows the pattern.
- **Use absolute URLs** for any links to repository files. nuget.org does not resolve relative links. The existing `LICENSE` link becomes an absolute GitHub URL.

## Assumptions

The issue says only that the README is "outdated" and does not say which parts. Nobody could be asked, so these assumptions were made:

- "Reflect the current state" means documenting the public API, configuration, DI, filtering, cancellation and errors as they exist on the `development` branch today.
- The README stays in English and keeps its current tone and header layout.
- Release notes and a changelog are out of scope.
- Framework versions: the README states `net10.0`. If `migrate-to-dotnet-11` lands first, the implementer states whatever the library csproj targets at that time.

## Risks / Trade-offs

- [The README drifts again as the API changes] → The spec's "method list is complete" scenario gives reviewers a concrete check. Future changes that touch the public API should update the README too.
- [The sample code stops compiling after API changes] → Before finishing, compile the sample against the current project (task 3.2).
- [Merge conflict with the SonarCloud badge change] → This change does not touch the badge lines, so any conflict is limited to neighbouring lines and is easy to resolve.
