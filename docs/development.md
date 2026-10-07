# Development Guide

This document covers the development workflow, build process, testing, packaging, and contribution guidelines for NuciAPI.Middleware.ExceptionHandling.

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0.x | Build, test, pack, run |
| Git | Any recent | Version control |
| IDE | VS Code, Visual Studio, Rider | Development |

## Repository Structure

```
nuciapi.middleware.exceptionhandling/
├── .github/workflows/           # CI/CD automation
│   ├── dotnet.yml              # Continuous integration
│   └── github-release.yml      # Release packaging
├── NuciAPI.Middleware.ExceptionHandling/          # Library project
│   ├── ExceptionHandlingMiddleware.cs             # Core middleware
│   ├── ExceptionHandlingMiddlewareExtensions.cs   # Public API
│   ├── NuciAPI.Middleware.ExceptionHandling.csproj
│   └── AssemblyInfo.cs
├── NuciAPI.Middleware.ExceptionHandling.UnitTests/  # Test project
│   ├── ExceptionHandlingMiddlewareTests.cs
│   └── NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj
├── NuciAPI.Middleware.ExceptionHandling.sln       # Solution file
├── ARCHITECTURE.md                                # Root architecture doc
├── README.md                                      # User-facing documentation
├── SECURITY.md                                    # Security policy
├── PRIVACY.md                                     # Privacy policy
├── LICENSE                                        # GPL-3.0-or-later
└── docs/                                          # This documentation
    ├── overview.md
    ├── architecture.md
    ├── exception-mapping.md
    ├── testing.md
    └── development.md (this file)
```

## Common Commands

### Restore Dependencies
```bash
dotnet restore NuciAPI.Middleware.ExceptionHandling.sln
```

### Build
```bash
# Debug build (default)
dotnet build NuciAPI.Middleware.ExceptionHandling.sln

# Release build
dotnet build NuciAPI.Middleware.ExceptionHandling.sln -c Release
```

### Test
```bash
# Run all tests
dotnet test NuciAPI.Middleware.ExceptionHandling.sln

# Run with verbose output
dotnet test NuciAPI.Middleware.ExceptionHandling.sln --verbosity normal

# Run specific test project
dotnet test NuciAPI.Middleware.ExceptionHandling.UnitTests/

# Run specific test method
dotnet test --filter "FullyQualifiedName~Given_DownstreamException_When_InvokeAsync_Then_WritesMappedStatusCode"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Pack (Create NuGet Package)
```bash
# Pack Release configuration
dotnet pack NuciAPI.Middleware.ExceptionHandling.sln -c Release

# Pack with explicit version
dotnet pack NuciAPI.Middleware.ExceptionHandling.sln -c Release -p:Version=1.0.3

# Output location: NuciAPI.Middleware.ExceptionHandling/bin/Release/*.nupkg
```

### Clean
```bash
dotnet clean NuciAPI.Middleware.ExceptionHandling.sln
```

## Development Workflow

### 1. Branch Strategy
- **master** — Protected branch; all changes via PR
- **Feature branches** — `feature/description`, `fix/description`, `docs/description`
- **Release tags** — `v1.0.3` format triggers GitHub Release workflow

### 2. Making Changes

#### For Exception Mapping Changes
1. Modify `ExceptionHandlingMiddleware.cs` — add/modify catch blocks
2. Update `ExceptionHandlingMiddlewareTests.cs` — add test cases
3. Update `docs/exception-mapping.md` — document new mappings
4. Update `README.md` exception mapping table if user-facing

#### For Public API Changes
1. Modify `ExceptionHandlingMiddlewareExtensions.cs`
2. Consider backward compatibility — avoid breaking changes
3. Update version in `.csproj` (SemVer: MAJOR.MINOR.PATCH)

#### For Documentation Changes
1. Update relevant `.md` files in root and `docs/`
2. Ensure cross-references remain valid

### 3. Pre-Commit Checklist
- [ ] `dotnet build` succeeds
- [ ] `dotnet test` passes (all tests green)
- [ ] No compiler warnings (or documented suppressions)
- [ ] Documentation updated for user-facing changes
- [ ] Version bumped if releasing

### 4. Pull Request Process
1. Create branch from `master`
2. Make focused, atomic commits
3. Push branch and open PR against `master`
4. CI runs automatically (build + test)
5. Address review feedback
6. Squash and merge (or rebase and merge)

## Versioning

### Scheme: Semantic Versioning (MAJOR.MINOR.PATCH)

| Change Type | Version Bump | Example |
|-------------|--------------|---------|
| Breaking API change | MAJOR | 1.0.2 → 2.0.0 |
| New exception mapping, non-breaking | MINOR | 1.0.2 → 1.1.0 |
| Bug fix, internal improvement | PATCH | 1.0.2 → 1.0.3 |

### Version Location
- **Source of truth:** `NuciAPI.Middleware.ExceptionHandling.csproj` `<Version>` property
- **Release tag:** `v{Version}` (e.g., `v1.0.3`)
- **GitHub Release workflow** extracts version from tag

### Current Version
```xml
<Version>1.0.2</Version>
```

## Release Process

### Automated (GitHub Release Workflow)

1. **Create release on GitHub:**
   - Go to Releases → Create a new release
   - Tag: `v1.0.3` (must match version in `.csproj`)
   - Title: `NuciAPI.Middleware.ExceptionHandling 1.0.3`
   - Generate release notes or write manually

2. **Workflow triggers automatically** (`.github/workflows/github-release.yml`):
   - Checks out code with full history
   - Sets up .NET 10.0
   - Extracts version from tag (strips `v` prefix)
   - Restores dependencies
   - Packs Release configuration with extracted version
   - Calculates SHA256 checksum
   - Uploads `.nupkg` to GitHub Release
   - Appends SHA256 to release notes

### Manual Pack (for local verification)
```bash
dotnet pack -c Release -p:Version=1.0.3 --output ./nupkg
# Verify: ls ./nupkg/
# Install locally: dotnet add package --source ./nupkg NuciAPI.Middleware.ExceptionHandling
```

## Dependency Management

### Direct Dependencies (from `.csproj`)

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.App` | Framework | ASP.NET Core APIs |
| `NuciAPI.Middleware` | 2.0.3 | Base middleware class |
| `NuciAPI.Middleware.Security` | 1.0.6 | Security exceptions, idempotency |
| `NuciDAL` | 3.2.1 | Repository exceptions |
| `NuciLog.Core` | 3.1.0 | Logging (transitive?) |

