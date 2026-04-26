[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nuciapi.middleware.exceptionhandling)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/releases/latest)
[![Build Status](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciAPI.Middleware.ExceptionHandling

ASP.NET Core middleware that translates common runtime exceptions into consistent JSON API error responses.

It is intended for NuciAPI-based services and integrates directly in the request pipeline through a single extension method.

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciAPI.Middleware.ExceptionHandling)

### .NET CLI

```bash
dotnet add package NuciAPI.Middleware.ExceptionHandling
```

### Package Manager

```powershell
Install-Package NuciAPI.Middleware.ExceptionHandling
```

## Requirements

- .NET SDK/runtime with support for `net10.0`
- ASP.NET Core (`Microsoft.AspNetCore.App`)

## What This Package Includes

- `UseNuciApiExceptionHandling()` extension method for `IApplicationBuilder`
- Exception handling middleware that:
	- catches known exception categories
	- maps them to HTTP status codes
	- writes JSON error payloads (`application/json`)

## Quick Start

Register the middleware in your application startup pipeline.

### Minimal API (Program.cs)

```csharp
using NuciAPI.Middleware.ExceptionHandling;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.UseNuciApiExceptionHandling();

app.MapGet("/", () => "OK");

app.Run();
```

### ASP.NET Core Startup class

```csharp
using NuciAPI.Middleware.ExceptionHandling;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseNuciApiExceptionHandling();

        // Other middleware/components
    }
}
```

## Exception Mapping

The middleware currently maps the following exceptions:

| Exception type(s) | HTTP status | Error response |
| --- | --- | --- |
| `BadHttpRequestException`, `FormatException`, `ArgumentException`, `ValidationException` | 400 Bad Request | `NuciApiErrorResponse` with exception message and bad-request code |
| `SecurityException`, `UnauthorizedAccessException` | 403 Forbidden | `NuciApiErrorResponse.Unauthorised` |
| `HttpRequestException`, `TaskCanceledException`, `TimeoutException` | 503 Service Unavailable | `NuciApiErrorResponse.ServiceDependencyUnavailable` |
| `AuthenticationException` | 401 Unauthorized | `NuciApiErrorResponse.AuthenticationFailure` |
| `EntityNotFoundException`, `KeyNotFoundException` | 404 Not Found | `NuciApiErrorResponse.NotFound` |
| `RequestAlreadyProcessedException` | 409 Conflict | `NuciApiErrorResponse.AlreadyExists` or explicit already-processed response |
| `NotImplementedException` | 501 Not Implemented | `NuciApiErrorResponse` with not-implemented code |
| `OperationCanceledException` | 499 Client Closed Request | `NuciApiErrorResponse.ClientClosedTheRequest` |
| Any other unhandled exception | 500 Internal Server Error | `NuciApiErrorResponse.InternalServerError` |

## Response Format

Responses are written as JSON and use the error response types from `NuciAPI.Responses`.

The middleware sets:

- status code according to the mapping table
- `Content-Type: application/json`

## Notes

- Register this middleware early enough in the pipeline to capture downstream exceptions.
- The middleware converts exceptions to HTTP responses and prevents unhandled exceptions from propagating further.
- For cancellations, client-aborted requests are represented as HTTP 499.


## Development

### Build

```bash
dotnet build NuciAPI.Middleware.ExceptionHandling.sln
```

### Test

```bash
dotnet test NuciAPI.Middleware.ExceptionHandling.sln
```

### Pack

```bash
dotnet pack -c Release
```

## Contributing

Contributions are welcome.

Please:

- keep the changes cross-platform
- keep the pull requests focused and consistent with the existing style
- update the documentation when the behaviour changes
- add or update the tests for any new behaviour

## Related Projects

- [NuciAPI.Middleware](https://github.com/hmlendea/nuciapi.middleware)
- [NuciAPI.Middleware.ExceptionHandling](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling)
- [NuciAPI.Middleware.Logging](https://github.com/hmlendea/nuciapi.middleware.logging)
- [NuciAPI.Middleware.Security](https://github.com/hmlendea/nuciapi.middleware.security)

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.
