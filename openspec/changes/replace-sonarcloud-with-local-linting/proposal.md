# Proposal

## Why

SonarCloud currently checks style and unwanted C# patterns in a separate workflow (`.github/workflows/sonar.yml`), but its findings only show up after a push or pull request. The maintainer wants to retire SonarCloud and replace it with a linter that runs locally with the dotnet CLI, so the AI coding agent (and humans) can lint the code before opening a pull request (issue #52).

## What Changes

- Remove the SonarCloud workflow `.github/workflows/sonar.yml` (the SonarScanner, JDK set-up, dotnet-coverage run and `coverage-report` artifact).
- Remove the six SonarCloud badges (Bugs, Code Smells, Security Rating, Technical Debt, Maintainability Rating, Vulnerabilities) from `README.md`. Keep the other badges.
- Add a repository `.editorconfig` that encodes the C# conventions the existing code already follows (file-scoped namespaces, `_camelCase` private fields, `I`-prefixed interfaces, `Async` suffix, sorted usings with `System` first, and so on), with severities so that violations are reported.
- Turn on the built-in .NET analyzers and code-style enforcement for every project with a new `src/Directory.Build.props` (`EnableNETAnalyzers`, `AnalysisLevel`, `EnforceCodeStyleInBuild`). This adds no package dependency to the published library.
- Make one documented local lint command, `dotnet format src --verify-no-changes`, that exits non-zero when the code violates the rules. Document it in `.claude/skills/build-and-test/SKILL.md`, `CLAUDE.md` and `CONTRIBUTING.md` as a step to run before opening a pull request.
- Run the same lint command in the `build` job of `.github/workflows/main.yml`, so CI checks the same rules developers run locally.
- Fix any lint findings in the existing code so the lint command passes when the change is merged. Public signatures of the published package do not change.

## Capabilities

### New Capabilities
- `code-quality-checks`: how the repository checks C# code style and quality: one local lint command based on `.editorconfig` and the .NET SDK analyzers, the same check in CI, and no dependency on SonarCloud.

### Modified Capabilities
<!-- None: openspec/specs/ has no existing capabilities. -->

## Impact

- Workflows: `.github/workflows/sonar.yml` is deleted; `.github/workflows/main.yml` gets a lint step. The `SONAR_TOKEN` secret and the SonarCloud project `nikneem_sessionize-api-client` are no longer used. The maintainer can remove them outside this repository.
- Coverage: CI no longer collects coverage or uploads the `coverage-report` artifact, because only the SonarCloud workflow did that. Local coverage (`dotnet test src --collect:"XPlat Code Coverage"`) still works.
- New files: `.editorconfig`, `src/Directory.Build.props`.
- Docs: `README.md` (badges), `CLAUDE.md`, `CONTRIBUTING.md`, `.claude/skills/build-and-test/SKILL.md`, and `.claude/agents/code-style-critic.md` (which currently says there is no `.editorconfig`).
- Source: small style fixes in `src/` where the existing code does not match the rules. No public API or package dependency change.

## Source issue

<!-- source-issue: nikneem/sessionize-api-client#52 -->
https://github.com/nikneem/sessionize-api-client/issues/52
