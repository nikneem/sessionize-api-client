# Spec Delta

## Purpose

Makes sure the README, which is both the repository front page and the NuGet package readme, tells consumers accurately how to install, configure and use the Sessionize API client.

## ADDED Requirements

### Requirement: README documents installation and supported frameworks
The README SHALL give the command that installs the NuGet package and SHALL list every target framework the published package supports. The list SHALL match the package's actual target frameworks.

#### Scenario: Reader installs the package
- **WHEN** a reader follows the installation section
- **THEN** they find the `dotnet add package Sessionize.Api.Client` command and a link to the package on nuget.org

#### Scenario: Supported frameworks match the package
- **WHEN** the supported frameworks in the README are compared with the target frameworks of the published library
- **THEN** the two lists are the same

### Requirement: README documents configuration accurately
The README SHALL document the configuration section name, the `BaseUrl` and `ApiId` settings, the `BaseUrl` default, the HTTPS requirement, the automatic `/api/v2` suffix, and the API ID format (8–12 alphanumeric characters).

#### Scenario: Reader configures a custom base URL
- **WHEN** a reader looks up the `BaseUrl` setting
- **THEN** the README says it is optional, defaults to `https://sessionize.com`, must use HTTPS, and does not need `/api/v2` because the library adds it

#### Scenario: Reader checks the API ID format
- **WHEN** a reader looks up the API ID
- **THEN** the README says it comes from the Sessionize API/Embed settings and must be 8–12 alphanumeric characters

### Requirement: README documents API ID precedence
The README SHALL explain the three ways to supply the API ID and the order in which they are used: the per-call argument first, then the client's `SessionizeApiId` property, then the configured `ApiId`.

#### Scenario: Reader uses several events from one application
- **WHEN** a reader needs to query more than one Sessionize event
- **THEN** the README shows that each method takes an optional API ID argument that takes precedence over the configured value

#### Scenario: No API ID is supplied anywhere
- **WHEN** a reader looks up what happens if no API ID is set
- **THEN** the README says the call fails with the `InvalidConfiguration` error code

### Requirement: README documents dependency injection registration
The README SHALL document every public `AddSessionizeApiClient` overload with a code sample, SHALL state that `ISessionizeApiClient` is registered as scoped, and SHALL state that a standard HTTP resilience handler is included.

#### Scenario: Reader registers with configuration
- **WHEN** a reader follows the dependency injection section
- **THEN** they find a sample that calls `AddSessionizeApiClient` with a configuration builder, with a note that a custom section name can be passed

#### Scenario: Reader registers without configuration
- **WHEN** a reader wants the default base URL and supplies the API ID per call
- **THEN** they find the overload without arguments documented

### Requirement: README documents every public API method
The README SHALL list every method of `ISessionizeApiClient`, the Sessionize view each one calls, and its return type. It SHALL include at least one complete usage example.

#### Scenario: Method list is complete
- **WHEN** the method list in the README is compared with the public methods of `ISessionizeApiClient`
- **THEN** every method appears in the README and the README names no method that does not exist

#### Scenario: Reader copies the usage example
- **WHEN** a reader copies the usage example into a project that references the package
- **THEN** it uses only public types and members that exist in the package

### Requirement: README documents filtering
The README SHALL document `SessionFilter` and `SpeakerFilter`, every filter property, which methods accept each filter, and that filtering happens on the client after the full response is downloaded.

#### Scenario: Reader filters sessions by title
- **WHEN** a reader looks up session filtering
- **THEN** the README lists Title, Description, StartDate and EndDate, says text matches are case-insensitive "contains" matches, and names the methods that take a `SessionFilter`

#### Scenario: Reader filters top speakers
- **WHEN** a reader looks up speaker filtering
- **THEN** the README lists FirstName, LastName and IsTopSpeaker, and says IsTopSpeaker only applies to the speaker wall

### Requirement: README documents cancellation and errors
The README SHALL state that every API method accepts an optional cancellation token. It SHALL document the exceptions the library actually throws: the library exception and the error codes it raises, the exception for an invalid `BaseUrl`, and the HTTP request exception for non-success responses.

#### Scenario: Reader handles errors
- **WHEN** a reader looks up error handling
- **THEN** the README lists the error codes `InvalidConfiguration`, `InvalidApiId`, `SessionizeResponseEmpty` and `DeserializationFailed` with what each one means, and says that non-success HTTP status codes raise `HttpRequestException`

#### Scenario: Reader sets an invalid base URL
- **WHEN** a reader looks up what happens if `BaseUrl` is not an absolute HTTPS URL
- **THEN** the README says an `ArgumentException` is thrown when the value is set or bound from configuration

#### Scenario: Reader cancels a request
- **WHEN** a reader looks up cancellation
- **THEN** the README shows a call that passes a `CancellationToken`

### Requirement: README navigation matches its sections
The README's navigation links SHALL each point to a section that exists, and the README SHALL NOT contain obvious spelling errors in headings.

#### Scenario: Reader clicks a navigation link
- **WHEN** a reader clicks any link in the navigation bar at the top of the README
- **THEN** the page scrolls to an existing section with that name
