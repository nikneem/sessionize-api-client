# Spec Delta

## Purpose

Defines the rules for the repository's GitHub Actions workflows. They use maintained, up-to-date actions in a consistent way, and they keep producing the same build, package and release outputs.

## ADDED Requirements

### Requirement: Workflows use current action major versions
Every `uses:` reference in `.github/workflows/` SHALL point to the latest stable major version (or latest release, for actions that are only versioned by release) that the action publishes when the change is applied.

#### Scenario: Outdated checkout reference
- **WHEN** a workflow step references `actions/checkout@v4` while a newer stable major is available
- **THEN** the reference is updated to the newest stable major

#### Scenario: Action already current
- **WHEN** a workflow step already references the newest stable major of an action
- **THEN** the reference is left unchanged

### Requirement: Action versions are consistent across workflows
The same action SHALL be referenced at the same version in every workflow and job where it is used.

#### Scenario: Same action in several jobs
- **WHEN** `actions/checkout` is used by more than one job or workflow
- **THEN** all of those steps reference the same version

### Requirement: Workflows do not use archived or unmaintained actions
Workflows SHALL NOT reference actions whose repositories are archived or that their publisher has marked as deprecated or unmaintained.

#### Scenario: Archived release action
- **WHEN** the release workflow creates a GitHub release
- **THEN** it does so without using `actions/create-release`

### Requirement: Release workflow output is preserved
On a push to `main`, the release workflow SHALL create a published (non-draft, non-prerelease) GitHub release tagged `v<semVer>` and titled `Release v<semVer>`. The body SHALL contain the same version information and changelog link as before.

#### Scenario: Push to main creates release
- **WHEN** a commit is pushed to `main` and GitVersion computes semVer `1.2.3`
- **THEN** a GitHub release with tag `v1.2.3` and title `Release v1.2.3` is published, not as a draft or prerelease, and its body lists the semantic version, full semVer, informational version, branch, commit SHA and the changelog link

### Requirement: Build and publish behaviour is preserved
Updating action versions SHALL NOT change workflow triggers, job dependencies, required secrets, or the produced artifacts (NuGet package artifact, coverage report) and their names.

#### Scenario: NuGet publish after update
- **WHEN** the publish workflow runs on `main` after the update
- **THEN** it computes the version, builds, tests, packs, uploads the `nuget-package` artifact and pushes the package to NuGet as before
