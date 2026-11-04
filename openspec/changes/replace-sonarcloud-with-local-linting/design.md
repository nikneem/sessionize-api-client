# Design

## Context

- CI has two workflows that build and test the code. `main.yml` (Ubuntu) builds, tests, packs and publishes on every push. `sonar.yml` (Windows) runs on pushes to `main` and on pull requests: it installs a JDK and `dotnet-sonarscanner`, builds, collects coverage with `dotnet-coverage` and uploads the results to SonarCloud project `nikneem_sessionize-api-client` with the `SONAR_TOKEN` secret.
- `README.md` lines 17–22 contain six SonarCloud badges. No other file refers to SonarCloud.
- The repository has no `.editorconfig` and no `Directory.Build.props`. `.claude/agents/code-style-critic.md` lists the conventions taken from the existing code (file-scoped namespaces, `_camelCase` private fields, PascalCase static readonly fields, `I` prefix for interfaces, `Async` suffix, sorted usings with `System` first, no usings that `ImplicitUsings` already covers) and says there is no `.editorconfig`.
- All projects target `net10.0`. The .NET 10 SDK includes the Roslyn .NET analyzers, IDE code-style analyzers and `dotnet format`, so no extra package or tool is needed.
- The library is published to NuGet and must not get new package dependencies or public API changes (CLAUDE.md).

See proposal.md for motivation.

## Goals / Non-Goals

**Goals:**
- One lint command that runs offline with only the .NET SDK and gives the same result locally and in CI.
- Rules that encode the conventions already in use, so the change mostly adds enforcement and needs few code edits.

**Non-Goals:**
- Replacing SonarCloud's hosted dashboards, coverage tracking or quality gates.
- Adding third-party analyzer packages (StyleCop.Analyzers, SonarAnalyzer.CSharp, Roslynator).
- Changing the build to treat all compiler warnings as errors.
- Removing the `SONAR_TOKEN` secret or the SonarCloud project. These are outside the repository and are left to the maintainer.

## Decisions

### Linter: `.editorconfig` + SDK analyzers, checked with `dotnet format`
The lint command is `dotnet format src --verify-no-changes`. It applies the whitespace, IDE code-style and analyzer rules from `.editorconfig` and fails if any file would change or a diagnostic at warning severity or higher remains.

- *Alternative: StyleCop.Analyzers or SonarAnalyzer.CSharp packages.* These have more rules, but they add package references (which must be marked private to keep them out of the NuGet package) and need updates. They also add many rules that would require a large clean-up of the existing code. The issue only asks for a dev-time linter, so the SDK's built-in rules are enough.
- *Alternative: `dotnet build -warnaserror` only.* IDE style rules are not reported at build time unless `EnforceCodeStyleInBuild` is set, and the build does not check formatting. `dotnet format` covers both and can also fix issues (`dotnet format src`), which is useful for agents.

### `src/Directory.Build.props` enables analyzers for all projects
Set `EnableNETAnalyzers=true`, `AnalysisLevel=latest-recommended` and `EnforceCodeStyleInBuild=true` once for all three projects. Then editors and `dotnet build` show the same diagnostics as the lint command. The file goes in `src/` next to the solution, because all projects are there.

- *Alternative: set the properties in each `.csproj`.* This repeats the settings in three files, and a new project could miss them.

### `.editorconfig` at repository root
Put it at the root so it also covers future files outside `src/`. It contains:
- General formatting: 4-space indentation for C#, final newline, trimmed trailing whitespace, and 2 spaces for YAML/JSON/XML project files.
- C# style rules from the code-style critic, set to `warning`: `csharp_style_namespace_declarations = file_scoped`, `dotnet_sort_system_directives_first = true`, IDE0005 (unnecessary usings; this requires `GenerateDocumentationFile` or is otherwise only reported in the IDE, see Risks), and naming rules for interfaces (`I` prefix), private fields (`_camelCase`), static readonly fields (PascalCase), async methods (`Async` suffix) and types/members (PascalCase).
- Before choosing a severity for each rule, check it against the existing code. A rule that the current code breaks in many places for good reason is set to `suggestion` instead of being enforced, and the design or tasks record why.

### CI: add the lint step to `main.yml`
Add `dotnet format src --verify-no-changes` to the `build` job after `setup-dotnet` and before `dotnet build`, so a lint failure stops the build early. The `build` job runs on every push, including pull request branches, so it covers both cases `sonar.yml` covered. No separate lint workflow is added.

### Delete `sonar.yml` instead of disabling it
The workflow is deleted. Git history keeps it if it is ever needed again.

## Risks / Trade-offs

- [CI no longer collects coverage or uploads the `coverage-report` artifact] → Accepted: the issue asks to remove the SonarCloud workflow, which is the only place this happened. Local coverage with coverlet stays documented in the build-and-test skill. A coverage step can be added to `main.yml` later if wanted.
- [Fewer rules than SonarCloud, for example cognitive complexity or duplicate code] → Accepted per the issue: the AI review agents (`.claude/agents/*-critic.md`) cover design-level review. `AnalysisLevel=latest-recommended` adds the SDK's recommended CA quality rules.
- [Existing code may violate new rules, so the first lint run fails] → The implementation runs `dotnet format src` to fix problems that can be fixed automatically, fixes the rest by hand, and never changes public signatures. If keeping a public name requires it, use a targeted suppression with a justification.
- [IDE0005 (unnecessary usings) is only reported on the command line when documentation file generation is on] → Turn on `GenerateDocumentationFile` in `Directory.Build.props` and turn off the missing-XML-comment warning `CS1591`, or, if that causes too many changes, leave IDE0005 at `suggestion`. Decide while implementing. Either option meets the spec.
- [The lint result could differ between SDK versions] → CI and local runs both use the .NET 10 SDK (`dotnet-version: "10.0"`). Pinning the SDK with `global.json` is out of scope.

## Migration Plan

1. Merge the change. After merge, no workflow uses `SONAR_TOKEN`.
2. Optionally, the maintainer removes the `SONAR_TOKEN` repository secret and archives or deletes the SonarCloud project, and removes any branch protection rule that requires the SonarCloud check.
3. Rollback: revert the commit. This restores `sonar.yml` and the badges. The secret still works if it was not removed.

## Assumptions

The triage ran unattended, so these points were decided without asking the issue author:

- **Linter choice.** The issue only asks for "a linter". We assume the built-in .NET SDK analyzers with `.editorconfig` and `dotnet format`, because they need no new dependency and run offline.
- **CI enforcement.** The issue asks for a dev-time linter. We assume the same check should also run in CI, so pull requests that skip the local run are still caught and SonarCloud's PR gate is replaced. If the author wants a local-only linter, drop the CI task. The specs' "CI runs the same lint check" requirement would then be removed.
- **Coverage.** We assume losing CI coverage collection along with the SonarCloud workflow is acceptable, because the issue asks to remove the workflow and does not mention coverage.
- **README "labels".** We read "labels in the readme file" as the six SonarCloud badges. The other badges stay.
- **External clean-up.** Removing the `SONAR_TOKEN` secret and the SonarCloud project is left to the maintainer, because it cannot be done from the repository.
