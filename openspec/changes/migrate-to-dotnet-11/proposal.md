# Proposal

## Why

.NET 11 is due for general availability on 11 November 2026. The solution currently targets `net10.0` only, so it cannot use .NET 11 and does not ship a build made for consumers who move to .NET 11. Issue #53 asks for the projects to be updated to .NET 11 once it is released. This work MUST NOT start before the .NET 11 GA SDK is available.

## What Changes

- The published library `src/Sessionize.Api.Client` multi-targets `net10.0;net11.0`. It adds a .NET 11 build and keeps the .NET 10 (LTS) build, so existing consumers are not cut off.
- The unit test project `src/Sessionize.Api.Client.Tests` and the integration test project `src/Sessionize.Api.Client.IntegrationTests` move from `net10.0` to `net11.0`.
- `Microsoft.Extensions.*` package references move from the 10.x line to the matching stable 11.x release, with one version per package family across all projects. Other packages are updated only where a .NET 11-compatible version is needed.
- CI workflows (`.github/workflows/main.yml`, plus `.github/workflows/sonar.yml` if it still exists) install the .NET 11 SDK. The library's multi-target build must still restore and pack.
- Repository guidance that states the target framework (`CLAUDE.md`, `.claude/skills/build-and-test/SKILL.md`, `.claude/agents/dotnet-critic.md`, `.claude/agents/security-critic.md`) is updated to describe the new targets.
- `PackageReleaseNotes` in the library project mentions .NET 11 support.

## Capabilities

### New Capabilities
- `target-frameworks`: which .NET target frameworks the published NuGet package and the test projects support, and how CI builds them.

### Modified Capabilities
- None. No existing spec under `openspec/specs/` covers target frameworks.

## Impact

- **Code**: the three `.csproj` files under `src/`. The public API of the library does not change.
- **Package consumers**: the NuGet package gains a `lib/net11.0` folder next to `lib/net10.0`. Consumers on .NET 10 keep working. This change is not breaking.
- **Dependencies**: `Microsoft.Extensions.Http.Resilience` and the test-only `Microsoft.Extensions.*` packages move to 11.x. The 11.x packages must also support `net10.0`; this is checked during implementation.
- **CI**: the build agents need the .NET 11 SDK. It can also build `net10.0` targets.
- **Related in-flight changes**: `update-outdated-dependencies` (it pins packages to the 10.x line) and `replace-sonarcloud-with-local-linting` (it may remove `sonar.yml`). Whichever change is applied second must be rebased on the first.

## Source issue

<!-- source-issue: nikneem/sessionize-api-client#53 -->
https://github.com/nikneem/sessionize-api-client/issues/53