### Test Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.NET.Test.Sdk` | 18.10.1 | Test runner |
| `NUnit` | 4.6.1 | Test framework |
| `NUnit3TestAdapter` | 6.3.0 | VS/Test SDK adapter |
| `Moq` | 4.20.72 | Mocking (available) |
| `NuciAPI.Middleware` | 2.0.3 | Base class for tests |
| `NuciAPI.Middleware.Security` | 1.0.6 | Exception types for tests |

### Updating Dependencies
```bash
# List outdated packages
dotnet list NuciAPI.Middleware.ExceptionHandling.csproj package --outdated

# Update specific package
dotnet add NuciAPI.Middleware.ExceptionHandling.csproj package NuciAPI.Middleware --version 2.0.4

# Update all (interactive)
dotnet nuget update NuciAPI.Middleware.ExceptionHandling.csproj
```

## Code Style and Conventions

### C# Conventions (from project settings)
- **ImplicitUsings:** `disable` — explicit `using` directives required
- **Nullable:** Not enabled in main project (enabled in test project)
- **Target:** `net10.0`

### Naming (per repository conventions)
- **Classes:** PascalCase (`ExceptionHandlingMiddleware`)
- **Methods:** PascalCase (`InvokeAsync`, `WriteErrorResponseAsync`)
- **Parameters:** camelCase (`context`, `statusCode`, `errorResponse`)
- **Private fields:** Not used (stateless middleware)
- **Constants:** PascalCase (via `NuciApiResponseCodes`, `NuciApiResponseMessages`)

### File Organisation
- One type per file (mostly)
- `internal sealed` for implementation classes
- `public static` for extension methods
- Namespace matches folder: `NuciAPI.Middleware.ExceptionHandling`

## Debugging

### Debug Unit Tests
```bash
# In VS Code: use .NET Test Explorer or launch.json
# In Visual Studio: Test Explorer
# In Rider: Unit Test Sessions
```

### Debug Middleware in Consumer App
1. Build local package: `dotnet pack -c Debug -o ./local-packages`
2. Add local source: `dotnet nuget add source ./local-packages -n local`
3. Reference in consumer: `dotnet add package NuciAPI.Middleware.ExceptionHandling --source local`
4. Attach debugger to consumer process

### Inspecting Generated Package
```bash
dotnet pack -c Release --output ./nupkg
# Examine contents:
unzip -l ./nupkg/NuciAPI.Middleware.ExceptionHandling.1.0.2.nupkg
# Or use NuGet Package Explorer (GUI)
```

## Common Issues and Solutions

### Build Fails: "Framework not found"
```bash
# Install .NET 10 SDK
# Verify: dotnet --list-sdks
```

### Tests Fail: "InternalsVisibleTo not working"
- Check `AssemblyInfo.cs` in main project has:
  ```csharp
  [assembly: InternalsVisibleTo("NuciAPI.Middleware.ExceptionHandling.UnitTests")]
  ```
- Verify test project references main project via `ProjectReference`

### Pack Fails: "Version not set"
- Ensure `<Version>` in `.csproj` is valid SemVer
- Or pass `-p:Version=x.y.z` explicitly

### CI Fails: "dotnet-version not found"
- Workflow uses `dotnet-version: 10.0.x` — ensure GitHub Actions runner has it
- `actions/setup-dotnet@v5` should install it

## Contribution Guidelines

### Code Changes
- Keep PRs focused — one logical change per PR
- Follow existing code style
- Add tests for new behaviour
- Update documentation

### Exception Mapping Additions
When adding a new exception mapping:
1. Add catch block in correct order (specific before general)
2. Use existing `NuciApiErrorResponse` static properties where possible
3. Add test case to parameterised test
4. Update `docs/exception-mapping.md`
5. Update `README.md` mapping table

### Breaking Changes
- Require MAJOR version bump
- Document migration in release notes
- Consider providing backward-compatible alternative first

## Related Documentation

- [Architecture Details](architecture.md)
- [Exception Mapping Reference](exception-mapping.md)
- [Testing Guide](testing.md)
- [Root ARCHITECTURE.md](../ARCHITECTURE.md)
- [README.md](../README.md)