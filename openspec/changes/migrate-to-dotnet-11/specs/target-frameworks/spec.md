# Spec Delta

## Purpose

Defines which .NET target frameworks the published Sessionize API client NuGet package and its test projects support, so consumers know which runtimes they can use and CI builds and tests every supported target.

## ADDED Requirements

### Requirement: Package ships a .NET 11 build
The published NuGet package SHALL contain an assembly built for `net11.0`.

#### Scenario: Package contents include net11.0
- **WHEN** the library is packed
- **THEN** the produced `.nupkg` contains `lib/net11.0/Sessionize.Api.Client.dll`

#### Scenario: .NET 11 consumer resolves the net11.0 build
- **WHEN** a project that targets `net11.0` references the package
- **THEN** the project restores and builds against the `net11.0` assembly

### Requirement: Package keeps the .NET 10 build
The published NuGet package SHALL keep an assembly built for `net10.0`, so consumers on the .NET 10 LTS release can still install new versions.

#### Scenario: Package contents include net10.0
- **WHEN** the library is packed
- **THEN** the produced `.nupkg` also contains `lib/net10.0/Sessionize.Api.Client.dll`

#### Scenario: .NET 10 consumer can upgrade
- **WHEN** a project that targets `net10.0` updates to the new package version
- **THEN** the project restores and builds without errors

### Requirement: Public API is the same on every target
The library SHALL expose the same public types and members on every target framework it ships.

#### Scenario: Same public surface
- **WHEN** the public API of the `net10.0` and `net11.0` assemblies is compared
- **THEN** both expose the same public types and members as the previous `net10.0`-only release

### Requirement: Test projects run on .NET 11
The unit test and integration test projects SHALL target `net11.0`, and the test suites SHALL pass on .NET 11.

#### Scenario: Unit tests on .NET 11
- **WHEN** the unit test project is run with the .NET 11 SDK
- **THEN** it runs on the .NET 11 runtime and all tests pass

### Requirement: CI builds with the .NET 11 SDK
CI SHALL build, test and pack the solution with the .NET 11 SDK, and SHALL fail if any target framework of the library fails to build.

#### Scenario: CI run on push
- **WHEN** a commit is pushed and the build workflow runs
- **THEN** the workflow installs the .NET 11 SDK and builds the library for both `net10.0` and `net11.0`, runs the tests, and produces the package

#### Scenario: One target fails to compile
- **WHEN** the library compiles for one target framework but not the other
- **THEN** the CI build fails
