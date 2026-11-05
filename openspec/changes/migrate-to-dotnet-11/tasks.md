# Tasks

## 1. Release gate and preconditions

- [ ] 1.1 Confirm that .NET 11 is generally available (on or after 11 November 2026) and that `dotnet --list-sdks` shows a non-preview 11.0 SDK. Do not continue on a preview SDK.
- [ ] 1.2 Look up the latest stable 11.x versions of `Microsoft.Extensions.Http.Resilience`, `Microsoft.Extensions.Http`, `Microsoft.Extensions.Logging.Abstractions`, `Microsoft.Extensions.Options`, `Microsoft.Extensions.Configuration`, `Microsoft.Extensions.Configuration.Json` and `Microsoft.Extensions.DependencyInjection.Abstractions` on nuget.org. For each one, confirm that it supports `net10.0` as well as `net11.0`. Record the versions, and any package that needs a conditional reference, in design.md.
- [ ] 1.3 Check whether `update-outdated-dependencies` or `replace-sonarcloud-with-local-linting` has already been applied. If so, rebase on the current `.csproj` and workflow files and verify that `git status` shows no conflicts.

## 2. Library

- [ ] 2.1 In `src/Sessionize.Api.Client/Sessionize.Api.Client.csproj`, replace `<TargetFramework>net10.0</TargetFramework>` with `<TargetFrameworks>net10.0;net11.0</TargetFrameworks>`. Move `Microsoft.Extensions.Http.Resilience` to the 11.x version from 1.2. Verify that `dotnet build src/Sessionize.Api.Client -c Release` succeeds for both TFMs with no new warnings.
- [ ] 2.2 Update `PackageReleaseNotes` to mention the added .NET 11 target and the move to Microsoft.Extensions 11.x. Verify by reading the `.nuspec` inside the packed output.
- [ ] 2.3 Run `dotnet pack src/Sessionize.Api.Client/Sessionize.Api.Client.csproj -c Release` and verify that the `.nupkg` contains both `lib/net10.0/Sessionize.Api.Client.dll` and `lib/net11.0/Sessionize.Api.Client.dll`, with dependency groups for both TFMs.
- [ ] 2.4 Compare the public API of the `net10.0` and `net11.0` assemblies with the previous release, for example with Microsoft.DotNet.ApiCompat or by checking that the unit tests compile unchanged. Verify that no public type or member was added, removed or changed.

## 3. Test projects

- [ ] 3.1 Retarget `src/Sessionize.Api.Client.Tests/Sessionize.Api.Client.Tests.csproj` to `net11.0` and move its `Microsoft.Extensions.*` references to the aligned 11.x version. Verify that `dotnet test src/Sessionize.Api.Client.Tests` passes on the .NET 11 runtime.
- [ ] 3.2 Retarget `src/Sessionize.Api.Client.IntegrationTests/Sessionize.Api.Client.IntegrationTests.csproj` to `net11.0` and move its `Microsoft.Extensions.*` references to the same 11.x version. Verify that `dotnet build src/Sessionize.Api.Client.IntegrationTests` succeeds, and that `dotnet test` passes when `appsettings-public-api.json` is available.
- [ ] 3.3 If xunit, Moq, coverlet or Microsoft.NET.Test.Sdk fail on .NET 11, update only the affected package and record why in design.md. Verify by running `dotnet test src` again.

## 4. CI

- [ ] 4.1 In `.github/workflows/main.yml`, set the `actions/setup-dotnet` `dotnet-version` to `"11.0"`. Verify that the workflow run on the branch builds both library TFMs, passes the tests and uploads the package artifact.
- [ ] 4.2 If `.github/workflows/sonar.yml` or any workflow added by `replace-sonarcloud-with-local-linting` exists, set its `dotnet-version` to `"11.0"`. Verify that `git grep "dotnet-version"` shows no remaining `"10.0"` entries and that the workflow passes.

## 5. Repository guidance

- [ ] 5.1 Update `CLAUDE.md` to say the library targets `net10.0` and `net11.0`, the test projects target `net11.0`, and Microsoft.Extensions packages stay on the 11.x line. Update the listed package versions too. Verify that `git grep -n "net10.0\|\.NET 10"` returns only intended references to the library's `net10.0` target.
- [ ] 5.2 Update `.claude/skills/build-and-test/SKILL.md` (the .NET 11 SDK is required), `.claude/agents/dotnet-critic.md` and `.claude/agents/security-critic.md` to match. Verify with the same `git grep`.

## 6. Integration check

- [ ] 6.1 From a clean clone, run `dotnet build src`, `dotnet test src` and `dotnet pack` with only the .NET 11 SDK installed. Verify that all three succeed and that the package contents match task 2.3.
