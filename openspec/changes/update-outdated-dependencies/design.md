# Design

## Context

See proposal.md (Why) for the motivation. Current state, from reading the repo:

- There are three projects under `src/`, all targeting `net10.0`. There is no `Directory.Build.props`, no `Directory.Packages.props` and no `global.json`. Each `.csproj` sets its versions inline.
- The library has a single direct dependency: `Microsoft.Extensions.Http.Resilience` 10.5.0.
- The test projects reference `Microsoft.Extensions.*` 10.0.7, `Microsoft.NET.Test.Sdk` 18.5.1, `Moq` 4.20.72, `Moq.Contrib.HttpClient` 1.4.0, `xunit` 2.9.3, `xunit.runner.visualstudio` 3.1.5 and `coverlet.collector` 10.0.0.
- CI (`.github/workflows/main.yml`) runs `dotnet build src`, `dotnet test src` and `dotnet pack` on .NET SDK 10.0. Dependabot is configured for nuget and github-actions on a weekly schedule.
- Workflow action versions don't match. `main.yml` uses `actions/checkout@v4` in one job and `@v6` in another, and `create-release.yml` uses `@v4`. `cleanup-artifacts.yml` uses `actions/github-script@v7`, and `create-release.yml` uses the archived `actions/create-release@v1`.
- The issue doesn't name any specific package or CVE. Which packages are actually vulnerable has to be found with tooling when the change is applied.

## Goals / Non-Goals

**Goals:**
- Meet the requirements in `specs/dependency-security/spec.md`. Every direct package is current, no high or critical advisories remain, and the build enforces this.
- Configure the audit settings in one place so that new projects pick them up automatically.

**Non-Goals:**
- Moving to Central Package Management (`Directory.Packages.props`). It would help with version consistency, but it restructures every csproj and isn't needed to fix the issue.
- Upgrading to xUnit v3, changing the mocking library, or changing the target framework.
- Replacing `actions/create-release@v1` with another action. It is archived but still works, and replacing it changes how releases behave. Note it as a follow-up.

## Decisions

1. **Enforce the audit with NuGet's built-in restore audit, not an extra CI step.**
   Add a `src/Directory.Build.props` that sets `NuGetAudit=true`, `NuGetAuditMode=all` (include transitive packages) and `NuGetAuditLevel=low` (report everything). Add `WarningsAsErrors` for `NU1903;NU1904` (high and critical). Low and moderate advisories (NU1901/NU1902) stay as warnings.
   *Alternative:* a CI step running `dotnet list package --vulnerable --include-transitive` and failing on its output. Rejected because it only runs in CI and parsing text output is fragile. The restore audit fails locally and in CI the same way.
   *Assumption:* the issue says "contain vulnerabilities" but gives no severity threshold. Failing only on high and critical keeps the build from breaking on low-impact advisories in test-only dependencies.

2. **Update to the latest stable version, including major versions, within `net10.0` compatibility.**
   Use `dotnet list src package --outdated` to find target versions. Keep the `Microsoft.Extensions.*` 10.0.x packages on one aligned patch version. `Microsoft.Extensions.Http.Resilience` uses its own 10.x line, so take its latest stable release. Exclude prereleases.
   *Assumption:* the author wants everything that is outdated updated, not only the vulnerable packages. If a major bump of a test-only package (for example Moq or coverlet) breaks the tests, keep the latest version that works and record the exception here when the change is applied.

3. **Fix vulnerable transitive packages by upgrading the direct parent first. Pin them explicitly only as a last resort.**
   If upgrading the direct reference doesn't pull in a patched transitive version, add an explicit `PackageReference` for the patched version to the affected project, with a comment that names the advisory.

4. **Bump GitHub Actions to their current major versions.**
   Make `actions/checkout` the same version everywhere (the newest one already in use, `@v6`, or newer if available). Also update `gittools/actions`, `actions/github-script` and any other action that is behind its latest major version. Leave `actions/create-release@v1` as it is (see Non-Goals).
   *Assumption:* "libraries" in the issue mainly means NuGet packages. The action bumps are included because they are low-risk and part of the same supply chain, but they are kept to version changes only.

## Risks / Trade-offs

- [A new advisory published later breaks a build with no code change] → This is intended, because the spec requires it. Maintainers can fix it by upgrading the package. A temporary `NuGetAuditSuppress` entry with a justification is allowed as an escape hatch.
- [A major version bump changes behaviour in the resilience pipeline] → The unit tests in `Sessionize.Api.Client.Tests` cover the client. Run them, and check the release notes of `Microsoft.Extensions.Http.Resilience` before merging.
- [Integration tests need the `SESSIONIZE_API_ID` secret and can't run locally without it] → Run the unit tests locally and rely on CI for the integration tests.
- [Raising the library's minimum dependency versions affects consumers] → Consumers already on .NET 10 can take the 10.x packages without trouble. Mention the bump in `PackageReleaseNotes`.

## Migration Plan

This ships as a normal release through `main.yml`. To roll back, revert the commit. There are no data or API migrations.
