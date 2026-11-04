---
name: build-and-test
description: How to restore, build, test and pack the Sessionize API client solution with the dotnet CLI, including the unit and integration test projects.
---

# Build and test

The solution lives in `src/` (`src/Sessionize API Client.sln`). All projects target `net10.0`, so a .NET 10 SDK is required. These are the same commands the CI workflow (`.github/workflows/main.yml`) runs.

## Build

```
dotnet build src
dotnet build src -c Release
```

## Test

Run all tests (unit and integration):

```
dotnet test src
```

Run only the unit tests (no network needed; HTTP is mocked with Moq.Contrib.HttpClient):

```
dotnet test src/Sessionize.Api.Client.Tests
```

Run only the integration tests (they call the live Sessionize API):

```
dotnet test src/Sessionize.Api.Client.IntegrationTests
```

The integration tests read configuration files that are copied to the output directory:

- `appsettings-with-api-id.json` (a public demo API id),
- `appsettings-without-api-id.json`,
- `appsettings-public-api.json` (CI creates it from the `SESSIONIZE_API_ID` secret; create it locally with a `Sessionize` section containing `BaseUrl` and `ApiId` if it is missing, and never commit a real id).

Collect coverage with the coverlet collector referenced by both test projects:

```
dotnet test src --collect:"XPlat Code Coverage"
```

## Pack

```
dotnet pack src/Sessionize.Api.Client/Sessionize.Api.Client.csproj -c Release
```

CI sets `AssemblyVersion`, `FileVersion`, `InformationalVersion` and `PackageVersion` from GitVersion; locally you can pass `-p:PackageVersion=x.y.z`. The package includes `LICENSE`, `README.md` and `sessionize-icon.png`. Publishing to NuGet only happens from CI on `main`.

## Before you finish

- `dotnet build src` has no new warnings (nullable is enabled).
- `dotnet test src/Sessionize.Api.Client.Tests` passes.
- Run the integration tests when you changed request building, endpoints or DTOs.
