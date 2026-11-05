# Tasks

## 1. Determine target versions

- [ ] 1.1 For each action used in `.github/workflows/*` (actions/checkout, setup-dotnet, setup-java, cache, upload-artifact, download-artifact, github-script, gittools/actions/gitversion), record the latest stable release and any breaking changes since the current ref (for example with `gh release list -R <owner>/<repo>`). Verify that a version table exists in the PR description.

## 2. Update action references

- [ ] 2.1 In `.github/workflows/main.yml`, bump `actions/checkout` in the `version` job to the same version as the `build` job (at least v6), and bump all other actions to the versions from 1.1. Verify that `grep "uses:" .github/workflows/main.yml` shows only target versions.
- [ ] 2.2 In `main.yml` and `create-release.yml`, bump `gittools/actions/gitversion/setup` and `execute` to the latest release, and adjust `versionSpec` if the new action requires it. Verify that every `steps.gitversion.outputs.*` name used is still documented by the action.
- [ ] 2.3 In `.github/workflows/cleanup-artifacts.yml`, bump `actions/github-script` to the latest major and adjust the script if the bundled Octokit API changed. Verify that the `github.rest.actions.listArtifactsForRepo` and `deleteArtifact` calls are still valid for that version.
- [ ] 2.4 In `.github/workflows/sonar.yml` (skip if it has already been removed by `replace-sonarcloud-with-local-linting`), bump checkout, setup-dotnet, setup-java, cache and upload-artifact to the target versions. Verify with `grep "uses:"`.
- [ ] 2.5 Bump `upload-artifact` and `download-artifact` to compatible majors. Verify that the `nuget-package` artifact name and path are unchanged.

## 3. Replace archived release action

- [ ] 3.1 In `.github/workflows/create-release.yml`, bump `actions/checkout`, then replace the `actions/create-release@v1` step with a `gh release create "v<semVer>"` run step (`GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}`, `--title "Release v<semVer>"`, `--notes` with the existing body, `--target ${{ github.sha }}`, no draft or prerelease flags). Verify that `actions/create-release` no longer appears anywhere in `.github/`.

## 4. Verification

- [ ] 4.1 Run `actionlint` (or an equivalent YAML/workflow linter) on `.github/workflows/`. Verify that it reports no errors.
- [ ] 4.2 Verify that each action is used at a single version across all workflows (compare the `grep -h "uses:" .github/workflows/*.yml` output).
- [ ] 4.3 Push the branch and verify that the `Publish NuGet Package` workflow (version and build jobs) and, for PRs, `SonarCloud` pass. After merging to `main`, verify that the deploy job succeeds and the release is created with the tag `v<semVer>`, the expected title and body, and not as a draft or prerelease.
