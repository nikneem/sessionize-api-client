# Sessionize API Client

A .NET client library (published to NuGet as `Sessionize.Api.Client`) that downloads sessions, speakers, schedule grids and other conference data from the Sessionize API (`https://sessionize.com/api/v2/{apiId}/view/{View}`). There is no UI and no user authentication; callers identify an event by its Sessionize API id.

## Repository layout

- `src/Sessionize API Client.sln` - the solution.
- `src/Sessionize.Api.Client` - the library (the only packable project).
- `src/Sessionize.Api.Client.Tests` - unit tests; HTTP is mocked, no network.
- `src/Sessionize.Api.Client.IntegrationTests` - tests against the live Sessionize API, configured through `appsettings-*.json` files.
- `.github/workflows/main.yml` - CI: build, test, pack with GitVersion, publish to NuGet from `main`.

## Architecture

Inside `src/Sessionize.Api.Client`, folders map to namespaces:

| Folder | Role |
|---|---|
| `Abstractions/` | `ISessionizeApiClient`, the contract consumers depend on |
| root | `SessionizeApiClient`, request building, sending, deserializing, filtering |
| `Configuration/` | `SessionizeConfiguration` options (section `Sessionize`: `BaseUrl`, `ApiId`) |
| `DependencyInjection/` | `AddSessionizeApiClient` service collection extensions |
| `DataTransferObjects/` | top-level response records (`...Response`) |
| `ValueObjects/` | nested records used by the responses |
| `Filters/` | `SessionFilter`, `SpeakerFilter` for client-side filtering |
| `Exceptions/` | `SessionizeApiClientException` and the `ErrorCode` catalogue |

Dependency rules:

- DTOs, value objects, filters and exceptions depend on nothing else in the library.
- `SessionizeApiClient` depends on them; the DI extensions depend on the client and configuration.
- Test projects reference the library; the library never references tests.
- HTTP goes through `IHttpClientFactory` only (never `new HttpClient()`), with the standard resilience handler registered in DI.
- Failures are thrown as `SessionizeApiClientException` with an `ErrorCode`, never a bare `Exception`.
- Public methods are `Task`-returning, end in `Async` and take `string? sessionizeApiId = null, CancellationToken? cancellationToken = null` last. Public signatures must not break: the package is published.

The full design is in the `client-library-design` skill.

## Libraries and versions

All projects target `net10.0` (a .NET 10 SDK is required) with `Nullable` and `ImplicitUsings` enabled. JSON is handled with the built-in `System.Text.Json`.

Library `src/Sessionize.Api.Client/Sessionize.Api.Client.csproj`:

| Package | Version |
|---|---|
| Microsoft.Extensions.Http.Resilience | 10.5.0 |

Unit tests `src/Sessionize.Api.Client.Tests`:

| Package | Version |
|---|---|
| Microsoft.Extensions.Http | 10.0.7 |
| Microsoft.Extensions.Logging.Abstractions | 10.0.7 |
| Microsoft.Extensions.Options | 10.0.7 |
| Microsoft.NET.Test.Sdk | 18.5.1 |
| xunit | 2.9.3 |
| xunit.runner.visualstudio | 3.1.5 |
| Moq | 4.20.72 |
| Moq.Contrib.HttpClient | 1.4.0 |
| coverlet.collector | 10.0.0 |

Integration tests `src/Sessionize.Api.Client.IntegrationTests`:

| Package | Version |
|---|---|
| Microsoft.Extensions.Configuration | 10.0.7 |
| Microsoft.Extensions.Configuration.Json | 10.0.7 |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.7 |
| Microsoft.NET.Test.Sdk | 18.5.1 |
| xunit | 2.9.3 |
| xunit.runner.visualstudio | 3.1.5 |
| Moq | 4.20.72 |
| coverlet.collector | 10.0.0 |

Dependency rules:

- Versions are pinned exactly in each `.csproj` (there is no central package management).
- Keep Microsoft.Extensions packages on the 10.x line already in use; do not mix major versions.
- Keep the library lean: every package it references is carried by consumers. Add a library dependency only when the existing Microsoft.Extensions and BCL APIs cannot do the job.
- Use `System.Text.Json` for serialization; do not add Newtonsoft.Json or another JSON library.
- Tests use xUnit with Moq and Moq.Contrib.HttpClient; do not introduce another test or mocking framework. Unit tests never call the live API.

## Build and test

```
dotnet build src
dotnet test src/Sessionize.Api.Client.Tests
dotnet test src
dotnet pack src/Sessionize.Api.Client/Sessionize.Api.Client.csproj -c Release
```

Integration tests need network access and an `appsettings-public-api.json` with a `Sessionize` section; never commit a real API id. Details are in the `build-and-test` skill.

## Agents and skills

Critic agents in `.claude/agents/` (read-only reviewers):

- `dotnet-critic` - C#, project files, DI, HttpClient usage, tests and package versions.
- `security-critic` - HTTP calls, configuration, URL building, deserialization, logging, CI.
- `code-style-critic` - naming, layout and consistency with the existing code.

Skills in `.claude/skills/`:

- `build-and-test` - restore, build, test and pack.
- `add-sessionize-endpoint` - how to change the library, for example adding a Sessionize view.
- `client-library-design` - architecture and design conventions.
- `openspec-propose`, `openspec-explore`, `openspec-apply-change`, `openspec-sync-specs`, `openspec-archive-change` - the OpenSpec change workflow.

After a change, verify with `build-and-test`, then ask the critic agents for a review.
