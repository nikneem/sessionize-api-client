# Spec Delta

## Purpose

Keeps the Sessionize API client library and its test projects free of third-party packages with known vulnerabilities, and makes the build enforce this so vulnerable dependencies are not published to NuGet consumers.

## ADDED Requirements

### Requirement: No known high or critical vulnerabilities in dependencies
The solution SHALL NOT resolve any direct or transitive NuGet package that has a publicly known vulnerability rated high or critical, in any project under `src/`.

#### Scenario: Clean vulnerability report
- **WHEN** a vulnerability report that includes transitive packages is produced for the solution
- **THEN** it lists no package with a high or critical advisory

#### Scenario: Vulnerable transitive package
- **WHEN** a transitive package has a high or critical advisory and a patched version exists
- **THEN** the solution resolves the patched version instead

### Requirement: Build fails on vulnerable dependencies
Restoring or building the solution SHALL fail when any direct or transitive package has a known high or critical vulnerability. This applies both locally and in CI.

#### Scenario: Vulnerable package introduced
- **WHEN** a change adds or keeps a package with a known high or critical advisory and the solution is built
- **THEN** the build fails with an error that names the vulnerable package

#### Scenario: Lower-severity advisory
- **WHEN** a package has only a low or moderate advisory
- **THEN** the build still succeeds and reports the advisory as a warning

### Requirement: Dependencies are current stable releases
Every direct NuGet package reference SHALL be on the latest stable release that is compatible with the solution's target framework when this change is applied. Packages from the same family SHALL use one version across all projects.

#### Scenario: Outdated package check
- **WHEN** the list of outdated packages is produced right after the update
- **THEN** no direct reference has a newer stable, target-compatible version available, unless the exception is documented in the change's design

#### Scenario: Consistent versions across projects
- **WHEN** the same package is referenced by more than one project
- **THEN** every project references the same version

### Requirement: Public API unchanged by dependency updates
Dependency updates SHALL NOT change the public API surface or the target framework of the published client library.

#### Scenario: Existing tests still pass
- **WHEN** the dependencies are updated and the unit test project is run
- **THEN** all existing tests pass without changes to the library's public types or members
