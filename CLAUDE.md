# Sessionize API Client

A .NET client library for the Sessionize API, published to NuGet as `Sessionize.Api.Client`. No UI, no authentication of users.

## Layout

- `src/Sessionize API Client.sln` - the solution.
- `src/Sessionize.Api.Client` - the library (contract in `Abstractions/`, `SessionizeApiClient.cs`, `Configuration/`, `DependencyInjection/`, `DataTransferObjects/`, `ValueObjects/`, `Filters/`, `Exceptions/`).
- `src/Sessionize.Api.Client.Tests` - unit tests with mocked HTTP.
- `src/Sessionize.Api.Client.IntegrationTests` - tests against the live Sessionize API.
- `openspec/` - OpenSpec changes and specs.

## Architecture

Consumers depend only on `ISessionizeApiClient`. Model folders depend on nothing else in the library; the client uses the model; the DI extensions compose client and configuration. Tests reference the library, never the reverse. Public signatures are a published API: add overloads, do not break them. Details: `architecture-patterns` and `client-library-design` skills.

## Libraries and versions

- Target framework `net10.0` for all projects (.NET 10 SDK), `Nullable` and `ImplicitUsings` enabled.
- Library: `Microsoft.Extensions.Http.Resilience` 10.5.0 (the only package reference); JSON via `System.Text.Json`.
- Tests: xunit 2.9.3, xunit.runner.visualstudio 3.1.5, Microsoft.NET.Test.Sdk 18.5.1, Moq 4.20.72, Moq.Contrib.HttpClient 1.4.0 (unit tests), coverlet.collector 10.0.0, Microsoft.Extensions.* 10.0.7.
- Keep Microsoft.Extensions packages on the 10.x line and pin exact versions.
- Excluded: creating `HttpClient` directly (use `IHttpClientFactory`), Newtonsoft.Json, bare `Exception` (use `SessionizeApiClientException` with an `ErrorCode`), plain HTTP base URLs.

## Commands

```
dotnet build src
dotnet test src
dotnet test src/Sessionize.Api.Client.Tests
dotnet pack src/Sessionize.Api.Client/Sessionize.Api.Client.csproj -c Release
```

Integration tests need `src/Sessionize.Api.Client.IntegrationTests/appsettings-public-api.json`; never commit a real API id. See the `build-and-test` skill.

## Skills (`.claude/skills/`)

- `build-and-test` - build, test (writing and running), coverage, pack.
- `add-sessionize-endpoint` - how to change the library, e.g. add a Sessionize view.
- `code-style` - naming, formatting and idioms.
- `architecture-patterns` - layers, boundaries and patterns.
- `client-library-design` - requests, resilience, serialization, filtering, DI.
- `issue-sync` - keep OpenSpec changes and GitHub issues in sync.
- `openspec-propose`, `openspec-explore`, `openspec-apply-change`, `openspec-sync-specs`, `openspec-archive-change` - the OpenSpec workflow.

## Review agents (`.claude/agents/`, read-only)

After changes, ask `dotnet-critic`, `security-critic` and `code-style-critic` for a review.

## MCP

`.mcp.json` configures `microsoft-learn` for .NET documentation.
