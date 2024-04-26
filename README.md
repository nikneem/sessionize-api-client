[![Publish NuGet Package](https://github.com/nikneem/sessionize-api-client/actions/workflows/main.yml/badge.svg)](https://github.com/nikneem/sessionize-api-client/actions/workflows/main.yml)

# Sessionize HTTP Client Library for .NET

## Overview

The Sessionize HTTP Client Library for .NET is a software library designed to simplify communication with the Sessionize platform's API. Sessionize is a platform where conference organizers and speakers connect, facilitating the management of session data, room information, and schedules for events. This library provides a convenient interface for .NET applications to interact with Sessionize's API endpoints, allowing developers to integrate Sessionize functionality seamlessly into their applications.

## Features

- **Easy Communication**: Simplifies the process of making HTTP requests to Sessionize's API endpoints.
- **Retrieve Session Data**: Allows users to fetch session data from Sessionize, including details about speakers, session schedules, and more.
- **Access Room Information**: Provides functionality to access information about conference rooms available on Sessionize.
- **Manage Schedules**: Enables developers to retrieve schedule information for conferences and events hosted on Sessionize.
- **Asynchronous Support**: Supports asynchronous operations for improved performance and responsiveness.

## Getting Started

To begin using the Sessionize HTTP Client Library for .NET in your project, follow these steps:

1. Install the [library package from NuGet](https://www.nuget.org/packages/Sessionize.Api.Client/)
2. Configure your the library and inject the Sessionize API Client using dependency injection
3. Create an instance of the `SessionizeApiClient` class.
4. Use the provided methods to interact with Sessionize's API endpoints.

## Configuration

As a conference organizer, you must enable API/Embed access on the Sessionize website. Once you have done that, you will receive an API Endpoint ID. To configure the library, add the following to your application's configuration:

```json
{
  "Sessionize": {
    "BaseUrl": "url to sessionize (defaults to https://sessionize.com)",
    "ApiId": "The API ID mentioned above (optional)"
  }
}
```

The ApiId is optional. You can also create an instance of the `SessionizeApiClient` and set the Api ID through the `SessionizeApiId` property. Either setting the `SessionizeApiId` though configuration or by setting the property is mandatory, prior to making a call to the Sessionize API.

## Depencency Injection

You can take advantage of Dependency Injection. When your configuration is all set and, just do a `services.AddSessionizeApiClient()` and pass in your `IConfigurationBuilder` to add the API client to your Service Collection. You can now inject the `ISessionizeApiClient` interface wherever you need to call the API.
