# Documentation Index

This directory contains detailed technical documentation for NuciAPI.Middleware.ExceptionHandling, complementing the root-level documentation.

## Document Map

| Document | Purpose | Audience |
|----------|---------|----------|
| [overview.md](overview.md) | High-level purpose, scope, capabilities, and boundaries | All |
| [architecture.md](architecture.md) | Detailed component view, data flows, dependencies, extension points | Contributors, architects |
| [exception-mapping.md](exception-mapping.md) | Complete authoritative exception-to-HTTP mapping table | Consumers, contributors |
| [testing.md](testing.md) | Test structure, execution, coverage analysis, adding tests | Contributors, testers |
| [development.md](development.md) | Build, test, pack, release, contribution workflow | Contributors, maintainers |

## Root-Level Documents (Reference)

| Document | Location | Purpose |
|----------|----------|---------|
| `ARCHITECTURE.md` | Repository root | System context, architectural style, runtime flows, deployment |
| `README.md` | Repository root | Installation, usage, exception mapping table, quick start |
| `SECURITY.md` | Repository root | Vulnerability reporting, supported versions, disclosure policy |
| `PRIVACY.md` | Repository root | Data handling practices (no personal data collected) |
| `LICENSE` | Repository root | GPL-3.0-or-later license text |

## Quick Navigation by Task

| Task | Start Here |
|------|------------|
| **Understand what this package does** | [overview.md](overview.md) → `README.md` |
| **Integrate into an ASP.NET Core app** | `README.md` → [exception-mapping.md](exception-mapping.md) |
| **Modify exception mappings** | [architecture.md](architecture.md) → [exception-mapping.md](exception-mapping.md) → [development.md](development.md) |
| **Run tests locally** | [testing.md](testing.md) → [development.md](development.md) |
| **Create a release** | [development.md](development.md) → `.github/workflows/github-release.yml` |
| **Add a new test case** | [testing.md](testing.md) |
| **Understand dependency graph** | [architecture.md](architecture.md) |
| **Debug middleware behaviour** | [development.md](development.md) → [architecture.md](architecture.md) |

## Key Implementation Files (Cross-Reference)

| Area | File |
|------|------|
| Public API | `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddlewareExtensions.cs` |
| Middleware logic | `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddleware.cs` |
| Unit tests | `NuciAPI.Middleware.ExceptionHandling.UnitTests/ExceptionHandlingMiddlewareTests.cs` |
| Project config | `NuciAPI.Middleware.ExceptionHandling/NuciAPI.Middleware.ExceptionHandling.csproj` |
| Test config | `NuciAPI.Middleware.ExceptionHandling.UnitTests/NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj` |
| Solution | `NuciAPI.Middleware.ExceptionHandling.sln` |
| CI | `.github/workflows/dotnet.yml` |
| Release | `.github/workflows/github-release.yml` |

## Version Compatibility

| Package Version | .NET Target | ASP.NET Core | Status |
|-----------------|-------------|--------------|--------|
| 1.0.2 (current) | net10.0 | Microsoft.AspNetCore.App (net10.0) | Active |
| 1.x | net10.0 | Microsoft.AspNetCore.App (net10.0) | Supported |

## Contributing to Documentation

- Documentation in `docs/` is technical and implementation-grounded
- Update relevant `.md` when changing implementation
- Keep cross-references valid (relative paths from `docs/`)
- Root documents (`README.md`, `ARCHITECTURE.md`, etc.) are user-facing; `docs/` is contributor-facing