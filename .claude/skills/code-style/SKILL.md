---
name: code-style
description: The C# code style of the Sessionize API client as it is in the code under src - naming, formatting, file layout and idioms. Use when writing or reviewing C# code or project files.
---

# Code style

There is no `.editorconfig`; these conventions are taken from the code under `src/`. The `code-style-critic` agent checks them.

## Files and namespaces

- One type per file; the file name equals the type name.
- File-scoped namespaces that match the folder, for example `namespace Sessionize.Api.Client.Filters;` in `src/Sessionize.Api.Client/Filters/`.
- `ImplicitUsings` and `Nullable` are enabled in every project. Do not add usings for `System`, `System.Linq`, `System.Net.Http` or `System.Threading.Tasks`. Explicit usings come first, `System.*` before `Microsoft.*` before `Sessionize.*`.
- The integration test project adds a global `<Using Include="Xunit" />`.

## Formatting

- C# files use four spaces, Allman braces (opening brace on its own line).
- Short guard clauses may omit braces: `if (filter == null)` followed by `return response;` on the next line. Other `if` blocks use braces.
- Long method signatures stay on one line; constructors with several dependencies put one parameter per line.
- Avoid the double blank lines that appear in a few files (`ErrorCode.cs`, `ServiceCollectionExtensions.cs`); do not add new ones.

## Naming

- PascalCase for types, methods, properties, constants and `static readonly` fields (`ApiIdRegex`, `SectionName`).
- `_camelCase` for private instance fields (`_httpClientFactory`, `_logger`, `_sessionizeConfiguration`).
- Interfaces start with `I` (`ISessionizeApiClient`); async methods end in `Async`.
- Top-level DTOs end in `Response` (in `DataTransferObjects/`); nested records live in `ValueObjects/` (for example `SessionDetails`, `SessionInfo`); filters end in `Filter`.
- Error codes use `nameof` for their code: `new(nameof(InvalidApiId), "...")`.

## Idioms

- DTOs and value objects are positional `record`s: short ones on one line (`SpeakerWallResponse`), long ones with one parameter per line (`SessionDetails`).
- Filters and options are classes with nullable `{ get; set; }` auto-properties.
- Copy records with `with` expressions; never mutate a response.
- LINQ method syntax (`Select`, `Where`, `Any`, `ToList`, `ToHashSet`), not query syntax.
- Text comparisons use `StringComparison.OrdinalIgnoreCase`; paths are built with string interpolation.
- Target-typed `new(...)` for field initialisers; `var` for locals.
- Logging uses `_logger.LogInformation("Constant message")`, never interpolated messages.
- Errors throw `SessionizeApiClientException` with an `ErrorCode`, never bare `Exception`. Configuration validation throws `ArgumentException` with `nameof`.
- Optional parameters `string? sessionizeApiId = null, CancellationToken? cancellationToken = null` come last on public client methods.

## Project files

- `src/Sessionize.Api.Client/Sessionize.Api.Client.csproj` is indented with tabs; the test projects with two spaces. Keep each file's existing indentation.
- Package versions are pinned exactly (`Version="10.5.0"`).

## Tests

- xUnit `[Fact]` (and `[Theory]` for data-driven cases); test method names describe the scenario and the expected result.
- How to write and run the tests is in the `build-and-test` skill.
