# Proposal

## Why

Issue #55 reports that the GitHub Actions workflows use outdated action versions. The workflows are not consistent. `main.yml` uses `actions/checkout@v4` in the `version` job and `actions/checkout@v6` in the `build` job. `create-release.yml` still uses `actions/checkout@v4` and `actions/create-release@v1`, which GitHub has archived and no longer maintains. `cleanup-artifacts.yml` uses `actions/github-script@v7`. Old majors run on deprecated Node runtimes and stop getting fixes. They also slow down the publish and release pipelines for the NuGet package.

## What Changes

- In every workflow under `.github/workflows/` (`main.yml`, `create-release.yml`, `cleanup-artifacts.yml`, `sonar.yml`), update every `uses:` reference to the latest stable major version of that action, checked when the change is applied. Known outdated references today:
  - `actions/checkout@v4` → current major (`v6` is already used elsewhere in the repo) in `main.yml` (job `version`) and `create-release.yml`.
  - `actions/github-script@v7` → current major in `cleanup-artifacts.yml`.
  - `gittools/actions/gitversion/setup` and `.../execute` `@v4.3.3` → latest release in `main.yml` and `create-release.yml`. Keep `versionSpec` compatible with the chosen action version.
  - Check `actions/setup-dotnet`, `actions/setup-java`, `actions/cache`, `actions/upload-artifact` and `actions/download-artifact` too, and bump them if a newer major exists.
- Replace the archived `actions/create-release@v1` in `create-release.yml` with a maintained way to create the release. Keep the same tag name (`v<semVer>`), release title, body, and non-draft and non-prerelease flags.
- Use the same version of an action everywhere it appears.
- No change to triggers, job structure, secrets, the build, test and pack commands, or the published package.

## Capabilities

### New Capabilities
- `ci-workflows`: rules for the repository's GitHub Actions workflows. Workflows use current, maintained action versions consistently, and the release workflow keeps its observable output (tag and release content).

### Modified Capabilities
<!-- None: openspec/specs/ has no existing capabilities. -->

## Impact

- **Files**: `.github/workflows/main.yml`, `.github/workflows/create-release.yml`, `.github/workflows/cleanup-artifacts.yml`, `.github/workflows/sonar.yml`.
- **Behaviour**: The NuGet publish, GitHub release, artifact cleanup and SonarCloud workflows should behave as before. The release step uses a different action but produces an equivalent release.
- **Library / consumers**: no impact.
- **Related in-flight changes**: `update-outdated-dependencies` also plans to bump action versions, `replace-sonarcloud-with-local-linting` deletes `sonar.yml`, and `migrate-to-dotnet-11` changes the `dotnet-version` in workflows. Rebase whichever change is applied later onto the one applied first. If `sonar.yml` has already been removed, skip it.

## Source issue

<!-- source-issue: nikneem/sessionize-api-client#55 -->
https://github.com/nikneem/sessionize-api-client/issues/55
