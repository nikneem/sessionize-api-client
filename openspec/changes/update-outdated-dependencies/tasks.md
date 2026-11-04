# Tasks

## 1. Baseline

- [ ] 1.1 Run `dotnet list src package --outdated` and `dotnet list src package --vulnerable --include-transitive`. Record the outdated and vulnerable packages, with advisory IDs and severities, in the PR description. Verify by checking that both reports appear in the PR description.

## 2. Vulnerability audit gate

- [ ] 2.1 Add `src/Directory.Build.props` with `NuGetAudit=true`, `NuGetAuditMode=all`, `NuGetAuditLevel=low` and `WarningsAsErrors` for `NU1903;NU1904` (design decision 1). Verify that `dotnet restore src` reports any existing high or critical advisories as errors and any low or moderate ones as warnings.

## 3. NuGet package updates

- [ ] 3.1 Update `Microsoft.Extensions.Http.Resilience` in `src/Sessionize.Api.Client/Sessionize.Api.Client.csproj` to its latest stable version. Verify that `dotnet build src/Sessionize.Api.Client -c Release` succeeds.
- [ ] 3.2 Update every package in `src/Sessionize.Api.Client.Tests/Sessionize.Api.Client.Tests.csproj` to its latest stable `net10.0`-compatible version, keeping `Microsoft.Extensions.*` on one aligned version. Verify that `dotnet test src/Sessionize.Api.Client.Tests` passes.
- [ ] 3.3 Update every package in `src/Sessionize.Api.Client.IntegrationTests/Sessionize.Api.Client.IntegrationTests.csproj` to the same versions used in 3.2 for shared packages, and to the latest stable version for the rest. Verify that `dotnet build src/Sessionize.Api.Client.IntegrationTests` succeeds and that the shared packages have the same versions in both test projects.
- [ ] 3.4 For any high or critical transitive advisory that remains, pin the patched version explicitly in the affected project, with a comment naming the advisory (design decision 3). Verify that `dotnet list src package --vulnerable --include-transitive` reports no high or critical vulnerabilities.
- [ ] 3.5 Where a package can't be moved to its latest version without breaking the build or tests, record the package, the version kept and the reason in design.md (Decisions). Verify that `dotnet list src package --outdated` shows only the documented exceptions.
- [ ] 3.6 Update `PackageReleaseNotes` in `Sessionize.Api.Client.csproj` to mention the dependency and security update. Verify that `dotnet pack src/Sessionize.Api.Client -c Release` succeeds and that the nuspec contains the new release notes and dependency versions.

## 4. GitHub Actions updates

- [ ] 4.1 Make `actions/checkout` the same current major version in `main.yml` and `create-release.yml`. Bump `gittools/actions`, `actions/github-script` and any other outdated actions in `.github/workflows/*` to their latest major versions, leaving `actions/create-release@v1` unchanged. Verify that the workflow files still parse (for example with `actionlint`) and that the `Publish NuGet Package` workflow passes on the PR branch.

## 5. Integration check

- [ ] 5.1 Run `dotnet build src -c Release` and `dotnet test src` from a clean clone. Verify that both succeed with no NU1903 or NU1904 errors, and that the library's public API is unchanged: no source changes in `src/Sessionize.Api.Client` other than the csproj.

## Workflow follow-up

- Consider replacing the archived `actions/create-release@v1` in a separate change.
- Archive the change once the PR is merged.
