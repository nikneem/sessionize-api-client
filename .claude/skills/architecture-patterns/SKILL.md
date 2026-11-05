---
name: architecture-patterns
description: The architecture and patterns of the Sessionize API client - projects, layers, module boundaries, dependency direction and the design patterns in use. Use before structural changes; the detailed design lives in client-library-design.
---

# Architecture and patterns

The details of requests, serialization, filtering and DI are in the `client-library-design` skill. This skill is the overview and the rules for structure.

## Projects

The solution is `src/Sessionize API Client.sln`:

| Project | Role | May reference |
|---|---|---|
| `src/Sessionize.Api.Client` | The NuGet library | Only framework and Microsoft.Extensions packages |
| `src/Sessionize.Api.Client.Tests` | Unit tests, HTTP mocked | The library |
| `src/Sessionize.Api.Client.IntegrationTests` | Tests against the live Sessionize API through DI | The library |

The library never references a test project.

## Layers inside the library

1. **Contract**: `Abstractions/ISessionizeApiClient.cs`. Consumers depend on this interface only.
2. **Implementation**: `SessionizeApiClient.cs` in the root namespace. Builds requests, sends them, deserializes and filters.
3. **Composition**: `DependencyInjection/ServiceCollectionExtensions.cs` (`AddSessionizeApiClient` overloads) and `Configuration/SessionizeConfiguration.cs`.
4. **Model**: `DataTransferObjects/`, `ValueObjects/`, `Filters/`, `Exceptions/`. These depend on nothing else in the library.

Dependencies point downwards: composition uses the implementation, the implementation uses the model. Do not let model types reference the client, configuration or DI.

## Patterns in use

- **Interface plus implementation, registered by DI**: `AddScoped<ISessionizeApiClient, SessionizeApiClient>()`.
- **Typed HttpClient via IHttpClientFactory with a resilience pipeline**: `AddHttpClient<SessionizeApiClient>().AddStandardResilienceHandler()`; the client calls `IHttpClientFactory.CreateClient()` and never news up `HttpClient`.
- **Options pattern**: `IOptions<SessionizeConfiguration>` bound from the `Sessionize` section with `ValidateOnStart()`; the `BaseUrl` setter validates HTTPS and the getter normalises to `/api/v2/`.
- **Single generic request pipeline**: every public method delegates to `SendRequestAsync<TResult>(viewName, ...)`.
- **Overload pairs**: each view has a plain method and a filter overload; filtering is client-side and returns copies made with `with`.
- **Immutable records** for all responses.
- **Smart enum for errors**: `ErrorCode` is a sealed class with a private constructor and `static readonly` instances, carried by `SessionizeApiClientException`.
- **Lazy initialisation** of the shared `JsonSerializerOptions`.

## Rules for changes

- Public signatures are part of a published NuGet package: add overloads, do not change or remove existing ones.
- A new Sessionize view follows the `add-sessionize-endpoint` skill.
- A new failure mode gets a new `ErrorCode`, not a new exception type.
- Keep the library free of extra dependencies; Microsoft.Extensions packages stay on the 10.x line.
