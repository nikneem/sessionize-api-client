# Design

## Context

- See proposal.md (Why) for the motivation. The .NET 11 GA release is expected on 11 November 2026. Implementation must not start before then.
- Current state: the three projects under `src/` each set `<TargetFramework>net10.0</TargetFramework>` inline. There is no `Directory.Build.props` and no `global.json`.
- The library's only package dependency is `Microsoft.Extensions.Http.Resilience` 10.5.0. The test projects reference `Microsoft.Extensions.*` 10.0.7 plus xunit, Moq, coverlet and Microsoft.NET.Test.Sdk.
- `.github/workflows/main.yml` and `.github/workflows/sonar.yml` both use `actions/setup-dotnet@v5` with `dotnet-version: "10.0"`.
- `CLAUDE.md` says "do not break public signatures of the published package" and "keep Microsoft.Extensions packages on the 10.x line". This change updates the second rule.

## Goals / Non-Goals

**Goals:**
- Build and ship the library for .NET 11 without dropping .NET 10 consumers.
- Run the tests and CI on .NET 11.

**Non-Goals:**
- Adopting new .NET 11 or C# language features, or changing the public API.
- Adding a `global.json`, `Directory.Build.props` or central package management.
- Dropping `net10.0`. That can be a later change once .NET 10 support ends (November 2028).
- Upgrading test frameworks across major versions (for example xunit v3) unless .NET 11 requires it.

## Decisions

1. **The library multi-targets `net10.0;net11.0`; test projects single-target `net11.0`.**
   *Assumption (the issue only says "the projects must be updated to .NET 11"):* .NET 10 is an LTS release and .NET 11 is a standard-term release. Replacing `net10.0` with `net11.0` in the published package would stop .NET 10 consumers from installing new versions. That is a breaking change, and the repository rules say not to break consumers of the package. Multi-targeting gives the "updated to .NET 11" result and keeps compatibility.
   *Alternative considered:* retarget the library to `net11.0` only. This is simpler, but it breaks every consumer on .NET 10 LTS. Rejected. If the maintainer prefers it, the change is one line in the `.csproj` and a spec update.
   *Test projects:* only one TFM is needed. Running the tests on `net11.0` covers the new target. The `net10.0` build is still compiled in CI, so compile errors on that target fail the build.

2. **Move `Microsoft.Extensions.*` packages to the matching stable 11.x release, one version for both TFMs.**
   Microsoft.Extensions packages usually ship assets for the current and earlier supported TFMs, so a single 11.x version is expected to support `net10.0` too. This keeps the `.csproj` simple: no per-TFM conditional `PackageReference`. During implementation, check the 11.x package's supported frameworks on nuget.org. If an 11.x package does not support `net10.0`, fall back to a conditional reference (10.x for `net10.0`, 11.x for `net11.0`) and record that in this file.
   *Alternative considered:* keep 10.x for both TFMs. This works, but it leaves .NET 11 consumers on older dependencies and goes against the issue's intent.

3. **Other packages (xunit, Moq, coverlet, Test.Sdk) stay on their current versions unless they fail on .NET 11.**
   This keeps the change focused. General dependency freshness is the job of the `update-outdated-dependencies` change.

4. **CI installs only the .NET 11 SDK.**
   The .NET 11 SDK can build `net10.0` targets because the targeting packs are restored from NuGet. Tests run on `net11.0` only, so the .NET 10 runtime is not needed. Set `dotnet-version: "11.0"` in `main.yml`, and in `sonar.yml` if that file still exists when this change is applied.

5. **Update repository guidance in the same change.**
   `CLAUDE.md`, `.claude/skills/build-and-test/SKILL.md` and the `dotnet-critic` and `security-critic` agents say `net10.0` / ".NET 10". Update them so later agents and contributors do not undo the change.

## Risks / Trade-offs

- [.NET 11 GA slips or packages are not yet on nuget.org] → The first task is a gate: confirm the GA SDK and stable 11.x packages exist. Until then, do not use previews.
- [New analyzer warnings or behavior changes in the .NET 11 SDK] → Build with the SDK and fix any new warnings in the library. Check the .NET 11 breaking-changes list for `System.Net.Http`, `System.Text.Json` and `Microsoft.Extensions.Http.Resilience`.
- [Multi-targeting makes builds slower and the package larger] → Only one small assembly is added. This is acceptable.
- [Conflict with in-flight changes] → `update-outdated-dependencies` pins to 10.x and `replace-sonarcloud-with-local-linting` may delete `sonar.yml` or add workflows with `dotnet-version: "10.0"`. Whichever is applied later must update the versions and SDK references to match. Task 1 checks this.
- [Raising the minimum `Microsoft.Extensions.*` version to 11.x for `net10.0` consumers] → Consumers on .NET 10 that pin 10.x Extensions packages get a version bump through the transitive dependency. Mention this in `PackageReleaseNotes`. If it becomes a problem, use the conditional reference from Decision 2.

## Migration Plan

1. After GA, apply the change on `development`, check that CI is green, then merge to `main`. This publishes a new package version through the existing workflow.
2. GitVersion picks the version. Multi-targeting is not breaking, so a minor version bump is appropriate.
3. Rollback: revert the commit. Consumers who already installed the new version can pin the previous one.

## Open Questions

- The exact stable 11.x version numbers of the `Microsoft.Extensions.*` packages. These will be known at GA and do not change the approach.
