# CLAUDE.md

Sessionize.Api.Client is a .NET 10 class library, published to NuGet, that downloads sessions, speakers, schedules and rooms from the Sessionize API (`https://sessionize.com/api/v2/{apiId}/view/{View}`). It has no UI and does not authenticate users; the Sessionize API id is the only access setting.

## Solution structure

All code is under `src/` in `src/Sessionize API Client.sln`:

| Project | Role |
|---|---|
| `src/Sessionize.Api.Client` | The packable library |
| `src/Sessionize.Api.Client.Tests` | Unit tests; HTTP is mocked, no network |
| `src/Sessionize.Api.Client.IntegrationTests` | Tests against the live Sessionize API, configured with `appsettings-*.json` files |

## Architecture and boundaries

- `Abstractions/ISessionizeApiClient` is the public contract consumers inject. `SessionizeApiClient` implements it; each public method maps to one Sessionize view (`All`, `GridSmart`, `Speakers`, `Sessions`, `SpeakerWall`), with an optional overload that takes a `SessionFilter` or `SpeakerFilter` for client-side filtering.
- `Configuration/SessionizeConfiguration` holds the options from the `Sessionize` section (`BaseUrl`, which must be HTTPS and is normalised to end in `/api/v2/`, and `ApiId`).
- `DependencyInjection/ServiceCollectionExtensions.AddSessionizeApiClient` registers the options, an HttpClient with the standard resilience handler, and `ISessionizeApiClient` as scoped.
- `DataTransferObjects/` and `ValueObjects/` contain immutable positional records; `Filters/` contains filter classes; `Exceptions/` contains `SessionizeApiClientException` and the `ErrorCode` catalogue.
- Dependency direction: models, filters and exceptions depend on nothing else in the library; the client depends on them; the DI extensions depend on the client and configuration; test projects depend on the library only.
- Rules: never create `HttpClient` directly (use `IHttpClientFactory`), pass the optional `CancellationToken` through, throw `SessionizeApiClientException` with an `ErrorCode` for failures, validate API ids before building paths, and do not break public signatures of the published package.

Details: `.claude/skills/client-library-design/SKILL.md`.

## Libraries and versions

All projects target `net10.0` with `Nullable` and `ImplicitUsings` enabled. Pin exact versions and keep Microsoft.Extensions packages on the 10.x line.

Library (`src/Sessionize.Api.Client/Sessionize.Api.Client.csproj`):

- Microsoft.Extensions.Http.Resilience 10.5.0 (the only package dependency; serialization uses the built-in System.Text.Json)

Unit tests (`src/Sessionize.Api.Client.Tests`):

- xunit 2.9.3, xunit.runner.visualstudio 3.1.5, Microsoft.NET.Test.Sdk 18.5.1, coverlet.collector 10.0.0
- Moq 4.20.72, Moq.Contrib.HttpClient 1.4.0
- Microsoft.Extensions.Http 10.0.7, Microsoft.Extensions.Logging.Abstractions 10.0.7, Microsoft.Extensions.Options 10.0.7

Integration tests (`src/Sessionize.Api.Client.IntegrationTests`):

- xunit 2.9.3, xunit.runner.visualstudio 3.1.5, Microsoft.NET.Test.Sdk 18.5.1, coverlet.collector 10.0.0, Moq 4.20.72
- Microsoft.Extensions.Configuration 10.0.7, Microsoft.Extensions.Configuration.Json 10.0.7, Microsoft.Extensions.DependencyInjection.Abstractions 10.0.7

## Build and test

```
dotnet build src
dotnet test src
dotnet pack src/Sessionize.Api.Client/Sessionize.Api.Client.csproj -c Release
```

The integration tests need network access and `appsettings-public-api.json` (created in CI from a secret). See `.claude/skills/build-and-test/SKILL.md`.

## Skills

- `.claude/skills/build-and-test/SKILL.md`: building, testing and packing.
- `.claude/skills/add-sessionize-endpoint/SKILL.md`: how to change the library, for example adding a Sessionize view with tests.
- `.claude/skills/client-library-design/SKILL.md`: architecture and design conventions.

## Critic agents

Read-only reviewers in `.claude/agents/`:

- `security-critic`: HTTPS, API id validation, deserialization, secrets and logging.
- `dotnet-critic`: public surface, HttpClient and resilience, async and cancellation, nullability, tests.
- `code-style-critic`: naming, namespaces and layout conventions of the existing code.

## MCP

`.mcp.json` configures the `microsoft-learn` server for .NET documentation.
