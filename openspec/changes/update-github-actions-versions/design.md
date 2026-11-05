# Design

## Context

There are four workflows in `.github/workflows/`. Current references:

| Workflow | Action | Current ref |
|---|---|---|
| main.yml (version job) | actions/checkout | v4 |
| main.yml (version job) | gittools/actions/gitversion/setup, execute | v4.3.3 |
| main.yml (build job) | actions/checkout | v6 |
| main.yml (build job) | actions/setup-dotnet | v5 |
| main.yml (build job) | actions/upload-artifact | v6 |
| main.yml (deploy job) | actions/download-artifact | v7 |
| create-release.yml | actions/checkout | v4 |
| create-release.yml | gittools/actions/gitversion/setup, execute | v4.3.3 |
| create-release.yml | actions/create-release | v1 (archived) |
| cleanup-artifacts.yml | actions/github-script | v7 |
| sonar.yml | actions/checkout, setup-dotnet, setup-java, cache, upload-artifact | v6, v5, v5, v5, v6 |

`.github/dependabot.yml` already checks the `github-actions` ecosystem every week. It does not replace archived actions, and it has not closed the gaps above. See proposal.md for the motivation.

## Goals / Non-Goals

**Goals:**
- Every action reference is at its latest stable version when the change is applied, and versions are consistent across jobs.
- Remove the archived `actions/create-release`.

**Non-Goals:**
- Pinning actions to commit SHAs, changing triggers or permissions beyond what a replacement needs, or restructuring jobs.
- Changing the `dotnet-version` (owned by `migrate-to-dotnet-11`) or removing `sonar.yml` (owned by `replace-sonarcloud-with-local-linting`).
- Changing the Dependabot configuration.

## Decisions

1. **Look up the latest versions when the change is applied, not now.** Versions move quickly, so the implementer checks each action's releases page (or `gh release list -R <owner>/<repo>`) and uses the newest stable release. Known minimums: `actions/checkout` must be at least `v6`, which the repo already uses. `actions/github-script` has a newer major than `v7`.
2. **Keep the existing ref style.** `actions/*` stay on floating major tags (`@vN`). `gittools/actions` stays on a full version tag (`@vX.Y.Z`), as it is now. Changing the pinning strategy is out of scope.
3. **Replace `actions/create-release@v1` with the GitHub CLI (`gh release create`)** in a `run:` step, using `GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}`. The CLI is preinstalled on `ubuntu-latest` and maintained by GitHub, and it adds no third-party action to the supply chain. Alternative considered: `softprops/action-gh-release`. It is widely used, but it is a third-party dependency. Use `--title`, `--notes` (with the same body text) and `--target ${{ github.sha }}`. Do not pass `--draft` or `--prerelease`. The existing `contents: write` permission is enough.
4. **GitVersion `versionSpec`.** If the newest `gittools/actions` release requires a different GitVersion range, update `versionSpec` in both workflows to the range that the action documents as supported. Keep the step outputs used by later steps (`semVer`, `fullSemVer`, `informationalVersion`, `majorMinorPatch`, `assemblySemVer`, `branchName`, `sha`).
5. **Keep artifact actions paired.** `upload-artifact` and `download-artifact` must stay on compatible majors, so bump them together if either moves.

## Risks / Trade-offs

- [A new major changes inputs or defaults, for example the checkout credential handling or artifact behaviour] → Read each action's release notes for breaking changes before bumping, and adjust inputs.
- [The GitVersion action release renames outputs] → Check that all `steps.gitversion.outputs.*` references still resolve. The release body depends on them.
- [`gh release create` fails if the tag already exists, while `create-release@v1` also failed in that case] → Behaviour is the same, so no mitigation is needed.
- [The release workflow cannot be fully tested without pushing to `main`] → Run the workflow on a branch or fork with `workflow_dispatch` if possible, or check the YAML with `actionlint` and review the first run on `main`.
- [Conflicts with in-flight changes that touch the same files (`update-outdated-dependencies`, `replace-sonarcloud-with-local-linting`, `migrate-to-dotnet-11`)] → Rebase whichever is applied later. If `sonar.yml` no longer exists, skip it.

## Assumptions (made without asking the issue author)

- "Deployment workflows" means all workflows in `.github/workflows/`, not only `main.yml` and `create-release.yml`. Updating the others is low-risk and keeps versions consistent.
- "Most recent version" means the latest stable (non-prerelease) release.
- Replacing the archived `actions/create-release` falls within the issue's scope, because no newer version of that action exists.
