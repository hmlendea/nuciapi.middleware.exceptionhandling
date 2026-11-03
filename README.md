[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nuciapi.middleware.exceptionhandling)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/releases/latest)
[![Build Status](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/NuciAPI.Middleware.ExceptionHandling)](https://nuget.org/packages/NuciAPI.Middleware.ExceptionHandling)
[![License](https://img.shields.io/github/license/hmlendea/nuciapi.middleware.exceptionhandling)](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/master/LICENSE)

# NuciAPI.Middleware.ExceptionHandling

NuciAPI.Middleware.ExceptionHandling is an ASP.NET Core middleware library that translates selected runtime exceptions from the downstream request pipeline into consistent JSON HTTP error responses. It is distributed as a NuciAPI.Middleware.ExceptionHandling NuGet package targeting `net10.0`.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Installation](#installation)
  - [Package Manager Installation](#package-manager-installation)
- [Usage](#usage)
- [Compatibility](#compatibility)
- [Development](#development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Test](#test)
- [GitHub Actions](#github-actions)
- [Project Structure](#project-structure)
  - [Projects and Packages](#projects-and-packages)
  - [Directories](#directories)
- [Architecture](#architecture)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [Security](#security)
- [Privacy and Data](#privacy-and-data)
- [Project Engagement](#project-engagement)
- [License](#license)

## ✨ Capabilities

- **Pipeline registration** — `app.UseNuciApiExceptionHandling()` registers the middleware at the chosen pipeline position.
- **Exception classification** — Maps 10 exception categories to specific HTTP status codes (400, 401, 403, 404, 409, 499, 500, 501, 503).
- **Consistent JSON responses** — All error responses use `NuciApiErrorResponse` from `NuciAPI.Responses` with `Content-Type: application/json`.
- **Client-abort representation** — `OperationCanceledException` maps to HTTP 499 (non-standard, client-closed request).
- **Fallback handling** — Any unmapped exception produces HTTP 500 with `NuciApiErrorResponse.InternalServerError`.

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciAPI.Middleware.ExceptionHandling)

### Package Manager Installation

```bash
dotnet add package NuciAPI.Middleware.ExceptionHandling
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciAPI.Middleware.ExceptionHandling
```

## 🚀 Usage

Register the middleware in your ASP.NET Core application's request pipeline:

```csharp
var app = builder.Build();

app.UseNuciApiExceptionHandling();

// ... other middleware and endpoint mappings

app.Run();
```

The middleware catches exceptions thrown by downstream components and converts them to standardised JSON error responses. Place it early enough in the pipeline to catch exceptions from controllers, endpoint filters, and other middleware.

### Exception Mapping Table

| Exception Type(s) | HTTP Status | Status Code | Response Payload |
|-------------------|-------------|-------------|------------------|
| `BadHttpRequestException`, `FormatException`, `ArgumentException`, `ValidationException` | Bad Request | 400 | `NuciApiErrorResponse(exception.Message, ErrorCodes.BadRequest)` |
| `SecurityException`, `UnauthorizedAccessException` | Forbidden | 403 | `NuciApiErrorResponse.Unauthorised` |
| `HttpRequestException`, `TaskCanceledException`, `TimeoutException` | Service Unavailable | 503 | `NuciApiErrorResponse.ServiceDependencyUnavailable` |
| `AuthenticationException` | Unauthorized | 401 | `NuciApiErrorResponse.AuthenticationFailure` |
| `EntityNotFoundException`, `KeyNotFoundException` | Not Found | 404 | `NuciApiErrorResponse.NotFound` |
| `EntityAlreadyExistsException` | Conflict | 409 | `NuciApiErrorResponse.AlreadyExists` |
| `RequestAlreadyProcessedException` | Conflict | 409 | `NuciApiErrorResponse.AlreadyProcessed` |
| `NotImplementedException` | Not Implemented | 501 | `NuciApiErrorResponse(message, ErrorCodes.NotImplemented)` |
| `OperationCanceledException` | Client Closed Request | 499 | `NuciApiErrorResponse.ClientClosedTheRequest` |
| *Any other exception* | Internal Server Error | 500 | `NuciApiErrorResponse.InternalServerError` |

See [docs/exception-mapping.md](docs/exception-mapping.md) for the complete authoritative mapping with source packages and implementation notes.

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
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

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [dotnet.yml](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/master/.github/workflows/dotnet.yml) | Continuous Integration | Restores, builds, and tests on every push and PR to `master` |
| [github-release.yml](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/master/.github/workflows/github-release.yml) | Release Packaging | Packs Release configuration, calculates SHA256, uploads `.nupkg` to GitHub Release, appends checksum to release notes |
| [nuget-release.yml](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/master/.github/workflows/nuget-release.yml) | NuGet.org Publishing | Publishes the NuGet package to NuGet.org using OIDC authentication when a GitHub Release is published |

## 🗂️ Project Structure

```
nuciapi.middleware.exceptionhandling/
├── .github/workflows/           # CI/CD automation
│   ├── dotnet.yml              # Continuous integration
│   ├── github-release.yml      # Release packaging
│   └── nuget-release.yml       # NuGet.org publishing
├── NuciAPI.Middleware.ExceptionHandling/          # Library project
│   ├── ExceptionHandlingMiddleware.cs             # Core middleware
│   ├── ExceptionHandlingMiddlewareExtensions.cs   # Public API
│   ├── NuciAPI.Middleware.ExceptionHandling.csproj
│   └── AssemblyInfo.cs
├── NuciAPI.Middleware.ExceptionHandling.UnitTests/  # Test project
│   ├── ExceptionHandlingMiddlewareTests.cs
│   └── NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj
├── NuciAPI.Middleware.ExceptionHandling.ArchitectureTests/  # Architecture tests
│   ├── ArchitectureProjectSourceTests.cs
│   ├── AssemblyInfo.cs
│   ├── NuciAPI.Middleware.ExceptionHandling.ArchitectureTests.csproj
│   ├── ProductionArchitectureTests.cs
│   ├── ProjectConfigurationTests.cs
│   └── SourceFileConventionTests.cs
├── NuciAPI.Middleware.ExceptionHandling.sln       # Solution file
├── ARCHITECTURE.md                                # Root architecture doc
├── README.md                                      # This file
├── SECURITY.md                                    # Security policy
├── PRIVACY.md                                     # Privacy policy
├── LICENSE                                        # GPL-3.0-or-later
└── docs/                                          # Technical documentation
    ├── overview.md
    ├── architecture.md
    ├── exception-mapping.md
    ├── testing.md
    └── development.md
```

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `NuciAPI.Middleware.ExceptionHandling` | Library (NuGet) | Exception handling middleware |
| `NuciAPI.Middleware.ExceptionHandling.UnitTests` | Test (NUnit) | Unit tests for exception mappings |
| `NuciAPI.Middleware.ExceptionHandling.ArchitectureTests` | Test (ArchUnitNET, NUnit) | Architecture and convention tests |

### Directories

| Directory | Purpose |
|-----------|---------|
| `.github/workflows/` | GitHub Actions CI/CD pipelines |
| `NuciAPI.Middleware.ExceptionHandling/` | Main library source |
| `NuciAPI.Middleware.ExceptionHandling.UnitTests/` | Unit test project |
| `NuciAPI.Middleware.ExceptionHandling.ArchitectureTests/` | Architecture test project |
| `docs/` | Technical documentation |

## 🏗️ Architecture

NuciAPI.Middleware.ExceptionHandling follows a library-oriented ASP.NET Core middleware adapter pattern with a public extension method and an internal sealed middleware implementation.

See the [architecture documentation](ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 📚 Documentation

| Resource | Description |
|----------|-------------|
| [README.md](README.md) | Installation, usage, exception mapping table, quick start |
| [ARCHITECTURE.md](ARCHITECTURE.md) | System context, architectural style, runtime flows, deployment |
| [SECURITY.md](SECURITY.md) | Vulnerability reporting, supported versions, disclosure policy |
| [PRIVACY.md](PRIVACY.md) | Data handling practices (no personal data collected) |
| [LICENSE](LICENSE) | GPL-3.0-or-later license text |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Contribution guidelines |
| [docs/overview.md](docs/overview.md) | High-level purpose, scope, capabilities, and boundaries |
| [docs/architecture.md](docs/architecture.md) | Detailed component view, data flows, dependencies, extension points |
| [docs/exception-mapping.md](docs/exception-mapping.md) | Complete authoritative exception-to-HTTP mapping table |
| [docs/testing.md](docs/testing.md) | Test structure, execution, coverage analysis, adding tests |
| [docs/development.md](docs/development.md) | Build, test, pack, release, contribution workflow |

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Include tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/issues) for problems or suggestions

See the [contributing guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 🔒 Security

NuciAPI.Middleware.ExceptionHandling follows a responsible disclosure policy for security vulnerabilities.

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 🛡️ Privacy and Data

NuciAPI.Middleware.ExceptionHandling collects, transmits, and stores no personal data. It operates entirely within the consumer application's process and does not communicate with external services, telemetry endpoints, or project maintainers.

For the detailed description of how the application handles privacy and personal data, see [PRIVACY.md](./PRIVACY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0 or later`.
See [LICENSE](./LICENSE) for additional information.
