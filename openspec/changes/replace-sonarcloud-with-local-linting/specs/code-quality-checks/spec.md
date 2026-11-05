# Spec Delta

## Purpose

Defines how the repository checks C# code style and quality: a lint check that developers and AI coding agents can run locally before opening a pull request, and that CI runs the same way, with no external code-analysis service.

## ADDED Requirements

### Requirement: Local lint command
The repository SHALL provide one lint command, run with the .NET SDK from the repository root and needing no network service or account, that checks all C# code under `src/` against the repository's style and code-quality rules. It SHALL exit with a non-zero code when a rule is violated and zero otherwise.

#### Scenario: Clean code passes
- **WHEN** a developer runs the lint command on code that follows all rules
- **THEN** the command exits with code zero

#### Scenario: Style violation fails
- **WHEN** a developer runs the lint command on code with a rule violation, such as a block-scoped namespace or a private field not named `_camelCase`
- **THEN** the command exits with a non-zero code and reports the file, line and rule of the violation

#### Scenario: Runs offline
- **WHEN** the lint command runs on a machine with the .NET SDK and restored packages but no access to an external analysis service
- **THEN** the command completes and reports results

### Requirement: Rules are defined in the repository
The style and code-quality rules used by the lint command SHALL be defined in files committed to the repository and SHALL match the conventions of the existing code, so that the code on the default branch passes the lint command.

#### Scenario: Default branch is lint-clean
- **WHEN** the lint command runs on the default branch after this change is merged
- **THEN** it exits with code zero

#### Scenario: Editors use the same rules
- **WHEN** a developer opens the solution in an editor that supports the committed rule files
- **THEN** the editor reports the same violations as the lint command

### Requirement: Lint command is documented for contributors and agents
The lint command SHALL be documented in the contributor guide and the AI agent instructions as a step to run, and pass, before opening a pull request.

#### Scenario: Agent finds the command
- **WHEN** an AI coding agent reads the repository's agent instructions or build-and-test guidance
- **THEN** it finds the exact lint command and the instruction to run it before opening a pull request

#### Scenario: Contributor finds the command
- **WHEN** a contributor reads `CONTRIBUTING.md`
- **THEN** it lists running the lint command among the steps before opening a pull request

### Requirement: CI runs the same lint check
The CI build that runs on every push SHALL run the same lint command developers run locally, and SHALL fail when the lint command fails.

#### Scenario: Lint violation fails CI
- **WHEN** a push contains code that violates a lint rule
- **THEN** the CI build fails at the lint step

#### Scenario: Clean push passes lint step
- **WHEN** a push contains code that passes the lint command locally
- **THEN** the CI lint step passes

### Requirement: No dependency on SonarCloud
The repository SHALL NOT run SonarCloud analysis in any workflow, and the README SHALL NOT show SonarCloud badges or links.

#### Scenario: No SonarCloud workflow
- **WHEN** a pull request is opened or a commit is pushed
- **THEN** no workflow runs a SonarCloud scanner or needs a `SONAR_TOKEN` secret

#### Scenario: README has no SonarCloud badges
- **WHEN** a reader views `README.md`
- **THEN** it shows no badge or link to `sonarcloud.io`, and the other badges (commit activity, NuGet downloads, license, last commit, release, publish workflow) are still shown
