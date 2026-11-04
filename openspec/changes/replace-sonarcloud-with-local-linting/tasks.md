# Tasks

## 1. Remove SonarCloud

- [ ] 1.1 Delete `.github/workflows/sonar.yml` and verify that no file under `.github/` mentions `sonar` (case-insensitive search)
- [ ] 1.2 Remove the six SonarCloud badge lines (Bugs, Code Smells, Security Rating, Technical Debt, Maintainability Rating, Vulnerabilities) from `README.md`, keep the other six badges, and verify that searching the repository (excluding `openspec/`) for `sonarcloud` finds nothing

## 2. Configure lint rules

- [ ] 2.1 Add `src/Directory.Build.props` with `EnableNETAnalyzers=true`, `AnalysisLevel=latest-recommended` and `EnforceCodeStyleInBuild=true`, and verify `dotnet build src` still succeeds and `dotnet pack src/Sessionize.Api.Client/Sessionize.Api.Client.csproj -c Release` produces a package with only the `Microsoft.Extensions.Http.Resilience` dependency
- [ ] 2.2 Add a root `.editorconfig` with general formatting settings and the C# style and naming rules from design.md (file-scoped namespaces, `System` usings first, `I`-prefixed interfaces, `_camelCase` private fields, PascalCase static readonly fields, `Async` suffix, PascalCase types and members) at `warning` severity, and verify a temporary block-scoped namespace in a scratch file makes `dotnet format src --verify-no-changes` exit non-zero (remove the scratch file afterwards)
- [ ] 2.3 Decide how to handle IDE0005 (unnecessary usings) as described in design.md Risks (turn on `GenerateDocumentationFile` with `CS1591` off, or leave it at `suggestion`), record the choice in a comment in `.editorconfig` or `Directory.Build.props`, and verify `dotnet build src` has no new CS1591 warnings

## 3. Make the existing code lint-clean

- [ ] 3.1 Run `dotnet format src` to apply automatic fixes, then fix the remaining diagnostics by hand without changing public type or member signatures of `Sessionize.Api.Client`. Use a targeted suppression with a justification only where a public name must stay. Verify `dotnet format src --verify-no-changes` exits with code zero
- [ ] 3.2 Verify `dotnet build src` has no new warnings and `dotnet test src/Sessionize.Api.Client.Tests` passes after the fixes

## 4. CI lint step

- [ ] 4.1 Add a `dotnet format src --verify-no-changes` step to the `build` job in `.github/workflows/main.yml`, after `actions/setup-dotnet` and before `dotnet build`, and verify the workflow YAML is valid and the step uses the same command as the docs

## 5. Documentation

- [ ] 5.1 Add a "Lint" section to `.claude/skills/build-and-test/SKILL.md` with `dotnet format src --verify-no-changes` (check) and `dotnet format src` (fix), add the lint check to its "Before you finish" list, and fix its claim that the commands match CI. Verify the documented commands run as written
- [ ] 5.2 Add the lint command to the "Build and test" section of `CLAUDE.md` with the instruction to run it before opening a pull request, and verify the command text matches the skill
- [ ] 5.3 Add a "run `dotnet format src --verify-no-changes`" step to both pull request checklists in `CONTRIBUTING.md`, and verify both lists include it
- [ ] 5.4 Update `.claude/agents/code-style-critic.md` to say the conventions are defined in `.editorconfig` and checked by the lint command, instead of saying there is no `.editorconfig`, and verify the conventions listed there still match `.editorconfig`

## 6. Integration check

- [ ] 6.1 From a clean checkout, run `dotnet format src --verify-no-changes`, `dotnet build src` and `dotnet test src/Sessionize.Api.Client.Tests` and verify all three succeed, which shows that the default branch will be lint-clean as the spec requires

## Workflow follow-up

- After merge, the maintainer can remove the `SONAR_TOKEN` repository secret, archive the SonarCloud project `nikneem_sessionize-api-client`, and remove any branch protection rule that requires the SonarCloud check.
- Archive the change after review.
