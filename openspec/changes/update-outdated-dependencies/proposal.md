# Proposal

## Why

Issue #50 reports that the library's dependencies are outdated and may contain known vulnerabilities. Sessionize.Api.Client is published to NuGet, so a vulnerable direct or transitive dependency is passed on to every consumer. Nothing in the build currently stops a vulnerable package from being shipped. Dependabot only opens PRs, and those PRs can sit unmerged.

## What Changes

- Update all NuGet `PackageReference`s in `src/Sessionize.Api.Client`, `src/Sessionize.Api.Client.Tests` and `src/Sessionize.Api.Client.IntegrationTests` to their latest stable versions that are compatible with `net10.0`. Shared package families must use the same version in every project.
- Resolve any known vulnerability in direct or transitive packages that NuGet reports. Upgrade the direct reference, or pin a patched transitive version explicitly when no direct upgrade fixes it.
- Turn on NuGet's restore-time vulnerability audit for all projects, including transitive packages. Treat high and critical advisories as build errors so CI (`dotnet build src` in `main.yml`) fails if a vulnerable package is introduced.
- Bring outdated GitHub Actions in `.github/workflows/*` up to their current major versions. For example, `actions/checkout@v4` sits next to `@v6` in the same workflow. This keeps the CI supply chain current as well.
- No public API changes to the client library are intended. Any upgrade that would force a public API or target framework change is out of scope and must be reported instead of applied.

## Capabilities

### New Capabilities
- `dependency-security`: Rules for keeping the library's third-party dependencies free of known vulnerabilities and for having the build enforce this.

### Modified Capabilities
<!-- None: no specs exist yet in openspec/specs/. -->

## Impact

- **Code**: The three `.csproj` files under `src/`. Possibly a new `src/Directory.Build.props` to hold the audit settings in one place.
- **CI**: `.github/workflows/main.yml`, `create-release.yml`, `cleanup-artifacts.yml` and `sonar.yml` (action version bumps). The build now fails when a high or critical vulnerability is found.
- **Consumers**: The published NuGet package gets newer minimum versions of `Microsoft.Extensions.Http.Resilience` and its transitive dependencies. There is no public API change.
- **Out of scope**: Changing the target framework, migrating to a different test framework (for example xUnit v3), or redesigning the Dependabot configuration.

## Source issue

<!-- source-issue: nikneem/sessionize-api-client#50 -->
https://github.com/nikneem/sessionize-api-client/issues/50
