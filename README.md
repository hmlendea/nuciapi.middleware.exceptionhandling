[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nuciapi.middleware.exceptionhandling)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/releases/latest)
[![Build Status](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciAPI.Middleware.ExceptionHandling

ASP.NET Core middleware that translates common runtime exceptions into consistent JSON API error responses.

It is intended for NuciAPI-based services and integrates directly into the request pipeline through a single extension method.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Installation](#installation)
    - [.NET CLI](#net-cli)
    - [Package Manager](#package-manager)
- [Usage](#usage)
    - [Minimal API (Program.cs)](#minimal-api-programcs)
    - [ASP.NET Core Startup class](#aspnet-core-startup-class)
    - [Exception Mapping](#exception-mapping)
    - [Response Format](#response-format)
    - [Notes](#notes)
- [Compatibility](#compatibility)
- [Development](#development)
    - [Requirements](#requirements-1)
    - [Setup](#setup)
    - [Build](#build)
    - [Test](#test)
    - [Pack](#pack)
- [GitHub Actions](#github-actions)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Related Projects](#related-projects)
- [Security](#security)
- [Contributing](#contributing)
- [Project Engagement](#project-engagement)
- [License](#license)

## ✨ Capabilities

- Registers exception handling in an ASP.NET Core pipeline with `UseNuciApiExceptionHandling()`.
- Maps recognised exceptions to consistent HTTP status codes and `NuciApiErrorResponse` payloads.
- Writes JSON responses with `Content-Type: application/json`.
- Represents client-aborted requests with HTTP 499.

## 📦 Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciAPI.Middleware.ExceptionHandling)

### .NET CLI

```bash
dotnet add package NuciAPI.Middleware.ExceptionHandling
```

### Package Manager

```powershell
Install-Package NuciAPI.Middleware.ExceptionHandling
```

## 🚀 Usage

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

### Exception Mapping

The middleware currently maps the following exceptions:

| Exception type(s) | HTTP status | Error response |
| --- | --- | --- |
| `BadHttpRequestException`, `FormatException`, `ArgumentException`, `ValidationException` | 400 Bad Request | `NuciApiErrorResponse` with exception message and bad-request code |
| `SecurityException`, `UnauthorizedAccessException` | 403 Forbidden | `NuciApiErrorResponse.Unauthorised` |
| `HttpRequestException`, `TaskCanceledException`, `TimeoutException` | 503 Service Unavailable | `NuciApiErrorResponse.ServiceDependencyUnavailable` |
| `AuthenticationException` | 401 Unauthorized | `NuciApiErrorResponse.AuthenticationFailure` |
| `EntityNotFoundException`, `KeyNotFoundException` | 404 Not Found | `NuciApiErrorResponse.NotFound` |
| `EntityAlreadyExistsException` | 409 Conflict | `NuciApiErrorResponse.AlreadyExists` |
| `RequestAlreadyProcessedException` | 409 Conflict | `NuciApiErrorResponse.AlreadyProcessed` |
| `NotImplementedException` | 501 Not Implemented | `NuciApiErrorResponse` with not-implemented code |
| `OperationCanceledException` | 499 Client Closed Request | `NuciApiErrorResponse.ClientClosedTheRequest` |
| Any other unhandled exception | 500 Internal Server Error | `NuciApiErrorResponse.InternalServerError` |

### Response Format

Responses are written as JSON and use the error response types from `NuciAPI.Responses`.

The middleware sets:

- status code according to the mapping table
- `Content-Type: application/json`

### Notes

- Register this middleware early enough in the pipeline to capture downstream exceptions.
- The middleware converts exceptions to HTTP responses and prevents unhandled exceptions from propagating further.
- For cancellations, client-aborted requests are represented as HTTP 499.

## 🧩 Compatibility

| Component | Supported version | Notes |
|-----------|------------------|-------|
| .NET | `net10.0` | The package targets .NET 10. |
| ASP.NET Core | `Microsoft.AspNetCore.App` for `net10.0` | Required framework reference. |


## 🛠️ Development

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

```bash
dotnet restore NuciAPI.Middleware.ExceptionHandling.sln
```

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
dotnet pack NuciAPI.Middleware.ExceptionHandling.sln -c Release
```

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [.NET](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/master/.github/workflows/dotnet.yml) | Continuous integration | Restores, builds, and tests on pushes and pull requests targeting `master`. |
| [GitHub Release](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/master/.github/workflows/github-release.yml) | Package publication | Packs the published release version and uploads the NuGet package and SHA256 checksum to the GitHub Release. |

## 🗂️ Project Structure

| Project or directory | Purpose |
|----------------------|---------|
| [`NuciAPI.Middleware.ExceptionHandling`](NuciAPI.Middleware.ExceptionHandling) | Library implementation and public pipeline extension. |
| [`NuciAPI.Middleware.ExceptionHandling.UnitTests`](NuciAPI.Middleware.ExceptionHandling.UnitTests) | NUnit tests for exception mapping and downstream delegation. |
| [`.github/workflows`](.github/workflows) | Continuous integration and release automation. |
| [`NuciAPI.Middleware.ExceptionHandling.sln`](NuciAPI.Middleware.ExceptionHandling.sln) | Solution containing the library and unit tests. |

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Add tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/issues) for problems or suggestions

## 🏗️ Architecture

See the [architecture documentation](ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🔗 Related Projects

- [NuciAPI.Middleware](https://github.com/hmlendea/nuciapi.middleware)
- [NuciAPI.Middleware.ExceptionHandling](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling)
- [NuciAPI.Middleware.Logging](https://github.com/hmlendea/nuciapi.middleware.logging)
- [NuciAPI.Middleware.Security](https://github.com/hmlendea/nuciapi.middleware.security)

## 🔒 Security

Report security vulnerabilities according to the [Security Policy](./SECURITY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/issues)!

If you find this project useful, consider starring ⭐️ it on GitHub!

## 📄 License

This project is being distributed under the `GNU General Public License v3.0` or later.
See [LICENSE](LICENSE) for further information.
