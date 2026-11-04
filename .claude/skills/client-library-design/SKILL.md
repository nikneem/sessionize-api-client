---
name: client-library-design
description: The design of the Sessionize API client library as found in the code - folders and their roles, HttpClient and resilience, models, configuration options, dependency injection, error handling and cancellation token conventions.
---

# Sessionize client library design

## Layout and boundaries

`src/Sessionize.Api.Client` is a single library project. Folders map to namespaces:

| Folder | Role |
|---|---|
| `Abstractions/` | `ISessionizeApiClient`, the only interface consumers should depend on |
| root | `SessionizeApiClient`, the implementation (request building, sending, deserializing, filtering) |
| `Configuration/` | `SessionizeConfiguration` options (`Sessionize` section: `BaseUrl`, `ApiId`) |
| `DependencyInjection/` | `ServiceCollectionExtensions.AddSessionizeApiClient` overloads |
| `DataTransferObjects/` | top-level response records (`AllDataResponse`, `ScheduleGridResponse`, `SpeakerDetailsResponse`, `SessionListResponse`, `SpeakerWallResponse`) |
| `ValueObjects/` | nested records used by the responses (sessions, speakers, rooms, time slots) |
| `Filters/` | `SessionFilter`, `SpeakerFilter` for client-side filtering |
| `Exceptions/` | `SessionizeApiClientException` and the `ErrorCode` catalogue |

Dependency direction: DTOs, value objects, filters and exceptions depend on nothing else in the library. The client depends on all of them; the DI extensions depend on the client and configuration. Tests depend on the library, never the other way round.

## Requests

- Every call is a GET to `{BaseUrl}{apiId}/view/{ViewName}`. `SessionizeConfiguration.BaseUrl` normalises any base to end with `/api/v2/` and defaults to `https://sessionize.com/api/v2/`; it must be HTTPS.
- The API id is resolved in order: method argument, the `SessionizeApiId` init property, then `SessionizeConfiguration.ApiId`. Missing id gives `ErrorCode.InvalidConfiguration`; an id that does not match `^[a-zA-Z0-9]{8,12}$` gives `ErrorCode.InvalidApiId`.
- All public methods go through the private generic `SendRequestAsync<TResult>`, which creates a client from `IHttpClientFactory`, sends, calls `EnsureSuccessStatusCode()` and deserializes.

## HttpClient and resilience

- The client never creates `HttpClient` itself; it uses `IHttpClientFactory.CreateClient()`.
- DI registers `AddHttpClient<SessionizeApiClient>().AddStandardResilienceHandler()` from Microsoft.Extensions.Http.Resilience and `AddScoped<ISessionizeApiClient, SessionizeApiClient>()`. Note that `CreateClient()` returns the default unnamed client, so verify that the resilience pipeline applies to it when changing this code.

## Models and serialization

- Responses are positional `record`s deserialized with `System.Text.Json` and a lazily created `JsonSerializerOptions` with `JsonNamingPolicy.CamelCase`.
- Empty bodies throw `ErrorCode.SessionizeResponseEmpty`; a null result throws `ErrorCode.DeserializationFailed`.
- Filtering is done client-side after download. Filters are optional: a null filter returns the response unchanged. Text matches use `Contains` with `OrdinalIgnoreCase`; dates compare `StartsAt`/`EndsAt` with `StartDate`/`EndDate`. Results are new copies made with `with`; containers that end up empty (rooms, schedule days, session groups) are removed, and `GetAllDataAsync` with a filter drops speakers without remaining sessions.

## Configuration and DI

- `AddSessionizeApiClient(IConfigurationBuilder, string? sectionName = null)` builds the configuration, binds `SessionizeConfiguration` from the section (default `Sessionize`) and validates on start.
- `AddSessionizeApiClient()` without arguments configures only the default base URL; the API id must then be supplied per call or through `SessionizeApiId`.
- Options are read through `IOptions<SessionizeConfiguration>` at request time.

## Conventions

- Public methods return `Task<T>`, end in `Async`, and take `string? sessionizeApiId = null, CancellationToken? cancellationToken = null` as the last parameters; a null token becomes `CancellationToken.None` and is passed to `SendAsync`.
- Filter overloads take the filter as the first parameter.
- Every public method logs one `LogInformation` line with a constant message.
- Errors are `SessionizeApiClientException` carrying an `ErrorCode` (sealed class with static readonly instances holding `Code` and `Message`), never bare `Exception`.
- Nullable reference types are enabled across the solution.
